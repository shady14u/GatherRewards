﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace Oxide.Plugins
{
    //Define:FileOrder=4
    public partial class GatherRewards
    {
     
        private void Init()
        {
            LoadConfigValues();
            Language();
            RegisterPermsAndCommands();
        }

        private void OnCollectiblePickup(Item item, BasePlayer player)
        {
            if (!Economics && !ServerRewards) return;
            if (player == null) return;
            _resource = null;
            var amount = 0f;

            foreach (var configValue in config.Rewards)
            {
                if (!item.ToString().ToLower().Contains(configValue.Key.ToLower())) continue;
                amount = config.Rewards[configValue.Key];
                _resource = configValue.Key;
                break;
            }

            if (_resource != null && amount != 0)
            {
                GiveCredit(player, "gather", amount, _resource);
            }
        }

        private void OnDispenserGather(ResourceDispenser dispenser, BaseEntity entity, Item item)
        {
            if (!Economics && !ServerRewards) return;
            var player = entity?.ToPlayer();
            if (!player) return;
            var amount = 0f;
            var shortName = item.info.shortname;
            _resource = null;

            foreach (KeyValuePair<string, float> configValue in config.Rewards)
            {
                if (!shortName.Contains(configValue.Key.ToLower())) continue;
                amount = CheckPoints(configValue.Key);
                _resource = item.info.shortname;
                break;
            }

            if (_resource == null || amount == 0) return;

            if (player.GetHeldEntity() is Jackhammer)
            {
                amount *= config.Settings.JackHammerModifier;
            }
            if(player.GetHeldEntity() is Chainsaw)
            {
                amount *= config.Settings.ChainsawModifier;
            }

            if (config.Settings.AwardOnlyOnFullHarvest)
            {
                var ent = dispenser.GetComponent<BaseEntity>();
                NextTick(() =>
                {
                    
                    if (dispenser.gatherType == ResourceDispenser.GatherType.Tree && dispenser.fractionRemaining <=0)
                    {
                        GiveCredit(player, "gather", amount, "Tree");
                        return;
                    }

                    if (ent != null)
                    {
                        return;
                    }
                    
                    GiveCredit(player, "gather", amount, item.info.shortname);
                    
                    return;
                });
            
            }
            else
            {
                GiveCredit(player, "gather", amount, _resource);
            }

            
        }

        private void OnEntityDeath(BaseCombatEntity entity, HitInfo info)
        {
            if (entity == null) return;
            if (!Economics && !ServerRewards) return;
            var player = info?.Initiator?.ToPlayer();
            if (player==null) return;
            
            string animal;
            float amount;
            
            if (entity.ToPlayer() != null && !(entity is NPCPlayer))
            {
                //Player Killed was a Real Player
                var victim = entity.ToPlayer();
                if (player == victim)
                {
                    return;
                }

                amount = CheckPoints(PluginRewards.Player);
                animal = "player";

                if (Friends && config.Rewards[PluginRewards.PlayerFriend] != 0)
                {
                    var isFriend = Friends.Call<bool>("HasFriend", victim.userID, player.userID);
                    var isFriendReverse = Friends.Call<bool>("HasFriend", player.userID, victim.userID);
                    if (isFriend || isFriendReverse)
                    {
                        amount = CheckPoints(PluginRewards.PlayerFriend);
                        animal = "friend";
                    }
                }

                if (Clans && config.Rewards[PluginRewards.ClanMember] != 0)
                {
                    var victimClan = Clans.Call<string>("GetClanOf", victim.userID);
                    var playerClan = Clans.Call<string>("GetClanOf", player.userID);
                    if (victimClan == playerClan && !string.IsNullOrEmpty(playerClan))
                    {
                        amount = CheckPoints(PluginRewards.ClanMember);
                        animal = "clan member";
                    }
                }

                if (player.Team.members.Contains(victim.userID) && config.Rewards[PluginRewards.TeamMember] != 0)
                {
                    amount = CheckPoints(PluginRewards.TeamMember);
                    animal = "team member";
                }

            }
            else
            {
                if (entity is NPCPlayer)
                {
                    var npcPlayer = (NPCPlayer)entity;
                    animal = npcPlayer?.displayName ?? "Murderer";
                    
                    //Patch Tunnel Dwellers
                    if (entity.ShortPrefabName.Contains("npc_tunnel"))
                    {
                        animal = "Tunnel Dweller";
                    }
                    
                    //set the default amount to that of a murdered or 125
                    amount = CheckPoints(animal, CheckPoints("Murderer", 125));

                    long npcId;
                    if (long.TryParse(animal, out npcId))
                    {
                        //override names that are just numbers
                        amount = CheckPoints(entity.ShortPrefabName, CheckPoints("Murderer", 125));
                        animal = "Scientist";
                    }
                }
                else
                {
                    animal = entity.ShortPrefabName;
                    amount = CheckPoints(animal);
                }
            }

            if (amount != 0)
            {
                GiveCredit(player, "kill", amount, animal);
            }
        }

        
    }
}