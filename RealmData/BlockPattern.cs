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
        public RealmFrequency frequency = RealmFrequency.Normal;

        /// <summary>
        /// optional
        /// </summary>
        /// <param name="Frequency"></param>
        /// <returns></returns>
        public BlockPattern Setup(RealmFrequency Frequency = RealmFrequency.Normal)
        {
            frequency = Frequency;
            return this;
        }

        public virtual int MaxTiles => 1;
        public int GetTypeIndex(int i, int j, int typeLength, Point16 center) => 0;
    }
}
