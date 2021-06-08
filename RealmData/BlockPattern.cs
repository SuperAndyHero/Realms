using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;

namespace Realms.RealmData
{
    public class BlockPattern
    {
        public virtual int MaxTiles => 1;
        public RealmFrequency frequency = RealmFrequency.Normal;
        public int GetTypeIndex(int i, int j, int typeLength, Point16 center) => 0;
    }
}
