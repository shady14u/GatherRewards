using System.Collections.Generic;


namespace Oxide.Plugins
{
    //Define:FileOrder=6
    public partial class GatherRewards
    {
        private void cmdGatherRewards(BasePlayer player, string command, string[] args)
        {
            if (!(CheckPermission(player, config.Settings.EditPermission)))
            {
                SendReply(player, config.Settings.PluginPrefix + " " + Lang("NoPermission", player.UserIDString));
                return;
            }

            if (args.Length < 2)
            {
                SendReply(player,
                    config.Settings.PluginPrefix + " " + string.Format(Lang("Usage", player.UserIDString),
                        config.Settings.ChatEditCommand));
                return;
            }

            float value;
            if (float.TryParse(args[1], out value) == false)
            {
                SendReply(player, config.Settings.PluginPrefix + " " + Lang("NotaNumber", player.UserIDString));
                return;
            }

            switch (args[0].ToLower())
            {
                case "clan":
                {
                    config.Rewards[PluginRewards.ClanMember] = value;
                    Config.WriteObject(config);
                    SendReply(player,
                        config.Settings.PluginPrefix + " " +
                        string.Format(Lang("Success", player.UserIDString), "clan member", value));
                    break;
                }
                case "friend":
                {
                    config.Rewards[PluginRewards.PlayerFriend] = value;
                    Config.WriteObject(config);
                    SendReply(player,
                        config.Settings.PluginPrefix + " " +
                        string.Format(Lang("Success", player.UserIDString), "friend", value));
                    break;
                }
                default:
                {
                    if (!config.Rewards.ContainsKey(UppercaseFirst(args[0].ToLower())))
                    {
                        SendReply(player,
                            config.Settings.PluginPrefix + " " +
                            string.Format(Lang("ValueDoesNotExist", player.UserIDString), args[0].ToLower()));
                        break;
                    }

                    config.Rewards[UppercaseFirst(args[0].ToLower())] = float.Parse(args[1]);
                    Config.WriteObject(config);
                    
                    SendReply(player,
                        config.Settings.PluginPrefix + " " + string.Format(Lang("Success", player.UserIDString),
                        args[0].ToLower(), value));
                    
                    break;
                }
            }
        }

        private void cmdConsoleGatherRewards(ConsoleSystem.Arg arg)
        {
            if (arg.IsAdmin != true)
            {
                return;
            }

            if (arg.Args == null)
            {
                Puts(string.Format(Lang("Usage"), config.Settings.ConsoleEditCommand));
                return;
            }

            if (arg.Args.Length <= 1)
            {
                Puts(string.Format(Lang("Usage"), config.Settings.ConsoleEditCommand));
                return;
            }

            float value = 0;
            if (float.TryParse(arg.Args[1], out value) == false)
            {
                Puts(Lang("NotaNumber"));
                return;
            }

            switch (arg.Args[0].ToLower())
            {
                case "clan":
                {
                    config.Rewards[PluginRewards.ClanMember] = value;
                    Config.WriteObject(config);
                    Puts(string.Format(Lang("Success"), "clan member", value));
                    break;
                }
                case "friend":
                {
                    config.Rewards[PluginRewards.PlayerFriend] = value;
                    Config.WriteObject(config);
                    Puts(string.Format(Lang("Success"), "friend", value));
                    break;
                }
                default:
                {
                    if (!config.Rewards.ContainsKey(UppercaseFirst(arg.Args[0].ToLower())))
                    {
                        Puts(string.Format(Lang("ValueDoesNotExist"), arg.Args[0].ToLower()));
                        break;
                    }
                    
                    config.Rewards[UppercaseFirst(arg.Args[0].ToLower())] = float.Parse(arg.Args[1]);
                    Config.WriteObject(config);
                    Puts(string.Format(Lang("Success"), arg.Args[0].ToLower(), value));
                    
                    break;
                }
            }
        }
    }
}