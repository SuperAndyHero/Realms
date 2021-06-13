using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;
using static Realms.ContentHandler;

namespace Realms.Tiles
{
	public class RealmAltar : ModTile
	{
		public override void SetDefaults()
		{
			Main.tileSolid[Type] = false;
			Main.tileMergeDirt[Type] = false;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			//Main.tileLighted[Type] = true;

			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Realm Altar");
			AddMapEntry(new Color(200, 200, 200), name);
			dustType = DustID.Gold;
			drop = ModContent.ItemType<Items.RealmAltarItem>();


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
			minPick = 200;
		}

		public override bool NewRightClick(int i, int j)
		{
			Tile tile = Main.tile[i, j];
			int left = i - tile.frameX / 18;
			int top = j - tile.frameY / 18;

			int index = ModContent.GetInstance<AltarEntity>().Find(left, top);
			if (index == -1)
				return false;

			AltarEntity entity = (AltarEntity)TileEntity.ByID[index];
			//foreach (var item in tEScoreBoard.scores)
			//{
			//	Main.NewText(item.Key + ": " + item.Value);
			//}
			return true;
		}

		public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref Color drawColor, ref int nextSpecialDrawIndex)
        {
            base.DrawEffects(i, j, spriteBatch, ref drawColor, ref nextSpecialDrawIndex);
        }

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            base.SpecialDraw(i, j, spriteBatch);
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            return base.PreDraw(i, j, spriteBatch);
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
			Tile tile = Main.tile[i, j];
			if (tile.frameX == 0 && tile.frameY == 0)
            {
				//Model model = GetModel("Realms/Models/altar_smooth");
				//model.SetModTexture("Realms/Models/altar");
				//model.LightingSetting(true, true, true);
				//model.Specular(10);
				//model.SetAlpha(0.5f);

				//Vector3 dir = ModelHandler.LightingDirection(Main.LocalPlayer.Center, out Vector3 averageColor, 72, 8, 0.5f);
				//model.AmbientColor(averageColor * 0.25f);
				//model.DirectionalLight0(true, dir, averageColor / 4, averageColor);
				//model.Draw((new Vector2(i, j) * 16), 1f, (float)Main.GameUpdateCount / 100, 0, (float)Main.GameUpdateCount / 183, false);
				//model.DrawBoundingBox(spriteBatch, Main.LocalPlayer.Center + new Vector2(0, Main.LocalPlayer.gfxOffY), Color.Purple * 0.25f, 50);
			}
		}

		public override void NumDust(int i, int j, bool fail, ref int num) => num = 1;
		public override bool Drop(int i, int j) => false;
		public override void KillMultiTile(int i, int j, int frameX, int frameY) => Item.NewItem(new Vector2(i, j) * 16, new Vector2(5, 3) * 16, drop);//change here if size change

    }



	public class AltarEntity : SimpleEntity, IDrawable
	{
		public void Draw(int i, int j, SpriteBatch spriteBatch)
        {
			Vector2 pos = new Vector2(i + 3, j - 3);
			Model model = GetModel("Realms/Models/altar_smooth");
			model.SetModTexture("Realms/Models/altar");
			model.LightingSetting(true, false);
			Vector3 dir = ModelHandler.LightingDirection(pos, out Vector3 averageColor, 4, 8, 10f);
			model.Specular(5, averageColor);
			model.AmbientColor(averageColor / 2);
			model.DirectionalLight0(true, dir, averageColor / 4, averageColor / 4);
			model.Draw((pos * 16), 0.45f, (float)Main.GameUpdateCount / 30, (float)Main.GameUpdateCount / 42, 0, true);
			spriteBatch.Draw(Main.blackTileTexture, new Vector2(i, j) * 16 - Main.screenPosition, Color.Green);
		}

		protected override int ValidType => ModContent.TileType<RealmAltar>();
        protected override int TileSquareRange => 5;
    }
}
