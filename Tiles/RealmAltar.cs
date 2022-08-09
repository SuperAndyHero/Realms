using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Realms.Effects;
using Realms.UI;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;
using static Realms.ContentHandler;
using static Realms.TEWorld;

namespace Realms.Tiles
{
	public class RealmAltar : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = false;
			Main.tileMergeDirt[Type] = false;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			//Main.tileLighted[Type] = true;

			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Realm Altar");
			AddMapEntry(new Color(200, 200, 200), name);
			DustType = DustID.Gold;
			ItemDrop = ModContent.ItemType<Items.RealmAltarItem>();


			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<AltarEntity>().Hook_AfterPlacement, -1, 0, true);


			TileObjectData.newTile.DrawYOffset = 2;

			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };

			TileObjectData.newTile.Width = 6;
			TileObjectData.newTile.Height = 2;

			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.addTile(Type);

			//drop = todo;
			MinPick = 200;
		}

		public override bool RightClick(int i, int j)
		{
			Tile tile = Main.tile[i, j];
			int left = i - tile.TileFrameX / 18;
			int top = j - tile.TileFrameY / 18;

			int index = ModContent.GetInstance<AltarEntity>().Find(left, top);
			if (index == -1)
			{
				Main.NewText("Tile broken! Please place this again!");
				return false;
			}

			AltarEntity entity = (AltarEntity)TileEntity.ByID[index];
			entity.OpenUI();
			//foreach (var item in tEScoreBoard.scores)
			//{
			//	Main.NewText(item.Key + ": " + item.Value);
			//}
			return true;
		}

		public override void NumDust(int i, int j, bool fail, ref int num) => num = 1;
		public override bool Drop(int i, int j) => false;
		public override void KillMultiTile(int i, int j, int frameX, int frameY) => Item.NewItem(new Vector2(i, j) * 16, new Vector2(5, 3) * 16, ItemDrop);//change here if size change

    }



	public class AltarEntity : SimpleEntity, IDrawableTE
	{
		public GUI AltarUI;

		public AltarItemData ItemData;

		public void OpenUI()
        {
			if (AltarUI != null)//debug
				AltarUI.Deactivate();//debug
			//if (AltarUI == null)
				AltarUI = new RealmAltarUI(this);

			if (PlayerInRange)
				AltarUI.Activate();
		}

		const int PlayerMaxRange = 150;
		private bool PlayerInRange => 
			(Main.LocalPlayer.position - ((Position.ToVector2() + new Vector2(2.5f, 0.5f)) * 16)).Length() < PlayerMaxRange;

		public override void Update()
        {
            if (!PlayerInRange)
				AltarUI.Deactivate();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
			Vector2 centerPos = new Vector2(Position.X + 3, Position.Y - 3);
			Vector2 pos = centerPos + new Vector2(0, ((float)Math.Sin((float)Main.GameUpdateCount / 40) * 0.2f));

			Lighting.AddLight(centerPos * 16, 0.25f, 0.4f, 0.25f);

			Model model0 = GetModel("Realms/Models/sphere");
			model0.BESetTexture(ModContent.GetTexture("Realms/Models/sphere_tex_png"));
			model0.BEEmissiveColor(Color.White);
			model0.Draw((pos * 16), (float)(Math.Sin((float)Main.GameUpdateCount / 65 + Position.Y) + 9) * 1.2f, (float)Main.GameUpdateCount / 50, (float)Main.GameUpdateCount / 63, (float)Main.GameUpdateCount / 300, true);

			Model model = GetModel("Realms/Models/altar");
			Vector3 dir = ModelHandler.LightingDirection(centerPos, out Vector3 averageColor, 4, 8, 10f);
			foreach (ModelMesh mesh in model.Meshes)//this can be condensed if the final model only has one mesh and one effect (at least only one you use)
				foreach (BasicNormalEffect effect in mesh.Effects)
				{
					effect.TextureMap.SetValue(ModContent.GetTexture("Realms/Models/altar_tex"));
					effect.NormalMap.SetValue(ModContent.GetTexture("Realms/Models/altar_normal"));

					effect.EyePosition.SetValue(new Vector3(ModelHandler.cameraPosition, 1));
					effect.LightDirection.SetValue(dir);

					effect.DiffuseIntensity.SetValue(1f);
					effect.AmbientIntensity.SetValue(1f);

					effect.SpecularColor.SetValue(new Vector4(averageColor, 1));
					effect.DiffuseColor.SetValue(new Vector4(averageColor / 2, 1));
					effect.AmbientColor.SetValue(new Vector4(averageColor / 2, 1));
				}


			model.DrawSplit((pos * 16), 0.0045f, (float)Main.GameUpdateCount / 200 + Position.Y, (float)Main.GameUpdateCount / 333 + Position.X, 0, true);

			//model.SetModTexture("Realms/Models/altar_tex");
			//model.LightingSetting(true, false);
			//Vector3 dir = ModelHandler.LightingDirection(pos, out Vector3 averageColor, 4, 8, 10f);
			//model.Specular(5, averageColor);
			//model.AmbientColor(averageColor / 2);
			//model.DirectionalLight0(true, dir, averageColor / 4, averageColor / 4);


			//model.Draw((pos * 16), 0.0045f, (float)Main.GameUpdateCount / 200, (float)Main.GameUpdateCount / 333, 0, true);

			//spriteBatch.Draw(Main.blackTileTexture, new Vector2(i, j) * 16 - Main.screenPosition, Color.Green);
			//model.IterateEffect(IterateMethod);
		}

        //private void IterateMethod(Effect effect)
        //      {
        //	(effect as BasicNormalEffect).AmbientColor.SetValue(new Vector4(Color.Red.ToVector3(), 1));
        //}

        public override void SaveData(TagCompound tag)/* tModPorter Suggestion: Edit tag parameter instead of returning new TagCompound */
        {
            return base.SaveData();
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);
        }

        protected override int ValidTileType => ModContent.TileType<RealmAltar>();
        protected override int TileSquareRange => 5;
    }
}
