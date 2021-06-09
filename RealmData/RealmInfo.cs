using Terraria.ModLoader;
using SubworldLibrary;
using System;
using Terraria.World.Generation;
using System.Collections.Generic;
using Terraria.Utilities;
using Terraria;
using System.Reflection;
using Terraria.ModLoader.IO;
using static Realms.SubworldHandler;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using System.Text;

namespace Realms.RealmData
{
    public class RealmInfo
    {
        public int Width;
        public int Height;
        public string DisplayName;
        public int Seed;
        public string RealmID;

        public RealmInfo(int width, int height, string displayName, List<RealmEffect> effectList, List<RealmFeature> featureList, int seed = 0)
        {
            Width = width;
            Height = height;
            DisplayName = displayName;
            if (seed == 0)
            {
                Main.rand = new UnifiedRandom((int)DateTime.Now.Ticks);
                Seed = Main.rand.Next();
            }
            else
                Seed = seed;

            realmEffectList = effectList;
            realmFeatureList = featureList;
            Main.NewText("New realm info initalized");
        }


        public List<RealmEffect> realmEffectList;
        public List<RealmFeature> realmFeatureList;

        public void CreateId()
        {
            byte[] idComination = Encoding.ASCII.GetBytes(Width + Height + DisplayName + Seed + realmEffectList.Count + realmFeatureList.Count);
            RealmID = Convert.ToBase64String(idComination);
            Main.NewText("Id Created");
        }
    }
}