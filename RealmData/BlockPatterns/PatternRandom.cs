using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria;

namespace Realms.RealmData.BlockPatterns
{
    public class PatternRandom : BlockPattern
    {
        public override int MaxTiles => int.MaxValue;
        public override int GetTypeIndex(int i, int j, int typeLength, Point16 center) =>
            WorldGen.genRand.Next(typeLength);
    }
}
