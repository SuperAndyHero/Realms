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
using System.Text;
using System.IO;

namespace Realms.RealmData
{
    public class RealmInfo
    {
        public int Width;
        public int Height;
        public string DisplayName;
        public int Seed;
        public string RealmID;

        public List<RealmEffect> realmEffectList;
        public List<RealmFeature> realmFeatureList;

        public RealmInfo(int width, int height, string displayName, List<RealmEffect> effectList, List<RealmFeature> featureList, int seed = 0, string Id = "")
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

            realmEffectList = effectList;
            realmFeatureList = featureList;

            RealmID = Id;
            Main.NewText("New realm info initalized " + DisplayName);
        }

        public void CreateId()
        {
            //byte[] idComination = Encoding.ASCII.GetBytes(DateTime.Now.ToString());
            RealmID = DateTime.Now.ToString().GetHashCode().ToString(); //Convert.ToBase64String(idComination);//fix always gives the same
            Main.NewText("Id Created " + RealmID);
        }

        public static TagCompound Save(RealmInfo realmInfo)
        {
            List<TagCompound> effectList = new List<TagCompound>();
            foreach(RealmEffect effect in realmInfo.realmEffectList)
            {
                TagCompound effectTagCompound = new TagCompound()
                {
                    {"effectType", effect.GetType().FullName},
                    {"locationType", effect.Location.GetType().FullName}
                };
                effectList.Add(effectTagCompound);
            }

            List<TagCompound> featureList = new List<TagCompound>();
            foreach(RealmFeature feature in realmInfo.realmFeatureList)
            {
                TagCompound featureTagCompound = new TagCompound()
                {
                    {"featureType", feature.GetType().FullName},
                    {"tileTypes", feature.TileTypes},
                    {"locationType", feature.Location.GetType().FullName},
                    {"patternType", feature.Pattern.GetType().FullName},
                    {"patternFreq", (int)feature.Pattern.frequency },
                    {"rarity", (int)feature.Rarity},
                    {"size", (int)feature.Size},
                    {"freq", (int)feature.Frequency}
                };
                featureList.Add(featureTagCompound);
            }

            TagCompound compound = new TagCompound
            {
                {"width", realmInfo.Width },
                {"height", realmInfo.Height },
                {"name", realmInfo.DisplayName },
                {"seed", realmInfo.Seed },
                {"Id", realmInfo.RealmID },
                {"effects", effectList},
                {"features", featureList }
            };

            return new TagCompound(){{"realmInfo", compound}};
        }

        public static RealmInfo Load(TagCompound tagCompound)
        {
            TagCompound compound = tagCompound.GetCompound("realmInfo");


            RealmInfo realmInfo = new RealmInfo(
                compound.GetInt("width"),
                compound.GetInt("height"),
                compound.GetString("name"),
                new List<RealmEffect>(),
                new List<RealmFeature>(),
                compound.GetInt("seed"),
                compound.GetString("Id")
                );

            List<TagCompound> effectList = (List<TagCompound>)compound.GetList<TagCompound>("effects");
            foreach(TagCompound effectTagCompound in effectList)
            {
                RealmEffect effect = (RealmEffect)Activator.CreateInstance(
                    Type.GetType(effectTagCompound.GetString("effectType")));

                effect.Setup((WorldLocation)Activator.CreateInstance(
                     Type.GetType(effectTagCompound.GetString("locationType"))));

                realmInfo.realmEffectList.Add(effect);
            }


            List<TagCompound> featureList = (List<TagCompound>)compound.GetList<TagCompound>("features");
            foreach (TagCompound featureTagCompound in featureList)
            {
                RealmFeature feature = (RealmFeature)Activator.CreateInstance(
                     Type.GetType(featureTagCompound.GetString("featureType")));

                feature.Setup(
                    featureTagCompound.GetIntArray("tileTypes"),
                    (WorldLocation)Activator.CreateInstance(
                         Type.GetType(featureTagCompound.GetString("locationType"))),
                    ((BlockPattern)Activator.CreateInstance(
                         Type.GetType(featureTagCompound.GetString("patternType"))))
                            .Setup((RealmFrequency)featureTagCompound.GetInt("patternFreq")),
                    (RealmRarity)featureTagCompound.GetInt("rarity"),
                    (RealmSize)featureTagCompound.GetInt("size"),
                    (RealmFrequency)featureTagCompound.GetInt("freq")
                    );

                realmInfo.realmFeatureList.Add(feature);
            }

            return realmInfo;
        }
    }
}