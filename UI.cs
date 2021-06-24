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
using ReLogic.Graphics;
using ReLogic.Localization.IME;
using ReLogic.OS;
using ReLogic.Utilities;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;
using static Realms.ContentHandler;
using Microsoft.Xna.Framework.Input;

namespace Realms
{
    public static class UIHandler
    {
        public static List<UI> ActiveUIList = new List<UI>();
        public static List<UI> ActivationQueue = new List<UI>();
        public static List<UI> DeactivationQueue = new List<UI>();

        public static IUIElement EmptyElement = new Empty();

        public static bool LastMouseLeft;

        public static void Update()
        {
            foreach (UI ui in ActivationQueue)
                if (!ActiveUIList.Contains(ui))
                    ActiveUIList.Add(ui);
            ActivationQueue.Clear();

            foreach (UI ui in ActiveUIList)
            {
                if (ui.CheckHasInteracted(HasLeftClicked))
                {
                    ActiveUIList.Remove(ui);
                    ActiveUIList.Insert(0, ui);//they are drawn in reverse order, so this moves it to the start of the list so its on top and checked first
                    break;
                }

            }

            foreach (UI ui in DeactivationQueue)
                if (ActiveUIList.Contains(ui))
                    ActiveUIList.Remove(ui);
            DeactivationQueue.Clear();

            LastMouseLeft = Main.mouseLeft;
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            for(int i = ActiveUIList.Count; i > 0; i--)//draws each ui in reverse order
            {
                ActiveUIList[i - 1].Draw(spriteBatch);
            }
        }

        public static bool HasLeftClicked => Main.mouseLeft && !LastMouseLeft;

        public static Rectangle GetRect(this IUIElement element) =>
            new Rectangle((int)(element.RealPosition.X), (int)(element.RealPosition.Y), (int)element.Size.X, (int)element.Size.Y);

        public static bool MouseOver(this IUIElement element) => 
            element.GetRect().Contains(new Point(Main.mouseX, Main.mouseY));

        public static void BlockItemUse()
        {
            Main.LocalPlayer.delayUseItem = true;
            Main.mouseLeftRelease = false;
            Main.LocalPlayer.releaseUseItem = false;
            Main.LocalPlayer.mouseInterface = true;
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
        Vector2 Offset { get; set; }
        Vector2 RealPosition { get; }
        void Draw(SpriteBatch spriteBatch);
        bool CheckHasInteracted(bool ClickReceived);
    }

    public class CloseButton : IUIElement
    {
        public IUIElement ParentUI { get; set; }
        public Vector2 Size { get; set; } = new Vector2(16, 16);
        public Vector2 Offset { get; set; } = Vector2.Zero;
        public Vector2 RealPosition { get => ParentUI.RealPosition + Offset; }

        public Texture2D texture = Main.cdTexture;
        public Color textureColor = Color.Gray;
        public Color highlightedColor = Color.White;

        public CloseButton(UI parentUI)
        {
            ParentUI = parentUI;
            Offset = new Vector2(ParentUI.Size.X - Size.X, 0);
        }

        public virtual void Draw(SpriteBatch spriteBatch) => spriteBatch.Draw(texture, this.GetRect(), this.MouseOver() ? highlightedColor : textureColor);

        public virtual bool CheckHasInteracted(bool ClickReceived)
        {
            if (this.MouseOver() && ClickReceived)
            {
                if (ParentUI is UI)
                    (ParentUI as UI).Deactivate();
                return true;
            }
            return false;
        }
    }

    public class DragBar : IUIElement
    {
        public IUIElement ParentUI { get; set; }
        public Vector2 Size { get; set; } = new Vector2(20, 20);
        public Vector2 Offset { get; set; } = Vector2.Zero;
        public Vector2 RealPosition { get => ParentUI.RealPosition + Offset; }

        public Texture2D texture = Main.blackTileTexture;
        public Color textureColor = Color.Transparent;
        public Color highlightedColor = Color.LightGoldenrodYellow * 0.25f;

        protected Vector2 mouseOffset = Vector2.Zero;

        public DragBar() { }
        public DragBar(UI parentUI)
        {
            ParentUI = parentUI;
            Size = new Vector2(parentUI.Size.X, 20);
        }

        public virtual void Draw(SpriteBatch spriteBatch) => spriteBatch.Draw(texture, this.GetRect(), this.MouseOver() ? highlightedColor : textureColor);

        public virtual bool CheckHasInteracted(bool ClickReceived)//the window slipping can be fixed by removing this and having a Var in UiHandler for dragged element
        {
            if (this.MouseOver())
            {
                if (Main.mouseLeft && UIHandler.LastMouseLeft && ParentUI is UI && (ParentUI as UI).HasFocus)
                {
                    ParentUI.Offset = new Vector2(Main.mouseX, Main.mouseY) + mouseOffset;
                }
                else
                    mouseOffset = ParentUI.Offset - new Vector2(Main.mouseX, Main.mouseY);
            }
            return false;
        }
    }

    public class Button : IUIElement
    {
        public IUIElement ParentUI { get; set; }
        public Vector2 Size { get; set; } = new Vector2(32, 32);
        public Vector2 Offset { get; set; } = Vector2.Zero;
        public Vector2 RealPosition { get => ParentUI.RealPosition + Offset; }

