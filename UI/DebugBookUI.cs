using Microsoft.Xna.Framework;
using Realms.Items;
using Realms.RealmData;
using Realms.RealmData.BlockPatterns;
using Realms.RealmData.RealmEffects;
using Realms.RealmData.RealmFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace Realms.UI
{
    public class DebugBookUI : GUI
    {
        public RealmBook realmItem;

        public override void OnCreate()
        {
            Size = Vector2.One * 200;
            focusedColor = Color.SlateBlue;
            unfocusedColor = Color.DarkSlateBlue;

            AddElement(new CloseButton(this));
            AddElement(new DragBar(this));
            AddElement(new Button()
            {
                Offset = new Vector2(10, 160),
                Size = new Vector2(100, 25),
                onClick = EnterButtonClicked,
            });
            AddElement(new UIText()
            {
                Offset = new Vector2(10, 162),
                TextString = "Enter Realm"
            });

            AddElement(new Button()
            {
                Offset = new Vector2(138, 160),
                Size = new Vector2(53, 25),
                onClick = CreateButtonClicked,
            });
            AddElement(new UIText()
            {
                Offset = new Vector2(138, 162),
                TextString = "Create"
            });
        }

        private void EnterButtonClicked()
        {
            if(realmItem.realmInfo == null)
                Main.NewText("No Realm Data");
            else
                SubworldHandler.Enter(realmItem.realmInfo);
        }

        private void CreateButtonClicked()
        {
                realmItem.realmInfo = new RealmInfo(400, 400, "TestWorldsD35",
                    new List<RealmEffect> { new CampfireBuffEffect().Setup() },
                    new List<RealmFeature> { new Spheres().Setup(
                        new int[] { TileID.WoodBlock, TileID.SilverBrick, TileID.RubyGemspark, TileID.Pearlstone }, 
                        null, 
                        new PatternSpiral().Setup(RealmFrequency.Fast), 
                        RealmRarity.Common,
                        RealmSize.VeryBig
                        )});
        }
    }
}
