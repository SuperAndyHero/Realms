using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Reflection.Emit;
using Terraria.GameContent.Generation;
using Terraria.World.Generation;
using SubworldLibrary;
using Terraria.Graphics;
using Realms;
using Terraria.ModLoader.IO;
using Realms.Effects;
using Realms;

namespace Realms
{
    public class TEWorld : ModWorld
    {
        public static List<Realms.IDrawable> TEBuffer;

        public override void PreUpdate()
        {
            TEBuffer = new List<Realms.IDrawable>();

            foreach (KeyValuePair<Point16, TileEntity> item in TileEntity.ByPosition)
                if (item.Value is IDrawable)
                    TEBuffer.Add(item.Value as Realms.IDrawable);//cull offscreen?
        }

        public override void PostDrawTiles()
        {
            if (TEBuffer != null)
            {
                Main.spriteBatch.Begin(default, BlendState.AlphaBlend, SamplerState.PointClamp, default, default, default, Main.GameViewMatrix.TransformationMatrix);

                foreach (Realms.IDrawable drawable in TEBuffer)
                    drawable.Draw(Main.spriteBatch);

                Main.spriteBatch.End();
            }
        }
    }

    public abstract class SimpleEntity : ModTileEntity
    {
        public override bool ValidTile(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            return tile.active() && tile.type == ValidType && tile.frameX == 0 && tile.frameY == 0;
        }

        protected abstract int ValidType { get; }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(Main.myPlayer, i, j, TileSquareRange);
                NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i, j, Type, 0f, 0, 0, 0);
                return -1;
            }
            return Place(i, j);
        }

        /// <summary>
        /// square range not counting the middle
        /// </summary>
        protected virtual int TileSquareRange => 2;
    }
}