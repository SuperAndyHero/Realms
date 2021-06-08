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
using static Realms.ContentHandler;

namespace Realms.Items
{
    public class EnterSubWorld : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("ConfigTool");
            Tooltip.SetDefault("Enter World.");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.rare = 2;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = 4;
            //GetModel("Realms/Models/Sans").SetTexture("Realms/Models/Sans Tex");
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Model model = GetModel("Realms/Models/LargeAsteroid_flat");
            model.SetModTexture("Realms/Models/AsteroidTexture");
            model.LightingSetting(true, true);
            model.Specular(10);

            Vector3 dir = ModelHandler.LightingDirection(Main.LocalPlayer.Center, out Vector3 averageColor, 72, 8, 0.5f);
            model.AmbientColor(averageColor * 0.25f);
            model.DirectionalLight0(true, dir, averageColor / 4, averageColor);
            model.Draw(Main.LocalPlayer.Center + new Vector2(0, Main.LocalPlayer.gfxOffY), 50, (float)Main.GameUpdateCount / 50);
            //model.DrawBoundingBox(spriteBatch, Main.LocalPlayer.Center + new Vector2(0, Main.LocalPlayer.gfxOffY), Color.Purple * 0.25f, 50);
        }

        public override bool UseItem(Player player)
        {
            //TeleportHandler.InitiateTeleportEvent();

            Subworld.Enter<RealmSubworld>(false);

            //if (Subworld.IsActive<Ship.ShipSubworld>())
            //{
            //    Subworld.Enter(planetArray[SelectedPlanet].EntryString);
            //}
            //else
            //{
            //    Subworld.Enter<Ship.ShipSubworld>();
            //}
            return true;
        }
    }
}