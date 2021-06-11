using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

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
			Main.tileLighted[Type] = true;

			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Realm Altar");
			AddMapEntry(new Color(200, 200, 200), name);
			dustType = DustID.Gold;
			//drop = ModContent.ItemType<Items.Placeable.ExampleSand>();


			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);


			TileObjectData.newTile.DrawYOffset = 2;

			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 18 };

			TileObjectData.newTile.Width = 5;
			TileObjectData.newTile.Height = 3;

			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.addTile(Type);

			//drop = todo;
			minPick = 200;
		}

		public override void NumDust(int i, int j, bool fail, ref int num) => num = 1;
		public override bool Drop(int i, int j) => false;
		public override void KillMultiTile(int i, int j, int frameX, int frameY) => Item.NewItem(new Vector2(i, j) * 16, new Vector2(5, 3) * 16, todo);//change here if size change
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            base.ModifyLight(i, j, ref r, ref g, ref b);
        }

    }
}
