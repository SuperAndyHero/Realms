using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Realms.RealmData
{
    public enum RealmFrequency
    {
        VerySlow = 1,
        Slower = 10,
        Slow = 50,
        Normal = 100,
        Fast = 200,
        Faster = 400,
        VeryFast = 1000
    }


    public enum RealmRarity
    {
        BarelyAny = 1,
        VeryRare = 10,
        Uncommon = 50,
        Normal = 100,
        Common = 200,
        VeryCommon = 400,
        Abundant = 1000
    }

    public enum RealmSize
    {
        Tiny = 10,
        VerySmall = 25,
        Small = 50,
        Normal = 100,
        Big = 150,
        VeryBig = 200,
        Giant = 300
    }
}
