using Terraria.ModLoader;
using System;
using Terraria;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.IO;
using static Realms.ContentHandler;
using Realms.Effects;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace Realms
{
	public class Realms : Mod
	{
        public static RenderTarget2D Target = Main.dedServ ? null : new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

        public override void Load()
        {
            ContentHandler.Load();
            ModelHandler.Load();
            var screenRef = new Ref<Effect>(GetEffect("Effects/NormalEffect"));
            Filters.Scene["NormalEffect"] = new Filter(new ScreenShaderData(screenRef, "Pass1"), EffectPriority.High);
            Filters.Scene["NormalEffect"].Load();

            Main.OnPreDraw += DrawToTargets;
            On.Terraria.Main.DrawTiles += BehindTiles;

            GetModel("Realms/Models/altar").SetEffect(new BasicNormalEffect());
        }

        private static void DrawToTargets(GameTime obj)
        {
            Main.graphics.GraphicsDevice.SetRenderTarget(Target);
            Main.graphics.GraphicsDevice.Clear(Color.Red);
        }

        //use a differnent method (that that gets called more often
        private static void BehindTiles(On.Terraria.Main.orig_DrawTiles orig, Main self, bool solidOnly = true, int waterStyleOverride = -1)
        {
            Main.spriteBatch.Draw(Target, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);
            orig(self, solidOnly, waterStyleOverride);
        }


        public override void PreUpdateEntities()
        {
           
        }

        public override void PostUpdateEverything()
        {
           
        }

        public override void PostUpdateInput()
        {
            //ModelHandler.Update();
        }
    }
}