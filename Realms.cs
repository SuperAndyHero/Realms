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
        
        private static MethodInfo renderBlackInfo = typeof(Main).GetMethod("RenderBlack", BindingFlags.NonPublic | BindingFlags.Instance);
        public static Action RenderBlack = (Action)Delegate.CreateDelegate(typeof(Action), Main.instance, renderBlackInfo);

        public static MethodInfo drawTilesInfo = typeof(Main).GetMethod("DrawTiles", BindingFlags.NonPublic | BindingFlags.Instance);
        public static Action<bool, int> DrawTiles = (Action<bool, int>)Delegate.CreateDelegate(typeof(Action<bool, int>), Main.instance, drawTilesInfo);
        public override void Load()
        {
            ContentHandler.Load();
            ModelHandler.Load();
            var screenRef = new Ref<Effect>(GetEffect("Effects/NormalEffect"));
            Filters.Scene["NormalEffect"] = new Filter(new ScreenShaderData(screenRef, "Pass1"), EffectPriority.High);
            Filters.Scene["NormalEffect"].Load();
            On.Terraria.Main.RenderTiles += BehindTiles;

            GetModel("Realms/Models/altar").SetEffect(new BasicNormalEffect());
        }

        private static void BehindTiles(On.Terraria.Main.orig_RenderTiles orig, Main self)
        {
            Main.graphics.GraphicsDevice.SetRenderTarget(Target);
            Main.graphics.GraphicsDevice.Clear(Color.Red);
            Main.spriteBatch.Begin();
            Main.spriteBatch.Draw(Main.blackTileTexture, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Green);
            Main.spriteBatch.End();

            if (!Main.drawToScreen)
            {
                RenderBlack();
                Main.graphics.GraphicsDevice.SetRenderTarget(Main.instance.tileTarget);
                Main.graphics.GraphicsDevice.Clear(Color.Transparent);
                Main.spriteBatch.Begin();
                Main.spriteBatch.Draw(Target, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin();
                try
                {
                    DrawTiles(true, -1);
                }
                catch (Exception e)
                {
                    TimeLogger.DrawException(e);
                }
                TimeLogger.DetailedDrawReset();
                Main.spriteBatch.End();
                TimeLogger.DetailedDrawTime(28);
                Main.graphics.GraphicsDevice.SetRenderTarget(null);
            }

            //Main.graphics.GraphicsDevice.SetRenderTarget(Target);
            //Main.graphics.GraphicsDevice.Clear(Color.Transparent);
            //Main.spriteBatch.Begin();

            //Main.spriteBatch.End();

            //Main.graphics.GraphicsDevice.SetRenderTarget(null);
            ////Main.spriteBatch.Begin();//this gets drawn somewhere else instead, but the RT is filled here so the cache can be cleared
            ////Main.spriteBatch.Draw(Target, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);
            ////Main.spriteBatch.End();
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