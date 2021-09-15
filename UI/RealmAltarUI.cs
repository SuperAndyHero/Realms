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
    public class AltarItemData
    {
        public readonly List<EffectSlotContainer> effectList = new List<EffectSlotContainer>();
        public readonly List<FeatureSlotContainer> featureList = new List<FeatureSlotContainer>();
        public readonly List<MiscSlotContainer> miscList = new List<MiscSlotContainer>();
        public readonly Ref<Item> realmBook = new Ref<Item>();
    }

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
    #endregion

    public class ExpandButton : Button
    {
        public bool boolValue = false;

        public override void Draw(SpriteBatch spriteBatch)
        {
            bool hasFocus = ParentUI.HasFocus();
            if (this.MouseOver() && hasFocus)
                spriteBatch.Draw(highlightedTexture, RealPosition + highlightedTexture.Size() * 0.5f, null, highlightedColor, boolValue ? -1.57f : 0, highlightedTexture.Size() * 0.5f, 1f, default, default);
            else
                spriteBatch.Draw(texture, RealPosition + texture.Size() * 0.5f, null, hasFocus ? baseColor : unfocusedColor, boolValue ? -1.57f : 0, texture.Size() * 0.5f, 1f, default, default);
        }

        public override bool CheckHasInteracted(bool ClickReceived)
        {
            if (this.MouseOver() && ClickReceived)
            {
                boolValue = true;//boolValue = !boolValue; //old version allows for them all to be collasped
                onClick?.Invoke();
                return true;
            }
            return false;
        }
    }

    public class MiscPanel : GUI
    {
        readonly AltarItemData itemdata;
        public List<MiscSlotRow> rowList = new List<MiscSlotRow>();
        public MiscPanel(AltarItemData data)
        {
            itemdata = data;
            AddElement(new MiscSlotRow(itemdata, rowList) { Offset = new Vector2(0, 60) });
            AddElement(new MiscSlotRow(itemdata, rowList) { Offset = new Vector2(0, 60) });
        }
    }

    public class MiscSlotRow : GUI
    {
        const int miscSlotCount = 9;

        readonly AltarItemData itemdata;
        public List<MiscSlotRow> rowList;

        public MiscSlotRow(AltarItemData data, List<MiscSlotRow> list)
        {
            itemdata = data;
            rowList = list;

            int listOffset = rowList.Count * miscSlotCount;

            if (!rowList.Contains(this))
                rowList.Add(this);

            if(itemdata.miscList.Count > rowList.Count * miscSlotCount)
            {
                int missingCount = (rowList.Count * miscSlotCount) - itemdata.miscList.Count;
                for (int i = 0; i < missingCount; i++)
                {
                    itemdata.miscList.Add(new MiscSlotContainer());
                }
            }

            Vector2 allOffset = new Vector2(10, 10);

            for (int i = 0; i < miscSlotCount; i++)
            {
                AddElement(new ItemSlot() { Offset = new Vector2(32 * i, 0) + allOffset, Size = new Vector2(24, 24), 
                    ItemReference = itemdata.miscList[i + listOffset].misc });
            }
        }
    }

    public class CollapsiblePanel : GUI
    {
        public const int width = 362;
        public const int collaspedHeight = 34;
        const int fullHeight = 248;
        const int moveOffset = 214;


        private bool _expanded;
        public bool Expanded { get => _expanded; set { _expanded = value; RebuildHeight();} }

        public int downOffset = 0;

        public override Vector2 RealPosition { get => ParentUI.RealPosition + Offset + Vector2.UnitY * downOffset; }


        public void RebuildHeight()
        {
            if (Expanded)
            {
                Size = new Vector2(Size.X, fullHeight);
                backTexture = ModContent.GetTexture("Realms/UI/RealmUI/PanelBackFull");
                downOffset = 0;
                button.boolValue = true;

                foreach (CollapsiblePanel panel in panelList)
                    if (panel != this)
                        panel.Expanded = false;
            }
            else
            {
                Size = new Vector2(Size.X, collaspedHeight);
                backTexture = ModContent.GetTexture("Realms/UI/RealmUI/PanelBack");
                downOffset = 0;
                button.boolValue = false;

                bool past = false;
                foreach (CollapsiblePanel panel in panelList)
                {
                    if (panel == this)
                        past = true;

                    if (past)
                        panel.downOffset = 0;
                    else if (panel.Expanded == true)
                    {
                        downOffset = moveOffset;
                        break;
                    }
                }
            }
        }

        public ExpandButton button;
        public Texture2D titleTex;
        public List<CollapsiblePanel> panelList;

        public CollapsiblePanel(List<CollapsiblePanel> panels, Texture2D title)
        {
            Size = new Vector2(width, collaspedHeight);
            backTexture = ModContent.GetTexture("Realms/UI/RealmUI/PanelBack");

            titleTex = title;
            panelList = panels;
            if (!panelList.Contains(this))
                panelList.Add(this);

            Texture2D buttonTex = ModContent.GetTexture("Realms/UI/RealmUI/ExpandArrow");
            button = new ExpandButton() { Offset = new Vector2(width - 30, 4), Size = new Vector2(26, 26), texture = buttonTex, highlightedTexture = buttonTex, onClick = Toggle };
            AddElement(button);
            AddElement(new UISprite() {Size = new Vector2(134, 26), Offset = new Vector2(4, 4 ), texture = titleTex});
        }

        private void Toggle() => 
            Expanded = true; //Expanded = button.boolValue; //old version allows for them all to be collasped
    }

    public class RealmAltarUI : GUI
    {
        readonly AltarEntity altarEntity;

        readonly AltarItemData itemdata;

        public RealmAltarUI(AltarEntity altar)
        {
            altarEntity = altar;

            if (altarEntity.ItemData == null)
                altarEntity.ItemData = new AltarItemData();

            itemdata = altarEntity.ItemData;

            PostCreate();
        }

        const int mainUIWidth = 402;
        const int mainUIHeight = 332;

        const int effectSlotCount = 7;
        const int featureSlotCount = 9;

        readonly List<CollapsiblePanel> panelList = new List<CollapsiblePanel>();

        public void PostCreate()//oncreate gets called by the base ctor, so this method was made to be able to access the info passed in when this is created
        {
            backTexture = ModContent.GetTexture("Realms/UI/RealmUI/MainBackground");
            Size = new Vector2(mainUIWidth, mainUIHeight);
            Offset = (new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f) - (new Vector2(mainUIWidth + 200, mainUIHeight) * 0.5f);

            AddElement(new CloseButton(this) { texture = ModContent.GetTexture("Realms/UI/RealmUI/Close"), Offset = new Vector2(Size.X - 21, 11), baseColor = Color.LightGray });
            AddElement(new DragBar(this) { Size = new Vector2(Size.X - 28, 16) });

            #region debugbuttons
            AddElement(new Button()
            {
                Offset = new Vector2(-130, 160),
                Size = new Vector2(100, 25),
                onClick = EnterButtonClicked,
            });
            AddElement(new UIText()
            {
                Offset = new Vector2(-130, 162),
                TextString = "Enter Realm"
            });

            AddElement(new Button()
            {
                Offset = new Vector2(-200, 160),
                Size = new Vector2(53, 25),
                onClick = CreateButtonClicked,
            });
            AddElement(new UIText()
            {
                Offset = new Vector2(-200, 162),
                TextString = "Create"
            });
            #endregion

            Texture2D mainslotTex = ModContent.GetTexture("Realms/UI/RealmUI/MainSlot");
            GUI ItemSlotPanel = new GUI() { backTexture = mainslotTex, Size = mainslotTex.Size(), Offset = new Vector2(-112, 0) };
            AddElement(ItemSlotPanel);
            //ItemSlotPanel.AddElement(new DragBar(ItemSlotPanel));//debug and showing gui features
            ItemSlotPanel.AddElement(new ItemSlot()
            {
                ItemReference = itemdata.realmBook,
                itemAllowed = IsRealmItem,
                Size = new Vector2(40, 40),
                Offset = new Vector2(32, 34),
                iconTexture = ModContent.GetTexture("Realms/UI/RealmUI/SlotIcon"),
                slotTexture = Main.blackTileTexture,
                slotColor = Color.Transparent,
                slotUnfocusedColor = Color.Transparent,
            });

            AddElement(new CollapsiblePanel(panelList, ModContent.GetTexture("Realms/UI/RealmUI/Features")) { Offset = new Vector2(6, 6) });

            AddElement(new CollapsiblePanel(panelList, ModContent.GetTexture("Realms/UI/RealmUI/Effects")) { Offset = new Vector2(6, 8 + CollapsiblePanel.collaspedHeight) });

            AddElement(new CollapsiblePanel(panelList, ModContent.GetTexture("Realms/UI/RealmUI/Misc")) { Offset = new Vector2(6, 10 + CollapsiblePanel.collaspedHeight * 2) });
            panelList[panelList.Count - 1].Expanded = true;




            AddElement(new HoverText() { Size = new Vector2(Size.X - 28, 16), TextString = "Terraria privilege revoked." });
        }

        public override void OnDeactivate()
        {
            
        }

        private bool IsRealmItem(Item item) => 
            item.IsAir || item.modItem is Items.RealmBook;

        private void EnterButtonClicked()
        {
            panelList[0].Expanded = true;
        }

        private void CreateButtonClicked()
        {

        }
    }
}
