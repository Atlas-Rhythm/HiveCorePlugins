using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Hive.Extensions;
using Hive.Permissions;
using Hive.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Hive.PermissionQuery
{
    [Route("api/permissions/")]
    [ApiController]
    public class PermissionQueryController : ControllerBase
    {
        private readonly Serilog.ILogger log;
        private readonly IProxyAuthenticationService proxyAuth;
        private readonly PermissionsManager<PermissionContext> permissions;
        private readonly IOptions<PermissionQueryOptions> options;

        /// <summary>
        /// Create with DI
        /// </summary>
        /// <param name="log"></param>
        /// <param name="proxyAuth"></param>
        /// <param name="permissions"></param>
        /// <param name="options"></param>
        public PermissionQueryController([DisallowNull] Serilog.ILogger log, IProxyAuthenticationService proxyAuth,
            PermissionsManager<PermissionContext> permissions, IOptions<PermissionQueryOptions> options)
        {
            this.log = log;
            this.proxyAuth = proxyAuth;
            this.permissions = permissions;
            this.options = options;
        }

        [HttpGet("query")]
        public async Task<ActionResult<IDictionary<string, bool>>> Query([FromBody] string[] actions, [FromBody] PermissionContext context)
        {
            if (actions is null)
                return BadRequest("No list of actions to process.");

            // REVIEW: Should I return BadRequest on missing context?
            context ??= new PermissionContext();

            // REVIEW: Should we refuse outright with a non-whitelisted action?
            var filteredActions = actions.Where(it => options.Value.WhitelistedActions.Contains(it));

            if (!filteredActions.Any())
                return BadRequest("No whitelisted actions to process.");

            log.Information("Querying some permission actions: {actions}", actions);

            // REVIEW: Should the backend automatically fill User context like this? Or should it be left to the front end?
            var user = await HttpContext.GetHiveUser(proxyAuth).ConfigureAwait(false);
            context.User = user;

            // Key: Action, Value: Can the action be performed
            var results = new Dictionary<string, bool>();

            foreach (var action in filteredActions)
            {
                // REVIEW: Should parse states be used? What would that look like?
                results.Add(action, permissions.CanDo(action, context));
            }

            return results;
        }
    }
}
