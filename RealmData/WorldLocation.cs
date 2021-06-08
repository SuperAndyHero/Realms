using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;

namespace Realms.RealmData
{
    public class WorldLocation
    {

        public virtual Rectangle CoverageArea => new Rectangle(0, 0, Main.maxTilesX, Main.maxTilesY);
        public virtual Point16 Center => CoverageArea.Center().ToPoint16();

        public virtual bool LocationValid(int i, int j) => true;
    }
}
