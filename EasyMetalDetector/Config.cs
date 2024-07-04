using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

namespace EasyMetalDetector
{
    public class Config : IRocketPluginConfiguration
    {
        public int MetalDetectorID; 

        public List<Spots> Spots = new List<Spots>(); 

        public List<ItemReward> Items = new List<ItemReward>(); 

        public void LoadDefaults()
        {
            MetalDetectorID = 1234; 

            Spots.Add(new Spots { SpotID = 1, Position = new Vector3(0, 0, 0), active = true });
            Items.Add(new ItemReward { ID = 3, Chance = 10 });
            Items.Add(new ItemReward { ID = 2, Chance = 60 });
        }
    }
    public class Spots
    {
        public int SpotID;
        public Vector3 Position;
        public bool active;
    }
    public class ItemReward
    {
        [XmlAttribute]
        public ushort ID;
        [XmlAttribute]
        public int Chance; 
    }
}