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
using Realms.RealmData;
using Microsoft.Xna.Framework.Graphics;

namespace Realms
{
    public static class SubworldHandler 
    {
        //public static MethodInfo[] asd = typeof(SLWorld).GetMethods(BindingFlags.NonPublic | BindingFlags.Static);
        public static MethodInfo LoadWorldFile = typeof(SLWorld).GetMethod("LoadWorldFile", BindingFlags.NonPublic | BindingFlags.Static);
        private static string _mainworldName;
        public static string MainworldName { 
            get
            {
                if (_mainworldName == null)
                {
                    _mainworldName = 
                }

                return _mainworldName;
            } 
        }
        public static bool EnteringWorld;
        //public static string CurrentSubworldName;//filename
        public static string SubworldFolderPath => Main.WorldPath + "\\Subworlds\\" + MainworldName + "\\Realms\\";//MainWorldName is always set before this is ever called
        /// <summary>
        /// Only call this is there is an active realm with a generated ID
        /// </summary>
        public static string CurrentSubworldPathName => SubworldFolderPath + activeRealm.DisplayName + "_" + activeRealm.RealmID;
        public static string CurrentSubworldPathFull => CurrentSubworldPathName + ".wld";

        public static Dictionary<RealmInfo, Texture2D> WorldPreviews = new Dictionary<RealmInfo, Texture2D>() { {null, ModContent.GetTexture("Realms/UI/DefaultPreviewQuestion")} };//these may need to be cleared as need-be
        public static Texture2D LoadWorldTexture(RealmInfo realmInfo = null) 
        {
            if (WorldPreviews.ContainsKey(realmInfo))
                return WorldPreviews[realmInfo];
            else
            {
                string path;
                if (!SLWorld.subworld)
                    path = Main.WorldPath + "\\Subworlds\\" + (MainworldName == null ? Main.ActiveWorldFileData.GetFileName(false) : MainworldName) + "Preview";
                else
                    path = SubworldFolderPath

            }
        }

        public static RealmInfo activeRealm;

        public static void Enter(RealmInfo enterRealmInfo)
        {
            //if (!SLWorld.subworld)
            //{
            //    MainworldName = Main.ActiveWorldFileData.GetFileName(false);
            //}
            activeRealm = enterRealmInfo;
            if (string.IsNullOrEmpty(activeRealm.RealmID))
                activeRealm.CreateId();
            //Main.NewText("Entered subworld");
            Subworld.Enter<Realm>();
            EnteringWorld = false;
        }


        public static bool DebugNoSave = false;
    }

    public class SubworldWorld : ModWorld
    {
        public override void PreUpdate()
        {
            if(activeRealm != null)
                foreach (RealmEffect realmEffect in activeRealm.realmEffectList)
                    realmEffect.Update();
        }

        public override void Initialize()
        {
            UIHandler.ActiveUIList.Clear();
        }
    }

    public class Realm : Subworld
    {
        public override bool saveSubworld
        {
            get
            {
                if (DebugNoSave)
                    return false;
                EnteringWorld = !EnteringWorld && MainworldName != Main.ActiveWorldFileData.GetFileName(false);
                return EnteringWorld;
            }
        }
        public override bool saveModData => true;

        public override bool noWorldUpdate => false;
        public override int width => activeRealm.Width;// WorldWidth;
        public override int height => activeRealm.Height;// WorldHeight;

        public override List<GenPass> tasks => GenerationList;
        public List<GenPass> GenerationList { 
            get {

                List<GenPass> genlist = new List<GenPass>();

                genlist.Add(new SubworldGenPass(LoadRealm));

                if (CurrentSubworldPathFull != Main.ActiveWorldFileData.Path)
                    foreach (RealmFeature feature in activeRealm.realmFeatureList)
                        genlist.Add(new SubworldGenPass(feature.Generate));

                genlist.Add(new SubworldGenPass(FinishRealm));

                return genlist;
            } 
        }
        public void LoadRealm(GenerationProgress genMessage) =>
            LoadWorldFile.Invoke(ModContent.GetInstance<SLWorld>(), new object[] { (CurrentSubworldPathFull), false });
        public void FinishRealm(GenerationProgress genMessage) =>
            Main.ActiveWorldFileData = new Terraria.IO.WorldFileData(CurrentSubworldPathFull, false);


        //public override void OnVotedFor() => StartTeleportSequence();
        public override void Load()
        {
            SLWorld.drawUnderworldBackground = true;
            SLWorld.noReturn = false;
            Main.rand = new UnifiedRandom((int)DateTime.Now.Ticks);
        }
        public override void Unload()
        {
            //saving
            //if (SLWorld.subworld)
            //    Main.ActiveWorldFileData = new Terraria.IO.WorldFileData(CurrentSubworldPath, false);
            //enteringWorld = false;
        }
    }
}