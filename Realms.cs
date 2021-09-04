using Terraria.ModLoader;
using System;
using Terraria;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.IO;
using Realms.Effects;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using System.Collections.Generic;
using System.Linq;
using static Realms.ModelHandler;
using Terraria.DataStructures;

namespace Realms
{
	public class Realms : Mod
	{
        public static RenderTarget2D BackgroundTarget = Main.dedServ ? null : new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.DiscardContents);
        public static RenderTarget2D ForegroundTarget = Main.dedServ ? null : new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.DiscardContents);

        public static GUI testUI;
        public static GUI testUI2;

        public override void Load()
        {
            ContentHandler.Load();
            ModelHandler.Load();
            var screenRef = new Ref<Effect>(GetEffect("Effects/NormalEffect"));
            Filters.Scene["NormalEffect"] = new Filter(new ScreenShaderData(screenRef, "Pass1"), EffectPriority.High);
            Filters.Scene["NormalEffect"].Load();

            ContentHandler.GetModel("Realms/Models/altar").SetEffect(new BasicNormalEffect());


            On.Terraria.Main.DoDraw += DrawToTargets;
            On.Terraria.Main.DrawBG += BeforeWorld;
            On.Terraria.Main.DrawInfernoRings += AfterWorld;

            SetupTestUI();
        }

        public static void SetupTestUI()
        {
            testUI = new GUI()
            {
                Size = Vector2.One * 200,
            };
            testUI.AddElement(new Button()
            {
                Size = Vector2.One * 50,
                onClick = Click
            });
            testUI.AddElement(new ItemSlot()
            {
                Offset = Vector2.One * 100,
                Size = Vector2.One * 50
            });
            testUI.AddElement(new CloseButton(testUI));
            testUI.AddElement(new DragBar(testUI));

            testUI2 = new GUI()
            {
                Size = Vector2.One * 300,
            };
            testUI2.AddElement(new Button()
            {
                Size = Vector2.One * 50,
                onClick = Click
            });
            testUI2.AddElement(new CloseButton(testUI2));
            testUI2.AddElement(new DragBar(testUI2));
        }

        private static void Click()
        {
            Main.NewText("clicked");
        }

        private static void BeforeWorld(On.Terraria.Main.orig_DrawBG orig, Main self)
        {
            orig(self);
            Main.spriteBatch.Draw(BackgroundTarget, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);
        }

        private static void AfterWorld(On.Terraria.Main.orig_DrawInfernoRings orig, Main self)
        {
            orig(self);
            Main.spriteBatch.End();//A spritebatch swap to change the matrix, this spritebatch only draws a few things before this
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.BackgroundViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(ForegroundTarget, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);
            Main.spriteBatch.End();

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.Transform);
        }

        private static void DrawToTargets(On.Terraria.Main.orig_DoDraw orig, Main self, GameTime obj)
        {
            Main.graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            Main.graphics.GraphicsDevice.SetRenderTarget(BackgroundTarget);
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);
            foreach (CachedModelDraw modelDraw in cachedModels)
                foreach (ModelMesh mesh in modelDraw.model.Meshes)
                {
                    foreach (IEffectMatrices effect in mesh.Effects)
                    {
                        effect.World = Matrix.CreateScale(modelDraw.scale) * Matrix.CreateFromYawPitchRoll(modelDraw.rotX, modelDraw.rotY, modelDraw.rotZ) * Matrix.CreateTranslation(new Vector3(((modelDraw.position - (Main.screenPosition + Main.LocalPlayer.velocity))) * new Vector2(1, -1), 0));
                        effect.View = CameraView;
                        effect.Projection = modelDraw.perspective ? Projection_Perspect_Split_Far : Projection_Ortho_Split_Far;
                    }
                    mesh.Draw();
                }
            Main.graphics.GraphicsDevice.SetRenderTarget(null);


            Main.graphics.GraphicsDevice.SetRenderTarget(ForegroundTarget);
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);
            foreach (CachedModelDraw modelDraw in cachedModels)
                foreach (ModelMesh mesh in modelDraw.model.Meshes)
                {
                    foreach (IEffectMatrices effect in mesh.Effects)
                    {
                        effect.World = Matrix.CreateScale(modelDraw.scale) * Matrix.CreateFromYawPitchRoll(modelDraw.rotX, modelDraw.rotY, modelDraw.rotZ) * Matrix.CreateTranslation(new Vector3(((modelDraw.position - (Main.screenPosition + Main.LocalPlayer.velocity))) * new Vector2(1, -1), 0));
                        effect.View = CameraView;
                        effect.Projection = modelDraw.perspective ? Projection_Perspect_Split_Near : Projection_Ortho_Split_Near;
                    }
                    mesh.Draw();
                }
            Main.graphics.GraphicsDevice.SetRenderTarget(null);

            cachedModels = new List<CachedModelDraw>();

            orig(self, obj);
        }

        public override void PostUpdateEverything() =>
            ModelHandler.Update();

        public override void UpdateUI(GameTime gameTime) =>
            UIHandler.Update();

        public override void PostDrawInterface(SpriteBatch spriteBatch) =>
            UIHandler.Draw(spriteBatch);
    }
}