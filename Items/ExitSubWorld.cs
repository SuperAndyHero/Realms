using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.World.Generation;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Generation;
using Terraria.ModLoader.IO;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ObjectData;
using Terraria.UI;
using Terraria.Graphics.Effects;
using SubworldLibrary;

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
            item.width = 18;
            item.height = 18;
            item.rare = 2;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = 4;
        }

        public override bool UseItem(Player player)
        {
            Subworld.Exit();
            return true;
        }
    }
}