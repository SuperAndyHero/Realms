using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Effects;
using SubworldLibrary;
using static Realms.ContentHandler;
using Terraria.ID;
using Terraria.WorldBuilding;

namespace Realms.Items
{
    public class RealmAltarItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Realm Altar");
        }

        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.RealmAltar>();
        }
    }
}