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

namespace Realms
{
	public class Realms : Mod
	{
		public override void Load()
        {
            ContentHandler.Load();
            ModelHandler.Load();
            var screenRef = new Ref<Effect>(GetEffect("Effects/NormalEffect"));
            Filters.Scene["NormalEffect"] = new Filter(new ScreenShaderData(screenRef, "Pass1"), EffectPriority.High);
            Filters.Scene["NormalEffect"].Load();

            GetModel("Realms/Models/altar").SetEffect(new BasicNormalEffect());
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