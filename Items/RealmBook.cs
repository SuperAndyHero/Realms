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
using Terraria.ModLoader.IO;
using Realms.RealmData.BlockPatterns;
using Realms.UI;

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

        public RealmInfo realmInfo;
        public DebugBookUI DebugUI;

        public override TagCompound Save()
        {
            if (realmInfo != null)
                return RealmInfo.Save(realmInfo);
            return base.Save();
        }
        public override void Load(TagCompound tag)
        {
            if(tag.ContainsKey("realmInfo"))
                realmInfo = RealmInfo.Load(tag);
        }

        public override void UpdateInventory(Player player)
        {
            if(realmInfo != null)
                item.SetNameOverride(realmInfo.DisplayName);
        }

        public override bool AltFunctionUse(Player player) => true;
        public override bool UseItem(Player player)
        {

            if (DebugUI == null)
            {
                DebugUI = new DebugBookUI();
                DebugUI.realmItem = this;
                DebugUI.Activate();
            }
            else
                DebugUI.Activate();

            return true;

            //if (player.altFunctionUse == 2)
            //{
            //    //if (realmInfo == null)
            //        realmInfo = new RealmInfo(400, 400, "TestWorldsD35",
            //        new List<RealmEffect> { new CampfireBuffEffect().Setup() },
            //        new List<RealmFeature> { new Spheres().Setup(new int[] { TileID.Demonite, TileID.TungstenBrick, TileID.BubblegumBlock }, null, new PatternRadial().Setup(RealmFrequency.Slow))});
            //    //else
            //    //    Main.NewText(realmInfo.DisplayName);
            //}
            //else
            //    if (realmInfo != null)
            //        SubworldHandler.Enter(realmInfo);
            //    else
            //        Main.NewText("null");
        }
    }
}