using Terraria.World.Generation;
using System.Collections.Generic;
using Terraria.Utilities;
using Terraria;
using System.Reflection;
using Terraria.ModLoader.IO;
using static Realms.SubworldHandler;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;

namespace Realms.RealmData
{
    public enum RealmFeatureType
    {
        Main = 1,
        Biome = 2003,
        Ore = 3008,
        SurfaceObject = 4016,
        Blob = 5008,
        Vegetation = 6024,
        Structure = 7012,
        Misc = 8016,
    }

    public class RealmFeature
    {
        #region fields
        public int[] TileTypes;
        public WorldLocation Location;
        public BlockPattern Pattern;
        public RealmRarity Rarity;
        public RealmSize Size;
        public RealmFrequency Frequency;

        /// <summary>
        /// A center-point for patterns, set this if any patterns should center around a custom point
        /// </summary>
        protected Point16 center = Point16.Zero;
        /// <summary>
        /// (1 in this) chance that takes into account rarity. Is the input to ShouldPlace.
        /// </summary>
        private float placeChanceNormal = 0;
        //private float placeChanceScaled = 0;
        #endregion

        #region properies
        /// <summary>
        /// Should be used for block placing, takes into account both PrecentChance adjusted for RarityMult
        /// Not set if Generate is overridden
        /// </summary>
        protected bool ShouldPlaceNormal => WorldGen.genRand.NextFloat(placeChanceNormal) < 1f;
        /// <summary>
        /// Should be used for block placing, takes into account both PrecentChance adjusted for world size and RarityMult
        /// Not set if Generate is overridden
        /// </summary>
       // protected bool ShouldPlaceScaled => WorldGen.genRand.NextFloat(1) < placeChanceScaled;
        protected float RarityMult => (int)Rarity * 0.01f;
        protected float SizeMult => (int)Size * 0.01f;
        protected float FrequencyMult => (int)Frequency * 0.01f;
        /// <summary>
        /// Should be used for block type, this takes into account the tileList, the pattern, and if set the center
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        protected int TileType(int i, int j) =>
            TileTypes[Pattern.GetTypeIndex(i, j, TileTypes.Length, center)];
        #endregion

        #region virtual properties
        /// <summary>
        /// What type of feature is this, this is used for type caps and sorting
        /// </summary>
        public virtual RealmFeatureType FeatureType => RealmFeatureType.Misc;
        /// <summary>
        /// input chance, the output that takes rarity into account is 'PlaceChance'
        /// </summary>
        protected virtual float PrecentChance => 100f;
        /// <summary>
        /// Worldgen screen message
        /// </summary>
        protected virtual string GenerateMessage => "Filling World";
        #endregion

        public RealmFeature Setup(int[] tileTypes = null, WorldLocation location = null, BlockPattern pattern = null, RealmRarity rarity = RealmRarity.Normal, RealmSize size = RealmSize.Normal, RealmFrequency frequency = RealmFrequency.Normal)
        {
            TileTypes = tileTypes ?? new int[] { 0 };//save
            Pattern = pattern ?? new BlockPattern();//save
            Location = location ?? new WorldLocation();//save
            Rarity = rarity;//save
            Size = size;//save
            Frequency = frequency;//save
            placeChanceNormal = 100f / (PrecentChance * RarityMult);//deterministic
            return this;
        }

        public virtual void Generate(GenerationProgress progress)
        {
            //placeChanceScaled = RealmHelper.WorldScaleChance(PrecentChance * RarityMult);//deterministic

            progress.Message = GenerateMessage;
            center = Location.Center;

            Rectangle area = Location.CoverageArea;
            for (int i = area.X; i < area.Width; i++)
            {
                //progress.CurrentPassWeight = ((float)i / (float)area.Width);
                for (int j = area.Y; j < area.Height; j++)
                    if (Location.LocationValid(i, j))
                        IterateValidZone(i, j);
            }
        }

        /// <summary>
        /// called once per tile within WorldLocation bounds, and location returns true
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        protected virtual void IterateValidZone(int i, int j)
        {
            if(ShouldPlaceNormal)
                WorldGen.PlaceTile(i, j, TileType(i, j), true, true);
        }

        /// <summary>
        /// called after all other features finish generation
        /// </summary>
        /// <param name="progress"></param>
        public virtual void PostGenerate(GenerationProgress progress)//second pass
        {

        }

        /// <summary>
        /// for sorting this feature after others, or moving/removing conflicting features
        /// called when the parent realm info is first compiled
        /// </summary>
        /// <param name="featureList"></param>
        public virtual void ModifyList(List<RealmFeature> featureList)
        {

        }
    }
}