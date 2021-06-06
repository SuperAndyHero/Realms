using Terraria.ModLoader;
using SubworldLibrary;
using System;
using Terraria.World.Generation;
using System.Collections.Generic;
using Terraria.Utilities;
using Terraria;
using System.Reflection;
using Terraria.ModLoader.IO;
using static Realms.SubworldHandler;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;

namespace Realms
{
    public class RealmInfo
    {
        int Width;
        int Height;
        string DisplayName;
        int Seed;

        public RealmInfo(int width, int height, string displayName, int seed = 0)
        {
            Width = width;
            Height = height;
            DisplayName = displayName;
            if (seed == 0)
            {
                Main.rand = new UnifiedRandom((int)DateTime.Now.Ticks);
                Seed = Main.rand.Next();
            }
            else
                Seed = seed;
        }

        public List<RealmEffect> realmEffects;
        public List<RealmFeature> realmFeatures;

    }

    public class WorldLocation
    {
        public virtual bool LocationValid(int i, int j) => true;
    }

    public enum RealmEffectType
    {
        Overlay = 1004,
        Filter = 2003,
        Potion = 3006,
        Spawning = 4024,
        Misc = 100032
    }

    public enum RealmFeatureType
    {
        Main = 1,
        Biome = 2003,
        Ore = 3008,
        Surface = 4016,
        Blob = 5008,
        Vegetation = 6024,
        Structure = 7012,
        Misc = 8016,
    }

    public abstract class RealmEffect
    {
        public WorldLocation location;
        public RealmEffectType effectType;

        public RealmEffect(WorldLocation Location = null)
        {
            location = Location ?? new WorldLocation();
        }

        public virtual void Update()
        {
            if (location.LocationValid((int)Main.LocalPlayer.position.X / 16, (int)Main.LocalPlayer.position.Y / 16))
                UpdateInRange();
        }

        /// <summary>
        /// only runs if player location is valid, is not called is Update() is overridden
        /// </summary>
        public virtual void UpdateInRange()
        {

        }

        /// <summary>
        /// precent chance that a random update will occur
        /// </summary>
        public virtual float RandomUpdateChance => 0;
        /// <summary>
        /// precent coverage of a random update
        /// </summary>
        public virtual float RandomUpdateCoverage => 5;

        public virtual void RandomUpdate(int i, int j)
        {
            if (location.LocationValid(i, j))
                RandomUpdateInRange(i, j);
        }

        /// <summary>
        /// called if the chosen location is in range, is not called is RandomUppdate is overridden
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        public virtual void RandomUpdateInRange(int i, int j)
        {

        }

        /// <summary>
        /// for drawing on the screen
        /// </summary>
        /// <param name="spriteBatch"></param>
        public virtual void PostDraw(SpriteBatch spriteBatch)
        {

        }

        public virtual void ModifyList(List<RealmEffect> effectList)
        {

        }
    }

    public enum PatternFreq
    {
        VerySlow = 1,
        Slow = 5,
        Normal = 10,
        Fast = 20,
        VeryFast = 100
    }

    public class BlockPattern
    {
        public virtual int maxTiles => 1;
        public PatternFreq frequency = PatternFreq.Normal;
        public int GetTypeIndex(int i, int j, int typeLength, Point16 center) => 0;
    }

    public abstract class RealmFeature
    {
        public int[] TileTypes;
        public readonly BlockPattern Pattern;
        public WorldLocation location;

        protected Point16 center = Point16.Zero;
        protected int TileType(int i, int j) => 
            TileTypes[Pattern.GetTypeIndex(i, j, TileTypes.Length, center)];

        public RealmFeature(int[] tileTypes = null, BlockPattern pattern = null)
        {
            TileTypes = tileTypes ?? new int[] { 0 };
            Pattern = pattern ?? new BlockPattern();
        }

        public virtual void Generate(GenerationProgress progress)
        {

        }

        public virtual void PostGenerate(GenerationProgress progress)//second pass
        {

        }

        public virtual void ModifyList(List<RealmFeature> featureList)
        {

        }
    }
}