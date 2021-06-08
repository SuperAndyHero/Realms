using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.IO;
using Terraria;
using Terraria.ModLoader;

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
		public static void SetXnaTexture(this Model model, string texture) =>
			SetTexture(model, ContentHandler.GetXnaTexture(texture));
		public static void SetModTexture(this Model model, string texture) =>
			SetTexture(model, ModContent.GetTexture(texture));

		public static void SetAlpha(this Model model, float alpha)
		{
			foreach (ModelMesh mesh in model.Meshes)
				foreach (BasicEffect effect in mesh.Effects)
					effect.Alpha = alpha;
		}

		public static void DirectionalLight0(this Model model, bool enabled, Vector3 direction, Color diffuseColor = default, Color specularColor = default) =>
			DirectionalLight0(model, enabled, direction, diffuseColor.ToVector3(), specularColor.ToVector3());
		public static void DirectionalLight0(this Model model, bool enabled, Vector3 direction = default, Vector3 diffuseColor = default, Vector3 specularColor = default)
		{
			foreach (ModelMesh mesh in model.Meshes)
				foreach (BasicEffect effect in mesh.Effects)
				{
					effect.DirectionalLight0.Enabled = enabled;
					effect.DirectionalLight0.Direction = Vector3.Normalize(direction);
					effect.DirectionalLight0.DiffuseColor = diffuseColor;
					effect.DirectionalLight0.SpecularColor = specularColor;
				}
		}

		public static void DirectionalLight1(this Model model, bool enabled, Vector3 direction, Color diffuseColor = default, Color specularColor = default) =>
			DirectionalLight1(model, enabled, direction, diffuseColor.ToVector3(), specularColor.ToVector3());
		public static void DirectionalLight1(this Model model, bool enabled, Vector3 direction = default, Vector3 diffuseColor = default, Vector3 specularColor = default)
		{
			foreach (ModelMesh mesh in model.Meshes)
				foreach (BasicEffect effect in mesh.Effects)
				{
					effect.DirectionalLight1.Enabled = enabled;
					effect.DirectionalLight1.Direction = Vector3.Normalize(direction);
					effect.DirectionalLight1.DiffuseColor = diffuseColor;
					effect.DirectionalLight1.SpecularColor = specularColor;
				}
		}

		public static void DirectionalLight2(this Model model, bool enabled, Vector3 direction, Color diffuseColor = default, Color specularColor = default) =>
			DirectionalLight2(model, enabled, direction, diffuseColor.ToVector3(), specularColor.ToVector3());
		public static void DirectionalLight2(this Model model, bool enabled, Vector3 direction = default, Vector3 diffuseColor = default, Vector3 specularColor = default)
		{
			foreach (ModelMesh mesh in model.Meshes)
				foreach (BasicEffect effect in mesh.Effects)
				{
					effect.DirectionalLight2.Enabled = enabled;
					effect.DirectionalLight2.Direction = Vector3.Normalize(direction);
					effect.DirectionalLight2.DiffuseColor = diffuseColor;
					effect.DirectionalLight2.SpecularColor = specularColor;
				}
		}

		public static void AmbientColor(this Model model, Color ambient) =>
			AmbientColor(model, ambient.ToVector3());
		public static void AmbientColor(this Model model, Vector3 ambient)
		{
			foreach (ModelMesh mesh in model.Meshes)
				foreach (BasicEffect effect in mesh.Effects)
					effect.AmbientLightColor = ambient;
		}

		public static void DiffuseColor(this Model model, Color diffuse = default) =>
			DiffuseColor(model, diffuse.ToVector3());
		public static void DiffuseColor(this Model model, Vector3 diffuse = default)
		{
			foreach (ModelMesh mesh in model.Meshes)
				foreach (BasicEffect effect in mesh.Effects)
					effect.DiffuseColor = diffuse;
		}

		public static void EmissiveColor(this Model model, Color emissive = default) =>
			EmissiveColor(model, emissive.ToVector3());
		public static void EmissiveColor(this Model model, Vector3 emissive = default)
		{
			foreach (ModelMesh mesh in model.Meshes)
				foreach (BasicEffect effect in mesh.Effects)
					effect.EmissiveColor = emissive;
		}

		public static void Specular(this Model model, float power) =>
			Specular(model, power, Vector3.One);
		public static void Specular(this Model model, float power, Color filterColor) =>
			Specular(model, power, filterColor.ToVector3());
		public static void Specular(this Model model, float power, Vector3 filterColor)
		{
			foreach (ModelMesh mesh in model.Meshes)
				foreach (BasicEffect effect in mesh.Effects)
                {
					effect.SpecularColor = filterColor;
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
		public static void DrawBoundingBox(this Model model, SpriteBatch spriteBatch, Vector2 position, Color color, float scale = 1)
		{
			foreach (ModelMesh mesh in model.Meshes)
			{
				Vector3 size = new Vector3(new Vector2(mesh.BoundingSphere.Radius) * Main.GameViewMatrix.Zoom * scale, 0);
				Vector3 topLeft = ((new Vector3(position, 0) - (mesh.BoundingSphere.Center * size)) - new Vector3(size.X, -size.Y, 0)) - new Vector3(Main.screenPosition, 0);
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