using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Generation;
using Terraria.ModLoader.IO;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ObjectData;
using Terraria.UI;
using Terraria.Graphics.Effects;
using SubworldLibrary;
using Terraria.WorldBuilding;

namespace Realms.Items
{
    public class ExitSubWorld : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Exit Current World");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.rare = 2;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = 4;
        }

        public override bool? UseItem(Player player)/* tModPorter Suggestion: Return null instead of false */
        {
            Realms.testUI.Deactivate();

            //Subworld.Exit();
            return true;
        }
    }
}