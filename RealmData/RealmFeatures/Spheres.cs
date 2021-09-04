using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.World.Generation;

namespace Realms.RealmData.RealmFeatures
{
    public class Spheres : RealmFeature
    {
        public override RealmFeatureType FeatureType => RealmFeatureType.Blob;
        protected override float PrecentChance => 0.01f;

        const int DefaultRadius = 10;
        protected override string GenerateMessage => "Generating Spheres";
        protected override void IterateValidZone(int i, int j)
        {
            if (ShouldPlaceNormal)//uses rarity for chance to spawn
            {
                int radius = (int)((DefaultRadius + WorldGen.genRand.Next((int)FrequencyMult)) * SizeMult);//(default + rand(Freq)) * size
                center = new Point16(i, j);//sets pattern center to center of sphere

                for(int x = -radius; x < radius; x++)//basic circle gen
                    for (int y = -radius; y < radius; y++)
                        if(Vector2.Distance(new Vector2(x, y), Vector2.Zero) <= radius)//in circle
                            if(WorldGen.InWorld(i + x, j + y))
                                WorldGen.PlaceTile(i + x, j + y, PatternTileType(i + x, j + y), true, true);
            }
        }
    }
}
