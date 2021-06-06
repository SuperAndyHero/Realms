using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.IO;
using Terraria;

namespace Realms
{
	public static class RealmExtensions
	{
		public static void SetTexture(this Model model, Texture2D texture)
		{
			foreach (ModelMesh mesh in model.Meshes)
				foreach (BasicEffect effect in mesh.Effects)
				{
					effect.TextureEnabled = true;
					effect.Texture = texture;
				}
		}
		public static void SetTexture(this Model model, string texture) =>
			SetTexture(model, ContentHandler.GetTexture(texture));

		public static void SetAlpha(this Model model, float alpha)
		{
			foreach (ModelMesh mesh in model.Meshes)
				foreach (BasicEffect effect in mesh.Effects)
					effect.Alpha = alpha;
		}

		public static void LightColors(this Model model, Color ambient, Color diffuse, Color emissive) =>
			LightColors(model, ambient.ToVector3(), diffuse.ToVector3(), emissive.ToVector3());
		public static void LightColors(this Model model, Vector3 ambient, Vector3 diffuse, Vector3 emissive = default)
		{
			foreach (ModelMesh mesh in model.Meshes)
				foreach (BasicEffect effect in mesh.Effects)
				{
					effect.AmbientLightColor = ambient;
					effect.DiffuseColor = diffuse;
					effect.EmissiveColor = emissive;
				}
		}

		public static void Specular(this Model model, Color color, float power)
		{
			Specular(model, color.ToVector3(), power);
		}
		public static void Specular(this Model model, Vector3 color, float power)
		{
			foreach (ModelMesh mesh in model.Meshes)
				foreach (BasicEffect effect in mesh.Effects)
				{
					effect.SpecularColor = color;
					effect.SpecularPower = power;
				}
		}


		public static void LightingSetting(this Model model, bool lightingEnabled = true, bool perPixelLighting = false, bool setDefaultLight = false)
		{
			foreach (ModelMesh mesh in model.Meshes)
				foreach (BasicEffect effect in mesh.Effects)
				{
					effect.LightingEnabled = lightingEnabled;
					effect.PreferPerPixelLighting = perPixelLighting;
					if(setDefaultLight)
						effect.EnableDefaultLighting();
				}
		}
		public static void DrawBoundingBox(this Model model, SpriteBatch spriteBatch, Color color, float scale = 1)
		{
			foreach (ModelMesh mesh in model.Meshes)
			{
				Vector3 size = new Vector3(new Vector2(mesh.BoundingSphere.Radius) * Main.GameViewMatrix.Zoom * scale, 0);
				Vector3 topLeft = (((mesh.Effects[0] as BasicEffect).World.Translation - (mesh.BoundingSphere.Center * size)) - new Vector3(size.X, -size.Y, 0)) - new Vector3(Main.screenPosition, 0);
				spriteBatch.Draw(Main.blackTileTexture, new Rectangle((int)topLeft.X, (int)topLeft.Y, (int)size.X * 2, (int)size.Y * 2), color);
			}
		}


		//public static void debug(this Model model, float alpha)
		//{
		//	foreach (ModelMesh mesh in model.Meshes)
		//	{
		//		foreach (BasicEffect effect in mesh.Effects)
		//		{
		//			effect.Alpha = alpha;// alpha;
		//			effect.SpecularPower = 6f;
		//			effect.SpecularColor = Color.LightGray.ToVector3();
		//			effect.DiffuseColor = new Vector3(0.2f, 0.7f, 0.5f);
		//			effect.EnableDefaultLighting();
		//			effect.AmbientLightColor = new Vector3(1f, 0.5f, 0.3f);
		//			//effect.LightingEnabled = true;
		//			effect.PreferPerPixelLighting = true;
		//			effect.TextureEnabled = false;
		//		}
		//	}
		//}

		//public static void SetMatrix(this Model model, Matrix world, Matrix view, Matrix projection)
		//{
		//	foreach (ModelMesh mesh in model.Meshes)
		//	{
		//		foreach (BasicEffect effect in mesh.Effects)
		//		{
		//			effect.World = world;
		//			effect.View = view;
		//			effect.Projection = projection;
		//		}
		//	}
		//}

		//public static void Draw(this Model model)
		//{
		//	Main.graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
		//	foreach (ModelMesh mesh in model.Meshes)
		//	{
		//		mesh.Draw();
		//	}
		//}
	}
}