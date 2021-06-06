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
	public static class ContentHandler
	{
		private static MethodInfo create_ContentReader;
		private static MethodInfo readAsset;

		public static string extension;

		public static Dictionary<string, object> assetCache = new Dictionary<string, object>();

		public static void Load(string xnbExtension = ".xnc")
		{
			extension = xnbExtension;
			create_ContentReader = typeof(ContentReader).GetMethod("Create", BindingFlags.NonPublic | BindingFlags.Static);
			readAsset = typeof(ContentReader).GetMethod("ReadAsset", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(typeof(object));
			
			//quick info
			//MemoryStream streams from memory
			//FileStream streams from disk
			//Both extend from stream
			//Here we are swapping out a filestream for a memory stream

			//model type is 'Model' material type is 'BasicEffect'
			//modelSans = LoadAsset<Model>("Realms/Models/Sans.xnc");
			//modelSans.SetTexture(ModContent.GetTexture("Realms/Models/Sans Tex"));

			//modelRock = LoadAsset<Model>("Realms/Models/LargeAsteroid.xnc");
			//modelRock.SetTexture(ModContent.GetTexture("Realms/Models/AsteroidTexture"));

			//modelSphere = LoadAsset<Model>("Realms/Models/UntexturedSphere.xnc");
		}

		public static Model GetModel(string path)
        {
			if (assetCache.ContainsKey(path))
				return (Model)assetCache[path];
			else
			{
				Model model = LoadAsset<Model>(path + extension);
				assetCache.Add(path, model);
				return model;
			}
		}

		public static BasicEffect GetMaterial(string path)
		{
			if (assetCache.ContainsKey(path))
				return (BasicEffect)assetCache[path];
			else
			{
				BasicEffect basicEffect = LoadAsset<BasicEffect>(path + extension);
				assetCache.Add(path, basicEffect);
				return basicEffect;
			}
		}

		public static Texture2D GetTexture(string path)
		{
			if (assetCache.ContainsKey(path))
				return (Texture2D)assetCache[path];
			else
			{
				Texture2D texture = LoadAsset<Texture2D>(path + extension);
				assetCache.Add(path, texture);
				return texture;
			}
		}

		/// <summary>
		/// Reads asset from disk, Use GetAsset instead to get from cache if asset has already been read.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="path">includes extension</param>
		/// <returns></returns>
		public static T LoadAsset<T>(string path) =>
			LoadAsset<T>(new MemoryStream(ModContent.GetFileBytes(path)));

		/// <summary>
		/// Reads asset from disk, Use GetAsset instead to get from cache if asset has already been read.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="stream"></param>
		/// <returns></returns>
		public static T LoadAsset<T>(Stream stream)
		{
			using (ContentReader contentReader = (ContentReader)create_ContentReader.Invoke(null, new object[] { Main.ShaderContentManager, stream, "", null }))
				return (T)readAsset.Invoke(contentReader, null);
		}
	}
}