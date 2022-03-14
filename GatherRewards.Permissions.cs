using Oxide.Core;
using Oxide.Game.Rust.Libraries;

namespace Oxide.Plugins
{
    //Define:FileOrder=2
    public partial class GatherRewards
    {
     
        private bool CheckPermission(BasePlayer player, string perm)
        {
            return permission.UserHasPermission(player.UserIDString, perm);
        }

        private void RegisterPermsAndCommands()
        {
            permission.RegisterPermission(config.Settings.EditPermission, this);
            foreach (var groupModifier in config.Settings.GroupModifiers)
            {
                permission.RegisterPermission(groupModifier.Key,this);
            }

            var command = Interface.Oxide.GetLibrary<Command>();
            command.AddChatCommand(config.Settings.ChatEditCommand, this, "cmdGatherRewards");
            command.AddConsoleCommand(config.Settings.ConsoleEditCommand, this, "cmdConsoleGatherRewards");
        }

    }
}