        public Texture2D texture = Main.blackTileTexture;
        public Color textureColor = Color.Blue;
        public Texture2D highlightedTexture = Main.blackTileTexture;
        public Color highlightedColor = Color.MediumPurple;

        public Action onClick;

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (this.MouseOver() && !Main.mouseLeft)
                spriteBatch.Draw(highlightedTexture, this.GetRect(), highlightedColor);
            else
                spriteBatch.Draw(texture, this.GetRect(), textureColor);
        }

        public virtual bool CheckHasInteracted(bool ClickReceived)
        {
            if (this.MouseOver() && ClickReceived)
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
        public Vector2 Offset { get; set; } = Vector2.Zero;
        public Vector2 RealPosition { get => ParentUI.RealPosition + Offset; }

        public Texture2D slotTexture = Main.inventoryBackTexture;
        public Color slotColor = Color.White;
        public Texture2D iconTexture = Main.EquipPageTexture[1];
        public Color iconColor = Color.White * 0.5f;

        public Func<Item, bool> itemAllowed;

        public Item storedItem = new Item();

        public ItemSlot() { storedItem.TurnToAir(); }

        public void TrySwapItem(ref Item item)
        {
            if(itemAllowed == null || itemAllowed(item))
            {
                Main.PlaySound(SoundID.Grab);
                Item temp = item;
                item = storedItem;
                storedItem = temp;
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            Rectangle rect = this.GetRect();

            spriteBatch.Draw(slotTexture, rect, slotColor);
            if (!storedItem.IsAir)
            {
                Texture2D tex = Main.itemTexture[storedItem.type];
                Rectangle frame = Main.itemAnimations[storedItem.type] != null ? Main.itemAnimations[storedItem.type].GetFrame(tex) : tex.Frame();
                Vector2 position = rect.TopLeft();
                Vector2 origin = (frame.Size() - rect.Size()) / 2;
                ModItem modItem = storedItem.modItem;

                if (modItem != null)
                {
                    if (modItem.PreDrawInInventory(spriteBatch, position, frame, Color.White, Color.White, origin, 1f))//vanilla item loader has a similar named method, keep in mind
                        spriteBatch.Draw(tex, position, frame, Color.White, 0f, origin, 1f, default, default);
                    modItem.PostDrawInInventory(spriteBatch, position, frame, Color.White, Color.White, origin, 1f);
                }
                else
                    spriteBatch.Draw(tex, position, frame, Color.White, 0f, origin, 1f, default, default);

                if(storedItem.stack > 1)
                    Utils.DrawBorderString(spriteBatch, storedItem.stack.ToString(), rect.Center() - new Vector2(rect.Width / 3, 0), Color.White);

                if (this.MouseOver())
                {
                    Main.HoverItem = storedItem;
                    Main.instance.MouseTextHackZoom("", Main.rare, 1);
                }

            }
            else
            {
                Vector2 origin = (iconTexture.Size() - rect.Size()) / 2;
                spriteBatch.Draw(iconTexture, rect.TopLeft(), null, iconColor, 0f, origin, 1f, default, default);
            }
        }

        public virtual bool CheckHasInteracted(bool ClickReceived)
        {
            if (this.MouseOver() && ClickReceived)
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
        public Vector2 Offset { get; set; } = Vector2.Zero;
        public Vector2 RealPosition { get => Offset; }

        public void Draw(SpriteBatch spriteBatch) { }
        public virtual bool CheckHasInteracted(bool ClickReceived) => false;
    }

    public class UI : IUIElement
    {
        public IUIElement ParentUI { get; set; } = UIHandler.EmptyElement;
        public Vector2 Size { get; set; } = new Vector2(100, 100);
        public Vector2 Offset { get; set; } = Vector2.Zero;
        public Vector2 RealPosition { get => ParentUI.RealPosition + Offset; }

        public Texture2D backTexture = Main.blackTileTexture;
        public Color focusedColor = Color.White * 0.5f;
        public Color unfocusedColor = Color.Gray * 0.5f;

        public List<IUIElement> elements = new List<IUIElement>();

        public UI()
        {
            Offset = (new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f) - (Size * 0.5f);
            OnCreate();
        }

        public virtual void OnCreate() { }

        public void AddElement(IUIElement element)
        {
            element.ParentUI = this;
            elements.Add(element);
        }
        public void Activate() =>
            UIHandler.ActivationQueue.Add(this);
        public void Deactivate() =>
            UIHandler.DeactivationQueue.Add(this);
        public bool HasFocus => UIHandler.ActiveUIList[0] == this;

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(backTexture, this.GetRect(), HasFocus ? focusedColor * 0.5f : unfocusedColor * 0.5f);

            foreach (IUIElement element in elements)
            {
                element.Draw(spriteBatch);
            }
        }

        public virtual bool CheckHasInteracted(bool ClickReceived)
        {
            if (this.MouseOver())
            {
                UIHandler.BlockItemUse();

                foreach (IUIElement element in elements)
                {
                    if (element.CheckHasInteracted(ClickReceived))
                        return true;
                }

                return ClickReceived;
            }
            else 
                return false;
        }
    }
}
