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

namespace Realms
{
    public static class SubworldHandler
    {
        public static MethodInfo LoadWorldFile = typeof(SLWorld).GetMethod("LoadWorldFile", BindingFlags.NonPublic | BindingFlags.Static);

        public static string MainworldName;
        public static string CurrentSubworldName;//filename
        public static string SubworldFolderPath => Main.WorldPath + "\\Subworlds\\" + MainworldName + "\\Realms\\";
        public static string CurrentSubworldPath => SubworldFolderPath + CurrentSubworldName;

        public static int WorldWidth;
        public static int WorldHeight;
        public static int WorldSeed;
    }

    public class RealmsWorld : ModWorld
    {
        public override void Initialize()
        {
            if (!Subworld.AnyActive<Realms>())//SLworld.subworld (bool)?//or store it before enter is called
                MainworldName = Main.ActiveWorldFileData.GetFileName(false);
            int a = 1;
        }

        public override void Load(TagCompound tag)
        {
            int b = 2;
            
        }
    }

    public class RealmSubworld : Subworld
    {
        public override bool saveSubworld
        {
            get
            {
                if (AnyActive<Realms>())
                    return true;
                else
                    return false;
            }
        }
        public override bool saveModData => true;

        public override bool noWorldUpdate => false;
        public override int width => 500;// WorldWidth;
        public override int height => 500;// WorldHeight;

        public override List<GenPass> tasks => new List<GenPass>() { new SubworldGenPass(GenMethod) };//do get gere, and run the loading code when it grabs it and then check before returning list

        public void GenMethod(GenerationProgress gen)//todo see above
        {
            LoadWorldFile.Invoke(ModContent.GetInstance<SLWorld>(), new object[] { (CurrentSubworldPath), false });
            if(SubworldHandler.CurrentSubworldPath == Main.ActiveWorldFileData.Path)
            {

            }
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
            Main.ActiveWorldFileData = new Terraria.IO.WorldFileData(SubworldHandler.CurrentSubworldPath, false);
        }
    }
}