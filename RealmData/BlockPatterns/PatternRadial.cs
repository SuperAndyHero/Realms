using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;

namespace Realms.RealmData.BlockPatterns
{
    public class PatternRadial : BlockPattern
    {
        public override int MaxTiles => int.MaxValue;
        public override int GetTypeIndex(int i, int j, int typeLength, Point16 center)
        {
            return (int)((Vector2.Distance(new Vector2(i, j), center.ToVector2()) / (200 / (float)frequency)) % (typeLength));
        }
    }
}
