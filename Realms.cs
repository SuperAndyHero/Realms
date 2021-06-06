using Terraria.ModLoader;
using System;
using Terraria;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.IO;

namespace Realms
{
	public class Realms : Mod
	{
		public override void Load()
        {
            ContentHandler.Load();
            ModelHandler.Load();
        }
        public override void PostUpdateEverything()
        {
            ModelHandler.Update();
        }
    }
}