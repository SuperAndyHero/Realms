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
using Terraria.WorldBuilding;

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
            Item.width = 18;
            Item.height = 18;
            Item.rare = 2;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = 4;
            //GetModel("Realms/Models/Sans").SetTexture("Realms/Models/Sans Tex");
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Model model = GetModel("Realms/Models/LargeAsteroid_flat");
            model.BESetModTexture("Realms/Models/AsteroidTexture");
            //Model model = GetModel("Realms/Models/altar_smooth");
            //model.SetModTexture("Realms/Models/altar");
            model.BELightingSetting(true, true);
            model.BESpecular(10);
            model.BESetAlpha(0.5f);

            Vector3 dir = ModelHandler.LightingDirection(Main.LocalPlayer.Center / 16, out Vector3 averageColor, 4, 8, 0.5f);
            model.BEAmbientColor(averageColor * 0.25f);
            model.BEDirectionalLight0(true, dir, averageColor / 4, averageColor);
            model.Draw(Main.LocalPlayer.Center + new Vector2(0, Main.LocalPlayer.gfxOffY), 50f, (float)Main.GameUpdateCount / 50, (float)Main.GameUpdateCount / 67, 0, false);
            //model.DrawBoundingBox(spriteBatch, Main.LocalPlayer.Center + new Vector2(0, Main.LocalPlayer.gfxOffY), Color.Purple * 0.25f, 50);
        }

        public override bool? UseItem(Player player)/* tModPorter Suggestion: Return null instead of false */
        {
            //TeleportHandler.InitiateTeleportEvent();
            Realms.testUI.Deactivate();
            Realms.testUI2.Deactivate();
            Realms.SetupTestUI();
            Realms.testUI.Activate();
            Realms.testUI2.Activate();

            //Subworld.Enter<Realm>(false);

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