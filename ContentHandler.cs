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
		private static MethodInfo createContentReader_Info;
		private static Func<ContentManager, Stream, string, Action<IDisposable>, ContentReader> createContentReader;

		private static MethodInfo readAsset_Info;
		private static Func<ContentReader, object> readAsset;

		public static string extension;

		public static Dictionary<string, object> assetCache = new Dictionary<string, object>();

		public static void Load(string xnbExtension = ".xnc")
		{
			extension = xnbExtension;
			//We grab info about a internal method in the xna framework
			createContentReader_Info = typeof(ContentReader).GetMethod("Create", BindingFlags.NonPublic | BindingFlags.Static);
			//Here we cache this method for performance
			createContentReader = (Func<ContentManager, Stream, string, Action<IDisposable>, ContentReader>)Delegate.CreateDelegate(
				typeof(Func<ContentManager, Stream, string, Action<IDisposable>, ContentReader>), createContentReader_Info);

			readAsset_Info = typeof(ContentReader).GetMethod("ReadAsset", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(typeof(object));
			readAsset = (Func<ContentReader, object>)Delegate.CreateDelegate(typeof(Func<ContentReader, object>), readAsset_Info);

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

		public static Model GetModel(string path) =>
			GetAsset<Model>(path);

		public static BasicEffect GetMaterial(string path) =>
			GetAsset<BasicEffect>(path);

		//Called this to match `ModContent.GetModTexture(str path)`
		public static Texture2D GetXnaTexture(string path) =>
			GetAsset<Texture2D>(path);

		//caches gotten assets
		public static T GetAsset<T>(string path)
		{
			if (assetCache.ContainsKey(path))
				return (T)assetCache[path];
			else
			{
				T asset = LoadAsset<T>(path + extension);
				assetCache.Add(path, asset);
				return asset;
			}
		}

		/// <summary>
		/// Reads asset from memory, Use GetAsset instead to get from cache if asset has already been read.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="path">includes extension</param>
		/// <returns></returns>
		public static T LoadAsset<T>(string path) =>
			LoadAsset<T>(new MemoryStream(ModContent.GetFileBytes(path)));

		/// <summary>
		/// Reads asset from memory, Use GetAsset instead to get from cache if asset has already been read.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="stream"></param>
		/// <returns></returns>
		public static T LoadAsset<T>(Stream stream)
		{
			using (ContentReader contentReader = createContentReader(Main.ShaderContentManager, stream, "", null))
				return (T)readAsset(contentReader);
		}
	}
}