using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Realms.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Realms.UI
{
    #region containers
    public class EffectSlotContainer
    {
        public Ref<Item> effect = new Ref<Item>();
        public Ref<Item> location = new Ref<Item>();
    }

    public class FeatureSlotContainer
    {
        public Ref<Item> feature = new Ref<Item>();
        public Ref<Item> rarity = new Ref<Item>();
        public Ref<Item> size = new Ref<Item>();
        public Ref<Item> freq = new Ref<Item>();
        public Ref<Item> location = new Ref<Item>();
        public List<Ref<Item>> tiles = new List<Ref<Item>>();
        public PatternSlotContainer patternSlot = new PatternSlotContainer();
    }

    public class PatternSlotContainer
    {
        public Ref<Item> pattern = new Ref<Item>();
        public Ref<Item> freq = new Ref<Item>();
    }

    public class MiscSlotContainer
    {
        public Ref<Item> misc = new Ref<Item>();
    }

    public class RealmBookContainer
    {
        public Ref<Item> realmBook = new Ref<Item>();
    }
    #endregion

    public class CollapsiblePanel : GUI
    {
        const int collaspedHeight = 17;
        const int fullHeight = 124;
        const int moveOffset = 107;

        public List<CollapsiblePanel> panelList;
        private bool _expanded;
        public bool Expanded { get => _expanded; set {RebuildHeight(); _expanded = value;} }

        public int downOffset = 0;

        public new Vector2 RealPosition { get => ParentUI.RealPosition + Offset + Vector2.UnitY * downOffset; }


        public void RebuildHeight()
        {
            Size = new Vector2(Size.X, Expanded ? fullHeight : collaspedHeight);
            foreach (CollapsiblePanel panel in panelList)
            {
                downOffset = 0;
                if (panel == this)
                    break;
                else if (panel.Expanded == true)
                {
                    downOffset = moveOffset;
                    break;
                }
            }
        }

        public CollapsiblePanel(List<CollapsiblePanel> panels) => 
            panelList = panels;
    }

    public class RealmAltarUI : GUI
    {
        readonly AltarEntity altarEntity;

        readonly List<EffectSlotContainer> effectList = new List<EffectSlotContainer>();
        readonly List<FeatureSlotContainer> featureList = new List<FeatureSlotContainer>();
        readonly List<MiscSlotContainer> miscList = new List<MiscSlotContainer>();
        readonly RealmBookContainer realmBookContainer = new RealmBookContainer();

        public RealmAltarUI(AltarEntity altar)
        {
            altarEntity = altar;
            altarEntity.effectList = effectList;
            altarEntity.featureList = featureList;
            altarEntity.miscList = miscList;
            altarEntity.realmBookContainer = realmBookContainer;
        }

        const int mainUIWidth = 402;
        const int mainUIHeight = 332;

        const int effectSlotCount = 7;
        const int featureSlotCount = 9;
        const int miscSlotCount = 9;



        public override void OnCreate()
        {
            backTexture = ModContent.GetTexture("Realms/UI/RealmUI/MainBackground");
            Size = new Vector2(mainUIWidth, mainUIHeight);
            Offset = (new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f) - (new Vector2(mainUIWidth + 200, mainUIHeight) * 0.5f);

            AddElement(new CloseButton(this) { texture = ModContent.GetTexture("Realms/UI/RealmUI/Close"), Offset = new Vector2(Size.X - 21, 11), baseColor = Color.LightGray});
            AddElement(new DragBar(this) { Size = new Vector2(Size.X - 28, 16)});
            AddElement(new HoverText() { Size = new Vector2(Size.X - 28, 16), TextString = "Your balls. Hand em over." });
            Texture2D mainslotTex = ModContent.GetTexture("Realms/UI/RealmUI/MainSlot");

            #region debugbuttons
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
            #endregion


            GUI ItemSlotPanel = new GUI() { backTexture = mainslotTex, Size = mainslotTex.Size(), Offset = new Vector2(-112, 0) };
            AddElement(ItemSlotPanel);
            //ItemSlotPanel.AddElement(new DragBar(ItemSlotPanel));//debug and showing gui features
            ItemSlotPanel.AddElement(new ItemSlot()
            {
                ItemReference = realmBookContainer.realmBook,
                itemAllowed = IsRealmItem,
                Size = new Vector2(40, 40),
                Offset = new Vector2(32, 34),
                iconTexture = ModContent.GetTexture("Realms/UI/RealmUI/SlotIcon"),
                slotTexture = Main.blackTileTexture,
                slotColor = Color.Transparent,
                slotUnfocusedColor = Color.Transparent,
            });
        }

        public override void OnDeactivate()
        {
            //bookSlot.storedItem.
        }

        private bool IsRealmItem(Item item) => 
            item.IsAir || item.modItem is Items.RealmBook;

        private void EnterButtonClicked()
        {

        }

        private void CreateButtonClicked()
        {

        }
    }
}
