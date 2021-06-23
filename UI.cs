using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Realms.Effects;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;
using static Realms.ContentHandler;

namespace Realms
{
    public static class UIHandler
    {
        public static List<UI> ActiveUIList = new List<UI>();

        public static void Update()
        {
            foreach(UI ui in ActiveUIList)
            {
                if (ui.CheckClicked())
                {
                    ActiveUIList.Remove(ui);
                    ActiveUIList.Insert(0, ui);//they are drawn in reverse order, so this moves it to the start of the list so its on top and checked first
                    break;
                }
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            for(int i = ActiveUIList.Count - 1; i > 0; i--)
            {
                ActiveUIList[i].Draw(spriteBatch);
            }
        }

        public static Rectangle GetRect(this IUIElement element) =>
            new Rectangle((int)(element.Position.X + element.ParentUI.Position.X), (int)(element.Position.Y + element.ParentUI.Position.Y), (int)element.Size.X, (int)element.Size.Y);

        public static bool MouseOver(this IUIElement element) =>
            element.GetRect().Contains(new Point(Main.mouseX, Main.mouseY));

        public static bool HasClickedOn(this IUIElement element)
        {
            if (Main.mouseLeft && Main.mouseLeftRelease && element.MouseOver())
            {
                Main.mouseLeftRelease = false;
                Main.LocalPlayer.releaseUseItem = false;
                Main.LocalPlayer.mouseInterface = true;
                return true;
            }
            return false;
        }

    }

    //TODO
    //test it
    //figure out unfocused color
    //figure out Ui scale (might be possible using RT and Main.UiScaleMatrix)


    public interface IUIElement
    {
        IUIElement ParentUI { get; set; }
        Vector2 Size { get; set; }
        Vector2 Position { get; set; }
        void Draw(SpriteBatch spriteBatch);
        bool CheckClicked();
    }

    public class Button : IUIElement
    {
        public IUIElement ParentUI { get; set; }
        public Vector2 Size { get; set; } = new Vector2(32, 32);
        public Vector2 Position { get; set; } = Vector2.Zero;

        public Texture2D texture = Main.blackTileTexture;
        public Color textureColor = Color.White;
        public Texture2D highlightedTexture = Main.blackTileTexture;
        public Color highlightedColor = Color.White;

        public Action onClick;

        public Button() { }
        public Button(UI parent) =>
            ParentUI = parent;

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (this.MouseOver() && !Main.mouseLeft)
                spriteBatch.Draw(highlightedTexture, this.GetRect(), highlightedColor);
            else
                spriteBatch.Draw(texture, this.GetRect(), textureColor);
        }

        public virtual bool CheckClicked()
        {
            if (this.HasClickedOn())
            {
                onClick?.Invoke();
                return true;
            }
            return false;
        }
    }

    public class ItemSlot : IUIElement
    {
        public IUIElement ParentUI { get; set; }
        public Vector2 Size { get; set; } = new Vector2(32, 32);
        public Vector2 Position { get; set; } = Vector2.Zero;

        public Texture2D slotTexture = Main.blackTileTexture;
        public Color slotColor = Color.White;
        public Texture2D iconTexture = Main.blackTileTexture;
        public Color iconColor = Color.White;

        public Func<Item, bool> itemAllowed;

        public Item storedItem;

        public ItemSlot() { }
        public ItemSlot(UI parent) =>
            ParentUI = parent;

        public void TrySwapItem(ref Item item)
        {
            if(itemAllowed == null || itemAllowed(item))
            {
                Item temp = item;
                item = storedItem;
                storedItem = temp;
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            Rectangle rect = this.GetRect();

            spriteBatch.Draw(slotTexture, rect, slotColor);
            if(storedItem != null)
            {
                ModItem modItem = storedItem.modItem;
                if(modItem != null)
                {
                    if(modItem.PreDrawInInventory(spriteBatch, rect.TopLeft(), rect, Color.AliceBlue, Color.Red, rect.Center.ToVector2(), 1f))
                        spriteBatch.Draw(ModContent.GetTexture(modItem.Texture), rect, Color.Green);
                    modItem.PostDrawInInventory(spriteBatch, rect.TopLeft(), rect, Color.MediumPurple, Color.Aquamarine, rect.Center.ToVector2(), 1f);
                }
                else
                    spriteBatch.Draw(Main.itemTexture[storedItem.type], rect, Color.YellowGreen);
            }
            else
                spriteBatch.Draw(iconTexture, rect, iconColor);
        }

        public virtual bool CheckClicked()
        {
            if (this.HasClickedOn())
            {
                TrySwapItem(ref Main.mouseItem);
                return true;
            }
            return false;
        }
    }

    public class Empty : IUIElement
    {
        public IUIElement ParentUI { get; set; }
        public Vector2 Size { get; set; } = Vector2.Zero;
        public Vector2 Position { get; set; } = Vector2.Zero;

        public void Draw(SpriteBatch spriteBatch) { }
        public virtual bool CheckClicked() => false;
    }

    public class UI : IUIElement
    {
        public IUIElement ParentUI { get; set; } = new Empty();
        public Vector2 Size { get; set; } = new Vector2(100, 100);
        public Vector2 Position { get; set; } = Vector2.Zero;


        public Texture2D backTexture = Main.blackTileTexture;
        public Color focusedColor = Color.White;
        public Color unfocusedColor = Color.Gray;

        public List<IUIElement> elements = new List<IUIElement>();

        public UI()
        {
            Position = (new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f) - (Size * 0.5f);
            OnCreate();
        }

        public virtual void OnCreate() { }

        public void AddElement(IUIElement element)
        {
            element.ParentUI = this;
            elements.Add(element);
        }
        public void Activate()
        {
            if (!UIHandler.ActiveUIList.Contains(this))
                UIHandler.ActiveUIList.Add(this);
        }
        public void Deactivate()
        {
            if (UIHandler.ActiveUIList.Contains(this))
                UIHandler.ActiveUIList.Remove(this);
        }
        public bool HasFocus => UIHandler.ActiveUIList[0] == this;

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(backTexture, this.GetRect(), HasFocus ? focusedColor : unfocusedColor);

            foreach (IUIElement element in elements)
            {
                element.Draw(spriteBatch);
            }
        }

        public virtual bool CheckClicked()
        {
            foreach(IUIElement element in elements)
            {
                if (element.CheckClicked())
                    return true;
            }
             if (this.HasClickedOn())
                return true;
            else
                return false;
        }
    }
}
