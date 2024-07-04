using Rocket.Core.Plugins;
using Logger = Rocket.Core.Logging.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using UnityEngine;
using SDG.Unturned;
using static Rocket.Unturned.Events.UnturnedPlayerEvents;
using System.Security.Cryptography;
using System.Threading;
using Steamworks;
using HarmonyLib;
using System.Collections;
using Rocket.Unturned.Extensions;
using UnityEngine.PlayerLoop;
using static Rocket.Unturned.Events.UnturnedEvents;
using Rocket.Unturned;

namespace EasyMetalDetector
{
    public class Main : RocketPlugin<Config>
    {
        public static Dictionary<CSteamID, uint> Update;
        public static Main Instance { get; set; }

        protected override void Load()
        {
            Update = new Dictionary<CSteamID, uint>();
            Instance = this;
            Logger.LogWarning("Loaded EasyMetalDetector");
            UnturnedPlayerEvents.OnPlayerUpdateGesture += OnPlayerGestured;
            StartCoroutine(CheckLoop());
            U.Events.OnPlayerConnected += Joining;
            foreach (var spot in Configuration.Instance.Spots)
            {
                spot.active = true;
            }
        }

        protected override void Unload()
        {
            Logger.LogWarning("UnLoaded EasyMetalDetector");
            UnturnedPlayerEvents.OnPlayerUpdateGesture -= OnPlayerGestured;
            U.Events.OnPlayerConnected -= Joining;
        }

        private void Joining(UnturnedPlayer player)
        {
            EffectManager.sendUIEffect(55904, 6689, player.Player.channel.owner.transportConnection, true);
        }

        private IEnumerator CheckLoop()
        {
            while (true)
            {
                foreach (var player in Provider.clients)
                {
                    if (player != null)
                    {
                        if (player.player.equipment.itemID != Configuration.Instance.MetalDetectorID)
                        {
                            EffectManager.sendUIEffectVisibility(6689, player.player.channel.owner.transportConnection, true, "HERE", false);
                            EffectManager.sendUIEffectVisibility(6689, player.player.channel.owner.transportConnection, true, "FAR", false);
                            EffectManager.sendUIEffectVisibility(6689, player.player.channel.owner.transportConnection, true, "MID", false);
                            continue;
                        }
                        if (!Update.ContainsKey(player.playerID.steamID))
                            Update.Add(player.playerID.steamID, 0);
                        foreach (var spot in Configuration.Instance.Spots)
                        {
                            var distance = (spot.Position - player.ToUnturnedPlayer().Position).magnitude;
                            if (!spot.active) continue;
                            if (distance < 2)
                            {
                                if (Update[player.playerID.steamID] == 3) yield return new WaitForSeconds(0.1f);
                                Update[player.playerID.steamID] = 3;
                                EffectManager.sendUIEffectVisibility(6689, player.player.channel.owner.transportConnection, true, "HERE", true);
                                EffectManager.sendUIEffectVisibility(6689, player.player.channel.owner.transportConnection, true, "FAR", false);
                                EffectManager.sendUIEffectVisibility(6689, player.player.channel.owner.transportConnection, true, "MID", false);
                                yield return new WaitForSeconds(0.1f);
                            }
                            else if (distance < 5)
                            {
                                if (Update[player.playerID.steamID] == 2) yield return new WaitForSeconds(0.1f);
                                Update[player.playerID.steamID] = 2;
                                EffectManager.sendUIEffectVisibility(6689, player.player.channel.owner.transportConnection, true, "HERE", false);
                                EffectManager.sendUIEffectVisibility(6689, player.player.channel.owner.transportConnection, true, "FAR", false);
                                EffectManager.sendUIEffectVisibility(6689, player.player.channel.owner.transportConnection, true, "MID", true);
                                yield return new WaitForSeconds(0.1f);
                            }
                            else if (distance < 10)
                            {
                                if (Update[player.playerID.steamID] == 1) yield return new WaitForSeconds(0.1f);
                                Update[player.playerID.steamID] = 1;
                                EffectManager.sendUIEffectVisibility(6689, player.player.channel.owner.transportConnection, true, "HERE", false);
                                EffectManager.sendUIEffectVisibility(6689, player.player.channel.owner.transportConnection, true, "FAR", true);
                                EffectManager.sendUIEffectVisibility(6689, player.player.channel.owner.transportConnection, true, "MID", false);
                                yield return new WaitForSeconds(0.1f);
                            }
                        }
                    }
                }
                yield return new WaitForSeconds(0.2f);
            }
        }

        private IEnumerator ReactivateSpot(Spots spot)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(30, 60));  
            spot.active = true;
            Configuration.Save();
        }

        private void OnPlayerGestured(UnturnedPlayer player, PlayerGesture gesture)
        {
            if (gesture == PlayerGesture.Point && player.Stance == EPlayerStance.CROUCH)
            {
                foreach (var Spot in Configuration.Instance.Spots)
                {
                    if ((player.Position - Spot.Position).magnitude < 2 && Spot.active)
                    {
                        int rolls = 0;
                        foreach (var item in Configuration.Instance.Items)
                        {
                            rolls += item.Chance;
                        }
                        int roll = UnityEngine.Random.Range(0, rolls);
                        int currentChance = 0;
                        foreach (var item in Configuration.Instance.Items)
                        {
                            currentChance += item.Chance;
                            if (roll < currentChance)
                            {
                                if (Spot.active)
                                {
                                    Spot.active = false;
                                    Configuration.Save();
                                    StartCoroutine(ReactivateSpot(Spot));
                                }
                                if (!player.GiveItem(item.ID, 1))
                                {
                                    ItemManager.dropItem(new SDG.Unturned.Item(item.ID, true), player.Position, false, false, true);
                                }
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
}