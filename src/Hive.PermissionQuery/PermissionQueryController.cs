﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Hive.Extensions;
using Hive.Models;
using Hive.Permissions;
using Hive.Services;
using Hive.Services.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly ModService modService;
        private readonly ChannelService channelService;

        /// <summary>
        /// Create with DI
        /// </summary>
        /// <param name="log"></param>
        /// <param name="proxyAuth"></param>
        /// <param name="permissions"></param>
        /// <param name="options"></param>
        public PermissionQueryController([DisallowNull] Serilog.ILogger log, IProxyAuthenticationService proxyAuth,
            PermissionsManager<PermissionContext> permissions, IOptions<PermissionQueryOptions> options,
            ModService modService, ChannelService channelService)
        {
            this.log = log;
            this.proxyAuth = proxyAuth;
            this.permissions = permissions;
            this.options = options;
            this.modService = modService;
            this.channelService = channelService;
        }

        [HttpGet("query")]
        public async Task<ActionResult<IDictionary<string, bool>>> Query([FromBody] string[] actions,
            [FromBody] ModIdentifier? modId, [FromBody] string? channelId)
        {
            if (actions is null)
                return BadRequest("No list of actions to process.");

            var context = new PermissionContext
            {
                User = await HttpContext.GetHiveUser(proxyAuth).ConfigureAwait(false)
            };

            if (modId != null)
            {
                var modQuery = await modService.GetMod(context.User, modId).ConfigureAwait(false);

                context.Mod = modQuery.Value;
            }

            if (channelId != null)
            {
                var channelQuery = await channelService.GetChannel(channelId, context.User).ConfigureAwait(false);

                context.Channel = channelQuery.Value;
            }

            log.Information("Querying some permission actions: {actions}", actions);

            // Key: Action, Value: Can the action be performed
            var results = new Dictionary<string, bool>();

            foreach (var action in actions)
            {
                // REVIEW: Is it a good idea to assume an empty whitelist means the instance owner wants every permission rule available to query?
                if (options.Value.WhitelistedActions.Contains(action) || options.Value.WhitelistedActions.Count == 0)
                {
                    // REVIEW: Should parse states be used? What would that look like?
                    results.Add(action, permissions.CanDo(action, context));
                }
                else
                {
                    results.Add(action, false);
                }
            }

            return results;
        }
    }
}
