using Terraria.ModLoader;
using System;
using Terraria;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.IO;
using System.Collections.Generic;

namespace Realms
{
	public static class ModelHandler
	{
		public static Matrix Projection_Ortho = new Matrix();
		public static Matrix Projection_Perspect = new Matrix();

		public static Matrix CameraView = new Matrix();
		//public static Matrix TileCameraView = new Matrix();


		public static Vector2 OldViewsize = new Vector2();

		public static void Load()
		{
			Projection_Ortho = Matrix.CreateOrthographic(Main.screenWidth, Main.screenHeight, 0, 5000);
			Projection_Perspect = Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 8f, Main.ViewSize.X / Main.ViewSize.Y, 1, 5000);
		}

		public static void Update()
		{
			if (Main.screenPosition != Main.screenLastPosition || Main.ViewSize != OldViewsize)
			{
				Vector2 pos = Main.screenPosition + Main.LocalPlayer.velocity;
				Vector2 camPos = (pos  + (Main.ViewSize / 2) + new Vector2(0, Main.ViewSize.Y * (Main.GameViewMatrix.Zoom.Y - 1))) * new Vector2(1, -1);
				CameraView = Matrix.CreateLookAt(new Vector3(camPos, 1930), new Vector3(camPos, 0), Vector3.UnitY) * Main.GameViewMatrix.ZoomMatrix;
				//TileCameraView = Matrix.CreateLookAt(new Vector3(camPos, 383), new Vector3(camPos, 0), Vector3.UnitY);//broken matrix

				OldViewsize = Main.ViewSize;
			}
		}

		public static void Draw(this Model model, Vector2 position, float scale = 1, float rotX = 0, float rotY = 0, float rotZ = 0, bool perspective = false)
		{
			Main.graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
			foreach (ModelMesh mesh in model.Meshes)
			{
				foreach (BasicEffect effect in mesh.Effects)
				{
					effect.World = Matrix.CreateScale(scale) * Matrix.CreateFromYawPitchRoll(rotX, rotY, rotZ) * Matrix.CreateTranslation(new Vector3(position * new Vector2(1, -1), 0));
					effect.View = CameraView;
					effect.Projection = perspective ? Projection_Perspect : Projection_Ortho;
				}
				mesh.Draw();
			}
		}

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
			return new Vector3(-lightDirAver.X, lightDirAver.Y, -0.1f / lightingBias);//last number is lighting bias, lower is stronger
        }

		//wip
		public static void MultiLightDirection(Vector2 position, out Vector3 dir0, out Vector3 color0, out Vector3 dir1, out Vector3 color1, out Vector3 dir2, out Vector3 color2, float distance = 64, int checks = 16, float lightingBias = 1)
		{
			Vector2 lightDir0 = Vector2.Zero;
			Vector3 lightColor0 = Vector3.Zero;

			Vector2 lightDir1 = Vector2.Zero;
			Vector3 lightColor1 = Vector3.Zero;

			Vector2 lightDir2 = Vector2.Zero;
			Vector3 lightColor2 = Vector3.Zero;

			for (int i = 0; i < checks; i++)
			{
				Vector2 dir = Vector2.UnitY.RotatedBy((float)(Math.PI / checks * i) * 2);
				Vector2 tileCheckPos = (position + (dir * distance)) / 16;
				Vector3 color = Lighting.GetColor((int)tileCheckPos.X, (int)tileCheckPos.Y).ToVector3();

				if (color.Length() > lightColor0.Length())
				{

					lightDir2 = lightDir1;
					lightColor2 = lightColor1;

					lightDir1 = lightDir0;
					lightColor1 = lightColor0;

					lightDir0 = dir;
					lightColor0 = color;
				}
			}


			float level = -0.1f / lightingBias;

			dir0 = new Vector3(-lightDir0.X, lightDir0.Y, level);
			color0 = lightColor0;

			dir1 = new Vector3(-lightDir1.X, lightDir1.Y, level);
			color1 = lightColor1;

			dir2 = new Vector3(-lightDir2.X, lightDir2.Y, level);
			color2 = lightColor2;
		}
	}
}