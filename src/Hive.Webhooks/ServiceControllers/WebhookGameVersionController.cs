using Hive.Models;
using Hive.Services.Common;

namespace Hive.Webhooks.ServiceControllers
{
    public class WebhookGameVersionController : IGameVersionsPlugin
    {
        public void NewGameVersionCreated(GameVersion gameVersion)
        {

        }
    }
}
