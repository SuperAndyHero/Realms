using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Realms.RealmData.RealmEffects
{
    class CampfireBuffEffect : PotionRealmEffect
    {
        public override int BuffType => BuffID.Campfire;
    }
}
