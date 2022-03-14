using System;
using System.Linq;


namespace Oxide.Plugins
{
    //Define:FileOrder=5
    public partial class GatherRewards
    {
        private float CheckPoints(string animal, float defaultAmount = 0)
        {
            foreach (var configValue in config.Rewards.Where(configValue =>
                         string.Equals(animal, configValue.Key, StringComparison.CurrentCultureIgnoreCase)))
            {
                animal = configValue.Key;
                return config.Rewards[configValue.Key];
            }

            if (defaultAmount == 0) return defaultAmount;

            config.Rewards.Add(animal, defaultAmount);
            Config.WriteObject(config);

            return defaultAmount;
        }

        private void GiveCredit(BasePlayer player, string type, float amount, string gathered)
        {
            if(amount==0) return;
            foreach (var groupModifier in config.Settings.GroupModifiers.OrderByDescending(x=>x.Value))
            {
                if (permission.UserHasPermission(player.UserIDString, groupModifier.Key))
                {
                    amount *= float.Parse(groupModifier.Value.ToString());
                    break;
                }
            }

            if (amount > 0)
            {
                if (config.Settings.UseEconomics && Economics)
                {
                    Economics.Call("Deposit", player.UserIDString, (double)amount);
                }

                if (config.Settings.UseServerRewards && ServerRewards)
                {
                    ServerRewards.Call("AddPoints", new object[] { player.userID, (int)amount });
                }

                if (type == "gather" && config.Settings.ShowMessagesOnGather)
                {
                    PrintToChat(player,
                        config.Settings.PluginPrefix + " " + string.Format(Lang("ReceivedForGather", player), amount,
                            gathered.ToLower()));
                }
                else if (type == "kill" && config.Settings.ShowMessagesOnKill)
                {
                    PrintToChat(player,
                        config.Settings.PluginPrefix + " " + string.Format(Lang("ReceivedForKill", player), amount,
                            gathered.ToLower()));
                }
            }
            else
            {
                amount = Math.Abs(amount);

                if (config.Settings.UseEconomics && Economics)
                {
                    Economics.Call("Withdraw", player.UserIDString, (double)amount);
                }

                if (config.Settings.UseServerRewards && ServerRewards)
                {
                    ServerRewards.Call("TakePoints", new object[] { player.userID, (int)amount });
                }

                if (type == "gather" && config.Settings.ShowMessagesOnGather)
                {
                    PrintToChat(player,
                        config.Settings.PluginPrefix + " " +
                        string.Format(Lang("LostForGather", player), amount, gathered.ToLower()));
                }
                else if (type == "kill" && config.Settings.ShowMessagesOnKill)
                {
                    PrintToChat(player,
                        config.Settings.PluginPrefix + " " +
                        string.Format(Lang("LostForKill", player), amount, gathered.ToLower()));
                }
            }
        }

        private static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;
            var a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

    }
}