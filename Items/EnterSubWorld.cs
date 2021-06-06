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
            //Vector3 modelPos = new Vector3((Main.LocalPlayer.Center + new Vector2(0, Main.LocalPlayer.gfxOffY)), 0);
            //Vector2 camPos = (Main.screenPosition + (Main.ViewSize / 2) + new Vector2(0, Main.ViewSize.Y * (Main.GameViewMatrix.Zoom.Y - 1))) * new Vector2(1, -1);
            Model model = GetModel("Realms/Models/Sans");



            //model.SetAlpha(0.4f);
            //model.SetMatrix(Matrix.CreateScale(modelScale) * Matrix.CreateFromYawPitchRoll(Main.GameUpdateCount / 15f, 0, 0) * Matrix.CreateTranslation(modelPos * new Vector3(1, -1, 1)),
            //    Matrix.CreateLookAt(new Vector3(camPos, 500), new Vector3(camPos, 0), Vector3.UnitY) * Main.GameViewMatrix.ZoomMatrix,
            //    Matrix.CreateOrthographic(Main.screenWidth, Main.screenHeight, 0, 1000));
            //ModContent.GetInstance<Realms>().model.Center();
            model.Draw(Main.LocalPlayer.Center + new Vector2(0, Main.LocalPlayer.gfxOffY), 50, (float)Main.GameUpdateCount / 15);
            model.DrawBoundingBox(spriteBatch, Color.Purple * 0.25f, 50);

            //Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 2f, Main.ViewSize.X / Main.ViewSize.Y, 1, 1000));
            //Matrix.CreateLookAt(new Vector3(0, 0, 383), new Vector3(0, 0, 0), Vector3.UnitY),
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