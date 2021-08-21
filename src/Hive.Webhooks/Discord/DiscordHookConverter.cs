using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hive.Models;

namespace Hive.Webhooks.Discord
{
    public class DiscordHookConverter : IHookConverter
    {
        public string ID => nameof(Discord);

        public object? ChannelCreated(Channel channel)
        {
            return null;
        }
    }
}
