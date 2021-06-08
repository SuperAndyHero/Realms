using System;
using Terraria;

namespace Realms.RealmData
{
    public static class RealmHelper
    {
        public static float WorldScaleChance(float precentChance) =>
            (int)((Main.maxTilesX * Main.maxTilesY) / (100 / precentChance));
    }
}
