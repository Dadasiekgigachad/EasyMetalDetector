using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EasyMetalDetector
{
    public class AddSpotCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "addspot";
        public string Help => "Adds a metal detector spot at your current location";
        public string Syntax => "/addspot";
        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "d.MetalDetector.admin" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            int nextSpotID = Main.Instance.Configuration.Instance.Spots.Count + 1;

            Main.Instance.Configuration.Instance.Spots.Add(new Spots
            {
                SpotID = nextSpotID,
                Position = player.Position,
                active = true,
            });


            Main.Instance.Configuration.Save();

            ChatManager.say(player.CSteamID, "Metal detector spot added!", Color.green, true);
        }
    }
}