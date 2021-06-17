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

namespace Realms
{
    public class TEWorld : ModWorld
    {
        private List<(Point16, IDrawable)> drawbuffer;//tuple

        public override void PreUpdate()
        {
            drawbuffer = new List<(Point16, IDrawable)>();

            foreach (KeyValuePair<Point16, TileEntity> item in TileEntity.ByPosition)
                if (item.Value is IDrawable)
                    drawbuffer.Add((item.Key, item.Value as IDrawable));//cull offscreen?
        }

        public override void PostUpdate()
        {
            ModelHandler.Update();
        }

        public override void PostDrawTiles()
        {
            if (drawbuffer != null)
            {
                Main.spriteBatch.Begin(default, BlendState.AlphaBlend, SamplerState.PointClamp, default, default, default, Main.GameViewMatrix.TransformationMatrix);

                foreach ((Point16, IDrawable) drawable in drawbuffer)
                    drawable.Item2.Draw(drawable.Item1.X, drawable.Item1.Y, Main.spriteBatch);

                Main.spriteBatch.End();
            }
        }
    }

    interface IDrawable
    {
        void Draw(int i, int j, SpriteBatch spriteBatch);
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