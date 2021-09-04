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
    public class PatternSpiral : BlockPattern
    {
        public override int MaxTiles => int.MaxValue;
        public override int GetTypeIndex(int i, int j, int typeLength, Point16 center)
        {
            float off = 0.5f + ((float)frequency * 0.01f);
            Vector2 pos = new Vector2(i, j);
            Vector2 cent = center.ToVector2();
            float a = (((Vector2.Distance(pos, cent) * off) + ((pos - cent).ToRotation() * typeLength)) / (float)Math.PI / 2);
            return (int)(a - typeLength * Math.Floor(a / typeLength));
        }
    }
}
