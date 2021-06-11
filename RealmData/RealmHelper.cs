using System;
using Terraria;

namespace Realms.RealmData
{
    public static class RealmHelper
    {
        public static float WorldScaleChance(float precentChance) =>
            10000f / ((Main.maxTilesX * Main.maxTilesY) / precentChance);
    }
}
    