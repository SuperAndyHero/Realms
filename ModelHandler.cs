using Terraria.ModLoader;
using System;
using Terraria;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.IO;
using System.Collections.Generic;
using Terraria.Graphics.Effects;
using Realms.Effects;

namespace Realms
{
	public static class ModelHandler
	{
		public static Matrix Projection_Ortho = new Matrix();
		public static Matrix Projection_Perspect = new Matrix();

		public static Matrix Projection_Ortho_Split = new Matrix();
		public static Matrix Projection_Perspect_Split = new Matrix();

		public static Matrix CameraView = new Matrix();
		//public static Matrix TileCameraView = new Matrix();


		public static Vector2 OldViewsize = new Vector2();

		public static Vector2 cameraPosition = new Vector2();

		public const int CameraDistance = 1930;

		public static List<CachedModelDraw> cachedModels = new List<CachedModelDraw>();

		public static void Load()
		{
			Projection_Ortho = Matrix.CreateOrthographic(Main.screenWidth, Main.screenHeight, 0, 5000);
			Projection_Perspect = Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 8f, Main.ViewSize.X / Main.ViewSize.Y, 1, 5000);

			Projection_Ortho_Split = Matrix.CreateOrthographic(Main.screenWidth, Main.screenHeight, 0, CameraDistance);
			Projection_Perspect_Split = Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 8f, Main.ViewSize.X / Main.ViewSize.Y, 1, CameraDistance);
		}

		public static void Update()
		{
			if (/*Main.screenPosition != Main.screenLastPosition ||*/ Main.ViewSize != OldViewsize)
			{
				//translation is now done on draw, which adds one vector subtraction per draw, but removes a matrix creation every update
				//this might be a issue later since it replaces (matrix mul * 1) with (Vector subtract * DrawCount) worth a test... later.
				//Vector2 pos = Main.screenPosition + Main.LocalPlayer.velocity;
				cameraPosition = (/*pos + */(Main.ViewSize / 2) + new Vector2(0, Main.ViewSize.Y * (Main.GameViewMatrix.Zoom.Y - 1))) * new Vector2(1, -1);
				CameraView = Matrix.CreateLookAt(new Vector3(cameraPosition, CameraDistance), new Vector3(cameraPosition, 0), Vector3.UnitY) * Main.GameViewMatrix.ZoomMatrix;
				//TileCameraView = Matrix.CreateLookAt(new Vector3(camPos, 383), new Vector3(camPos, 0), Vector3.UnitY);//broken matrix

				OldViewsize = Main.ViewSize;//a
			}
		}

		readonly public struct CachedModelDraw
        {
			readonly public Model model;
			readonly Matrix world;
            readonly bool perspective;

			public CachedModelDraw(Model model, Matrix world, bool perspective)
            {
				this.model = model;
				this.world = world;
				this.perspective = perspective;
            }
		}

		public static void DrawSplit(this Model model, Vector2 position, float scale = 1, float rotX = 0, float rotY = 0, float rotZ = 0, bool perspective = true)
		{
			cachedModels.Add(new CachedModelDraw(
				model,
				Matrix.CreateScale(scale) * Matrix.CreateFromYawPitchRoll(rotX, rotY, rotZ) * Matrix.CreateTranslation(new Vector3((position - Main.screenPosition) * new Vector2(1, -1), 0)),
				perspective));
		}

		public static void Draw(this Model model, Vector2 position, float scale = 1, float rotX = 0, float rotY = 0, float rotZ = 0, bool perspective = false)
		{
			Main.graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
			Matrix world = Matrix.CreateScale(scale) * Matrix.CreateFromYawPitchRoll(rotX, rotY, rotZ) * Matrix.CreateTranslation(new Vector3((position - Main.screenPosition) * new Vector2(1, -1), 0));
			foreach (ModelMesh mesh in model.Meshes)
			{
				foreach (Effect effect in mesh.Effects)
				{
					IEffectMatrices drawEffect = effect as IEffectMatrices;
					drawEffect.World = world;
					drawEffect.View = CameraView;
					drawEffect.Projection = perspective ? Projection_Perspect : Projection_Ortho;
				}
				mesh.Draw();
			}
		}

		public static void Draw(this Model model)
        {
			Main.graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
			foreach (ModelMesh mesh in model.Meshes)
				mesh.Draw();
		}

		//public static void TestDraw(this Model model, Vector2 position, float scale = 1, float rotX = 0, float rotY = 0, float rotZ = 0, bool perspective = false)
		//{
			//Effect effect = Filters.Scene["NormalEffect"].GetShader().Shader;

			//Main.graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
			//foreach (ModelMesh mesh in model.Meshes)
			//{
			//	foreach (ModelMeshPart part in mesh.MeshParts)
			//	{
			//		part.Effect = effect;
			//		effect.set
			//		effect.Parameters["World"].SetValue(Matrix.CreateScale(scale) * Matrix.CreateFromYawPitchRoll(rotX, rotY, rotZ) * Matrix.CreateTranslation(new Vector3(position * new Vector2(1, -1), 0)) * mesh.ParentBone.Transform);
			//		effect.View = CameraView;
			//		effect.Projection = perspective ? Projection_Perspect : Projection_Ortho;
			//	}
			//	mesh.Draw();
			//}
			//effect.Parameters["colorMod"].SetValue(color.ToVector4());
			//effect.Parameters["progress"].SetValue(progress);
			//effect.Parameters["progress2"].SetValue((20 - projectile.timeLeft) / 20f);
			//effect.Parameters["noise"].SetValue(ModContent.GetTexture("StarlightRiver/Assets/Noise/ShaderNoise"));
			//effect.CurrentTechnique.Passes[0].Apply();
		//}

		public static Vector3 LightingDirection(Vector2 position, out Vector3 averageColor, float distance = 4, int checks = 8, float lightingBias = 1)
        {
			Vector2 lightDirAver = Vector2.Zero;
			Vector3 colorAver = Vector3.Zero;
			for (int i = 0; i < checks; i++)
            {
				Vector2 dir = Vector2.UnitY.RotatedBy((float)(Math.PI / checks * i) * 2);
				Vector2 tileCheckPos = (position + (dir * distance));
				Vector3 color = Lighting.GetColor((int)tileCheckPos.X, (int)tileCheckPos.Y).ToVector3();

				lightDirAver += (dir * color.Length());
				colorAver += color;
			}
			lightDirAver /= checks;
			averageColor = (colorAver / checks);
			return Vector3.Normalize(new Vector3(-lightDirAver.X, lightDirAver.Y, -0.1f / lightingBias));//last number is lighting bias, lower is stronger
        }

		//wip
		//public static void MultiLightDirection(Vector2 position, out Vector3 dir0, out Vector3 color0, out Vector3 dir1, out Vector3 color1, out Vector3 dir2, out Vector3 color2, float distance = 64, int checks = 16, float lightingBias = 1)
		//{
		//	Vector2 lightDir0 = Vector2.Zero;
		//	Vector3 lightColor0 = Vector3.Zero;

		//	Vector2 lightDir1 = Vector2.Zero;
		//	Vector3 lightColor1 = Vector3.Zero;

		//	Vector2 lightDir2 = Vector2.Zero;
		//	Vector3 lightColor2 = Vector3.Zero;

		//	for (int i = 0; i < checks; i++)
		//	{
		//		Vector2 dir = Vector2.UnitY.RotatedBy((float)(Math.PI / checks * i) * 2);
		//		Vector2 tileCheckPos = (position + (dir * distance)) / 16;
		//		Vector3 color = Lighting.GetColor((int)tileCheckPos.X, (int)tileCheckPos.Y).ToVector3();

		//		if (color.Length() > lightColor0.Length())
		//		{

		//			lightDir2 = lightDir1;
		//			lightColor2 = lightColor1;

		//			lightDir1 = lightDir0;
		//			lightColor1 = lightColor0;

		//			lightDir0 = dir;
		//			lightColor0 = color;
		//		}
		//	}


		//	float level = -0.1f / lightingBias;

		//	dir0 = new Vector3(-lightDir0.X, lightDir0.Y, level);
		//	color0 = lightColor0;

		//	dir1 = new Vector3(-lightDir1.X, lightDir1.Y, level);
		//	color1 = lightColor1;

		//	dir2 = new Vector3(-lightDir2.X, lightDir2.Y, level);
		//	color2 = lightColor2;
		//}
	}
}