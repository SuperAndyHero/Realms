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

namespace Realms.RealmData
{
    public enum RealmEffectType
    {
        Overlay = 1004,
        Filter = 2003,
        Potion = 3006,
        Spawning = 4024,
        Misc = 100032
    }

    public class RealmEffect
    {
        public WorldLocation location;
        public virtual RealmEffectType EffectType => RealmEffectType.Misc;

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
}