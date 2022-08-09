using Terraria.ModLoader;
using SubworldLibrary;
using System;
using System.Collections.Generic;
using Terraria.Utilities;
using Terraria;
using System.Reflection;
using Terraria.ModLoader.IO;
using static Realms.SubworldHandler;
using Realms.RealmData;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Linq;
using Terraria.WorldBuilding;

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
                    if (!SLWorld.subworld)
                        _mainworldName = Main.ActiveWorldFileData.GetFileName(false);
                    else
                        throw new Exception("Main world name not found");//TODO: save the world name as a string in the world data of every subworld
                }

                return _mainworldName;
            } 
        }

        public static bool EnteringWorld;
        //public static string CurrentSubworldName;//filename
        public static string SubworldFolderPath => 
            Main.WorldPath + "\\Subworlds\\" + MainworldName + "\\Realms\\";//MainWorldName is always set before this is ever called
        /// <summary>
        /// Only call this is there is an active realm with a generated ID
        /// </summary>
        public static string CurrentSubworldPathName(RealmInfo info) => 
            SubworldFolderPath + info.DisplayName + "_" + info.RealmID;
        public static string CurrentSubworldPathWld(RealmInfo info) => 
            CurrentSubworldPathName(info) + ".wld";

        public static Texture2D DefaultWorldPreview => ModContent.GetTexture("Realms/UI/DefaultPreviewQuestion");

        public static Dictionary<string, Texture2D> WorldPreviews = new Dictionary<string, Texture2D>();//these may need to be cleared as need-be
        public static Texture2D GetWorldTexture(RealmInfo realmInfo = null)
        {
            bool mainworld = (realmInfo == null);

            if (mainworld || !string.IsNullOrEmpty(realmInfo.RealmID))//if mainworld or ID has been set
            {
                //mainworld entry is no long added by default since this would add it
                string dictID = mainworld ? "" : realmInfo.RealmID;//main world always uses an empty string for the id for the id

                if (WorldPreviews.ContainsKey(dictID))
                    return WorldPreviews[dictID];
                else
                {
                    string mainPath = SubworldFolderPath;
                    string filePath = mainworld ?
                        SubworldFolderPath + "MainWorld_Preview.png" :
                        CurrentSubworldPathName(realmInfo) + "_Preview.png";

                    Main.NewText("loading: " + filePath);//debug

                    if (!Directory.Exists(mainPath))//creates the directory if it doesn't exist
                        Directory.CreateDirectory(mainPath);

                    Texture2D texture;
                    if (File.Exists(filePath))//kept as a if statement for readablity
                        texture = Texture2D.FromStream(Main.graphics.GraphicsDevice, File.OpenRead(filePath));
                    else
                        texture = DefaultWorldPreview;

                    WorldPreviews.Add(dictID, texture);
                    return texture;
                }
            }
            else//if id is unset return default
                return DefaultWorldPreview;
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

    public class SubworldWorld : ModSystem
    {

        public override void PreUpdateWorld()
        {
            if (activeRealm != null)
                foreach (RealmEffect realmEffect in activeRealm.realmEffectList)
                    realmEffect.Update();
        }

        public override void OnWorldLoad()/* tModPorter Suggestion: Also override OnWorldUnload, and mirror your worldgen-sensitive data initialization in PreWorldGen */
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

                if (CurrentSubworldPathWld(activeRealm) != Main.ActiveWorldFileData.Path)
                    foreach (RealmFeature feature in activeRealm.realmFeatureList)
                        genlist.Add(new SubworldGenPass(feature.Generate));

                genlist.Add(new SubworldGenPass(FinishRealm));

                return genlist;
            } 
        }
        public void LoadRealm(GenerationProgress genMessage) =>
            LoadWorldFile.Invoke(ModContent.GetInstance<SLWorld>(), new object[] { CurrentSubworldPathWld(activeRealm), false });
        public void FinishRealm(GenerationProgress genMessage) =>
            Main.ActiveWorldFileData = new Terraria.IO.WorldFileData(CurrentSubworldPathWld(activeRealm), false);


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