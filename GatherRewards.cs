﻿using System.ComponentModel;
using Oxide.Core.Plugins;



//plugin.merge -c -m -p ./merge.json

namespace Oxide.Plugins
{
    [Info("Gather Rewards", "Shady14u", "1.6.9")]
    [Description("Earn rewards through Economics/Server Rewards for killing and gathering")]
    //Define:FileOrder=1
    public partial class GatherRewards : RustPlugin
    {
        [PluginReference] 
        private Plugin Economics, ServerRewards, Friends, Clans, UINotify;

        private string _resource;

        private string _version = "1.6.9";
    }
}