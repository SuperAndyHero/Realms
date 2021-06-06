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
		public static Matrix CameraView = new Matrix();
		public static Matrix Projection_Ortho = new Matrix();

		public static Vector2 OldScreenPos = new Vector2();
		public static Vector2 OldViewsize = new Vector2();

		public static void Load()
		{
			Projection_Ortho = Matrix.CreateOrthographic(Main.screenWidth, Main.screenHeight, 0, 2000);
		}

		public static void Update()
		{
			if (Main.screenPosition != OldScreenPos || Main.ViewSize != OldViewsize)
			{
				Vector2 camPos = (Main.screenPosition + (Main.ViewSize / 2) + new Vector2(0, Main.ViewSize.Y * (Main.GameViewMatrix.Zoom.Y - 1))) * new Vector2(1, -1);
				CameraView = Matrix.CreateLookAt(new Vector3(camPos, 1000), new Vector3(camPos, 0), Vector3.UnitY) * Main.GameViewMatrix.ZoomMatrix;
				OldScreenPos = Main.screenPosition;
				OldViewsize = Main.ViewSize;
			}
		}

		public static void Draw(this Model model, Vector2 position, float scale = 1, float rotX = 0, float rotY = 0, float rotZ = 0)
		{
			Main.graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
			foreach (ModelMesh mesh in model.Meshes)
			{
				foreach (BasicEffect effect in mesh.Effects)
				{
					effect.World = Matrix.CreateScale(scale) * Matrix.CreateFromYawPitchRoll(rotX, rotY, rotZ) * Matrix.CreateTranslation(new Vector3(position * new Vector2(1, -1), 0));
					effect.View = CameraView;
					effect.Projection = Projection_Ortho;
				}

				mesh.Draw();
			}
		}
	}
}