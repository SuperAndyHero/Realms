using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Realms.UI
{
    public class EffectSlotContainer
    {
        public Ref<Item> effect;
        public Ref<Item> location;
    }

    public class FeatureSlotContainer
    {
        public Ref<Item> feature;
        public Ref<Item> rarity;
        public Ref<Item> size;
        public Ref<Item> freq;
        public Ref<Item> location;
        public List<Ref<Item>> tiles = new List<Ref<Item>>();
        public PatternSlotContainer patternSlot = new PatternSlotContainer();
    }

    public class PatternSlotContainer
    {
        public Ref<Item> pattern;
        public Ref<Item> freq;
    }

    public class MiscSlotContainer
    {
        public Ref<Item> misc;
    }

    public class RealmAltarUI : GUI
    {
        const int mainUIWidth = 402;
        const int mainUIHeight = 332;

        ItemSlot bookSlot;

        public override void OnCreate()
        {
            backTexture = ModContent.GetTexture("Realms/UI/RealmUI/MainBackground");
            Size = new Vector2(mainUIWidth, mainUIHeight);
            Offset = (new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f) - (new Vector2(mainUIWidth + 200, mainUIHeight) * 0.5f);

            AddElement(new CloseButton(this) { texture = ModContent.GetTexture("Realms/UI/RealmUI/Close"), Offset = new Vector2(Size.X - 21, 11), baseColor = Color.LightGray});
            AddElement(new DragBar(this) { Size = new Vector2(Size.X - 28, 16)});
            AddElement(new HoverText() { Size = new Vector2(Size.X - 28, 16), TextString = "Your balls. Hand em over." });
            Microsoft.Xna.Framework.Graphics.Texture2D mainslotTex = ModContent.GetTexture("Realms/UI/RealmUI/MainSlot");



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



            GUI ItemSlotPanel = new GUI() { backTexture = mainslotTex, Size = mainslotTex.Size(), Offset = new Vector2(-112, 0) };
            AddElement(ItemSlotPanel);
            //ItemSlotPanel.AddElement(new DragBar(ItemSlotPanel));//debug and showing gui features
            bookSlot = new ItemSlot()
            {
                itemAllowed = IsRealmItem,
                Size = new Vector2(40, 40),
                Offset = new Vector2(32, 34),
                iconTexture = ModContent.GetTexture("Realms/UI/RealmUI/SlotIcon"),
                slotTexture = Main.blackTileTexture,
                slotColor = Color.Transparent,
                slotUnfocusedColor = Color.Transparent,
            };
            ItemSlotPanel.AddElement(bookSlot);
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
