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

namespace Realms
{
    public static class SubworldHandler 
    {
        public static MethodInfo[] asd = typeof(SLWorld).GetMethods(BindingFlags.NonPublic | BindingFlags.Static);
        public static MethodInfo LoadWorldFile = typeof(SLWorld).GetMethod("LoadWorldFile", BindingFlags.NonPublic | BindingFlags.Static);
        public static string MainworldName;
        public static bool enteringWorld;
        //public static string CurrentSubworldName;//filename
        public static string SubworldFolderPath => Main.WorldPath + "\\Subworlds\\" + MainworldName + "\\Realms\\";
        /// <summary>
        /// Only call this is there is an active realm with a generated ID
        /// </summary>
        public static string CurrentSubworldPath => SubworldFolderPath + activeRealm.DisplayName + "_" + activeRealm.RealmID + ".wld";

        public static RealmInfo activeRealm;

        public static void Enter(RealmInfo enterRealmInfo)
        {
            if (!SLWorld.subworld)
            {
                MainworldName = Main.ActiveWorldFileData.GetFileName(false);
            }
            activeRealm = enterRealmInfo;
            if (string.IsNullOrEmpty(activeRealm.RealmID))
                activeRealm.CreateId();
            //Main.NewText("Entered subworld");
            Subworld.Enter<Realm>();
            enteringWorld = false;
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
    }

    public class Realm : Subworld
    {
        public override bool saveSubworld
        {
            get
            {
                if (DebugNoSave)
                    return false;
                enteringWorld = !enteringWorld && MainworldName != Main.ActiveWorldFileData.GetFileName(false);
                return enteringWorld;
            }
        }
        public override bool saveModData => true;

        public override bool noWorldUpdate => false;
        public override int width => activeRealm.Width;// WorldWidth;
        public override int height => activeRealm.Height;// WorldHeight;

        public override List<GenPass> tasks => new List<GenPass>() { new SubworldGenPass(GenMethod) };//do gen here, and run the loading code when it grabs it and then check before returning list

        public void GenMethod(GenerationProgress genMessage)//todo see above
        {
            LoadWorldFile.Invoke(ModContent.GetInstance<SLWorld>(), new object[] { (CurrentSubworldPath), false });
            if(CurrentSubworldPath != Main.ActiveWorldFileData.Path)
            {
                foreach(RealmFeature feature in activeRealm.realmFeatureList)
                {
                    feature.Generate(genMessage);
                }
            }
            Main.ActiveWorldFileData = new Terraria.IO.WorldFileData(CurrentSubworldPath, false);
        }

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