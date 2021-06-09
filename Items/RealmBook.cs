using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.World.Generation;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Effects;
using SubworldLibrary;
using static Realms.ContentHandler;
using Terraria.ID;
using Realms.RealmData;
using Realms.RealmData.RealmEffects;
using Realms.RealmData.RealmFeatures;

namespace Realms.Items
{
    public class RealmBook : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Realm Book");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.rare = ItemRarityID.Green;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.HoldingUp;
        }

        public RealmInfo realmInfo = new RealmInfo(1000, 1000, "TestWorld", 
            new List<RealmEffect> { new CampfireBuffEffect().Setup() }, 
            new List<RealmFeature> { new Spheres().Setup(new int[] { TileID.Stone }, null, null) });

        public override bool UseItem(Player player)
        {
            Subworld.Enter<RealmSubworld>(false);
            return true;
        }
    }
}