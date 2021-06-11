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
        public WorldLocation Location;
        public virtual RealmEffectType EffectType => RealmEffectType.Misc;

        public RealmEffect Setup(WorldLocation location = null)
        {
            Location = location ?? new WorldLocation();
            return this;
        }

        public virtual void Update()
        {
            if (Location.LocationValid((int)Main.LocalPlayer.position.X / 16, (int)Main.LocalPlayer.position.Y / 16))
                PlayerUpdateInRange();
        }

        /// <summary>
        /// only runs if world location is valid, is not called if Update() is overridden
        /// </summary>
        protected virtual void PlayerUpdateInRange()
        {

        }

        /// <summary>
        /// precent chance that a random update will occur
        /// </summary>
        protected virtual float RandomUpdateChance => 0;
        /// <summary>
        /// precent coverage of a random update
        /// </summary>
        protected virtual float RandomUpdateCoverage => 5;

        public virtual void RandomUpdate(int i, int j)
        {
            if (Location.LocationValid(i, j))
                RandomUpdateInRange(i, j);
        }

        /// <summary>
        /// called if the chosen location is in range, is not called is RandomUppdate is overridden
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        protected virtual void RandomUpdateInRange(int i, int j)
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

    public abstract class PotionRealmEffect : RealmEffect
    {
        public override RealmEffectType EffectType => RealmEffectType.Potion;
        public virtual int BuffType => 0;
        protected override void PlayerUpdateInRange()
        {
            Main.LocalPlayer.AddBuff(BuffType, 1, true);
        }
    }
}