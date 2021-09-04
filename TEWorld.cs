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

namespace Realms//SuperAndyHero's one file tile entity enhancer
{
    public class TEWorld : ModWorld
    {
        /// <summary>
        /// Allows TEs to draw: step one extend from interface, step two add draw method, step three enjoy
        /// </summary>
        public interface IDrawableTE
        {
            void Draw(SpriteBatch spriteBatch);//TEs have a vector2 position field, and it is in tile coordinates.
        }

        public static List<IDrawableTE> TEBuffer;

        public override void PreUpdate()
        {
            TEBuffer = new List<IDrawableTE>();

            foreach (KeyValuePair<Point16, TileEntity> item in TileEntity.ByPosition)
                if (item.Value is IDrawableTE)
                    TEBuffer.Add(item.Value as IDrawableTE);//cull offscreen?
        }

        public override void PostDrawTiles()
        {
            if (TEBuffer != null)
            {
                Main.spriteBatch.Begin(default, BlendState.AlphaBlend, SamplerState.PointClamp, default, default, default, Main.GameViewMatrix.TransformationMatrix);

                foreach (IDrawableTE drawable in TEBuffer)
                    drawable.Draw(Main.spriteBatch);

                Main.spriteBatch.End();
            }
        }
    }

    /// <summary>
    /// extending from this lets you create a TE with minimal effort.
    /// just set the valid tile type.
    /// </summary>
    public abstract class SimpleEntity : ModTileEntity
    {
        public override bool ValidTile(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            return tile.active() && tile.type == ValidTileType && tile.frameX == 0 && tile.frameY == 0;
        }

        protected abstract int ValidTileType { get; }

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
        /// square range not counting the middle.
        /// this may need to be changed if this is part of a multi-tile and said tile extends outside the 5x5 box around the TE.
        /// TE is in the top left.
        /// </summary>
        protected virtual int TileSquareRange => 2;//this may need to be changed if this is part of a multi-tile and said tile extends outside the 5x5 box around the TE (which is in the top left)
    }
}