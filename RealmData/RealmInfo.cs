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

namespace Realms.RealmData
{
    public class RealmInfo
    {
        int Width;
        int Height;
        string DisplayName;
        int Seed;

        public RealmInfo(int width, int height, string displayName, int seed = 0)
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
        }

        public List<RealmEffect> realmEffects;
        public List<RealmFeature> realmFeatures;

    }
}