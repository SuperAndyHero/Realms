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
        public static List<IUIElement> ActiveUIList = new List<IUIElement>();
        public static List<IUIElement> ActivationQueue = new List<IUIElement>();
        public static List<IUIElement> DeactivationQueue = new List<IUIElement>();

        public static readonly IUIElement EmptyElement = new Empty();

        public static IUIElement MouseElement;
        public static Vector2 MouseElementOffset = Vector2.Zero;

        public static bool LastMouseLeft;

        public static void Update()
        {

            if (MouseElement != null)
                MouseElement.Offset = new Vector2(Main.mouseX, Main.mouseY) + MouseElementOffset;
            MouseElement = null;

            foreach (GUI ui in ActivationQueue)
                if (!ActiveUIList.Contains(ui))
                    ActiveUIList.Add(ui);
            ActivationQueue.Clear();

            foreach (GUI ui in ActiveUIList)
            {
                if (ui.CheckHasInteracted(HasLeftClicked))
                {
                    ActiveUIList.Remove(ui);
                    ActiveUIList.Insert(0, ui);//they are drawn in reverse order, so this moves it to the start of the list so its on top and checked first
                    break;
                }

            }

            foreach (GUI ui in DeactivationQueue)
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

        public static void Activate(this IUIElement element) =>
            ActivationQueue.Add(element);
        public static void Deactivate(this IUIElement element) =>
            DeactivationQueue.Add(element);

        public static bool HasFocus(this IUIElement element) => 
            ActiveUIList[0] == element || (element != EmptyElement && element.ParentUI.HasFocus());
    }

    //TODO
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
        public Color unfocusedColor = Color.Gray;
        public Color baseColor = Color.LightGray;
        public Color highlightedColor = Color.White;

        public CloseButton(IUIElement parentUI)
        {
            ParentUI = parentUI;
            Offset = new Vector2(ParentUI.Size.X - Size.X, 0);
        }

        public virtual void Draw(SpriteBatch spriteBatch) => spriteBatch.Draw(texture, this.GetRect(), ParentUI.HasFocus() ? this.MouseOver() ? highlightedColor : baseColor : unfocusedColor);

        public virtual bool CheckHasInteracted(bool ClickReceived)
        {
            if (this.MouseOver() && ClickReceived)
            {
                ParentUI.Deactivate();
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
        //TODO unfocused color
        public Color baseColor = Color.Transparent;
        public Color highlightedColor = Color.LightGoldenrodYellow * 0.25f;

        public DragBar() { }
        public DragBar(IUIElement parentUI)
        {
            ParentUI = parentUI;
            Size = new Vector2(parentUI.Size.X, 20);
        }
        bool drag = false;//remove

        public virtual void Draw(SpriteBatch spriteBatch) => spriteBatch.Draw(texture, this.GetRect(), (this.MouseOver() && ParentUI.HasFocus()) ? highlightedColor : baseColor);

        public virtual bool CheckHasInteracted(bool ClickReceived)//the window slipping can be fixed by removing this and having a Var in UiHandler for dragged element
        {
            if (this.MouseOver())
            {
                if (ClickReceived)
                {
                    UIHandler.MouseElementOffset = ParentUI.Offset - new Vector2(Main.mouseX, Main.mouseY);
                    drag = true;
                    return true;
                }
                if (drag)
                {
                    if (Main.mouseLeft && UIHandler.LastMouseLeft && ParentUI.HasFocus())
                        UIHandler.MouseElement = ParentUI;
                    else
                        drag = false;
                }
            }
            //else //shouldn't be needed since you can move your mouse off this element without letting go of left mouse
            //    drag = false;
            return false;
        }
    }

    public class Button : IUIElement //for buttons to darken when the UI is out of focus, this needs a new color for 
    {
        public IUIElement ParentUI { get; set; }
        public Vector2 Size { get; set; } = new Vector2(32, 32);
        public Vector2 Offset { get; set; } = Vector2.Zero;
        public Vector2 RealPosition { get => ParentUI.RealPosition + Offset; }

        public Color unfocusedColor = Color.DimGray;
        public Texture2D texture = Main.blackTileTexture;
        public Color baseColor = Color.LightGray;
        public Texture2D highlightedTexture = Main.blackTileTexture;
        public Color highlightedColor = Color.White;

        public Action onClick;

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            bool hasFocus = ParentUI.HasFocus();
            if (this.MouseOver() && !Main.mouseLeft && hasFocus)
                spriteBatch.Draw(highlightedTexture, this.GetRect(), highlightedColor);
            else
                spriteBatch.Draw(texture, this.GetRect(), hasFocus ? baseColor : unfocusedColor);
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

    public class UIText : IUIElement
    {
        public IUIElement ParentUI { get; set; }
        public Vector2 Size { get; set; } = Vector2.One;
        public Vector2 Offset { get; set; } = Vector2.Zero;
        public Vector2 RealPosition { get => ParentUI.RealPosition + Offset; }

        public virtual Color UnfocusedTextColor { get; set; } = Color.Gray;
        public virtual Color TextColor { get; set; } = Color.White;
        public virtual float TextScale { get; set; } = 1f;
        public virtual string TextString { get; set; } = "Text Here";

        public virtual void Draw(SpriteBatch spriteBatch) =>
            Utils.DrawBorderString(spriteBatch, TextString, RealPosition, ParentUI.HasFocus() ? TextColor : UnfocusedTextColor, TextScale);

        public virtual bool CheckHasInteracted(bool ClickReceived) => false;
    }

    public class UISprite : IUIElement
    {
        public IUIElement ParentUI { get; set; }
        public Vector2 Size { get; set; } = Vector2.One;
        public Vector2 Offset { get; set; } = Vector2.Zero;
        public Vector2 RealPosition { get => ParentUI.RealPosition + Offset; }

        public Texture2D texture = Main.blackTileTexture;
        public Color baseColor = Color.White;
        public Color unfocusedColor = Color.Gray;

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, this.GetRect(), ParentUI.HasFocus() ? baseColor : unfocusedColor);
        }

        public virtual bool CheckHasInteracted(bool ClickReceived) => false;
    }

    public class HoverText : IUIElement
    {
        public IUIElement ParentUI { get; set; }
        public Vector2 Size { get; set; } = new Vector2(32, 32);
        public Vector2 Offset { get; set; } = Vector2.Zero;
        public Vector2 RealPosition { get => ParentUI.RealPosition + Offset; }

        public virtual Color TextColor { get; set; } = Color.White;
        public virtual float TextScale { get; set; } = 1f;
        public virtual string TextString { get; set; } = "Text Here";

        public virtual void Draw(SpriteBatch spriteBatch) {
            if (this.MouseOver())
                Main.instance.MouseTextHackZoom(TextString, Main.rare, 1);
        }

        public virtual bool CheckHasInteracted(bool ClickReceived) => false;
    }

    public class ItemSlot : IUIElement
    {
        public IUIElement ParentUI { get; set; }
        public Vector2 Size { get; set; } = new Vector2(32, 32);
        public Vector2 Offset { get; set; } = Vector2.Zero;
        public Vector2 RealPosition { get => ParentUI.RealPosition + Offset; }


        public Texture2D slotTexture = Main.inventoryBackTexture;
        public Color slotColor = Color.White;
        public Color slotUnfocusedColor = Color.Gray;

        public Texture2D iconTexture = Main.EquipPageTexture[1];
        public Color iconColor = Color.White;
        public Color iconUnfocusedColor = Color.Gray;

        public Color itemColor = Color.White;
        public Color itemUnfocusedColor = Color.Gray;

        public Func<Item, bool> itemAllowed;

        public Item storedItem = new Item();

        public ItemSlot() { storedItem.TurnToAir(); }

        public void TrySwapItem(ref Item item)
        {
            if(itemAllowed == null || itemAllowed(item))
            {
                if(!storedItem.IsAir || !item.IsAir)//so sound doesn't play when clicking a empty slot
                    Main.PlaySound(SoundID.Grab);
                Item temp = item;
                item = storedItem;
                storedItem = temp;
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            Rectangle rect = this.GetRect();
            bool hasFocus = ParentUI.HasFocus();

            spriteBatch.Draw(slotTexture, rect, hasFocus ? slotColor : slotUnfocusedColor);
            if (!storedItem.IsAir)
            {
                Texture2D tex = Main.itemTexture[storedItem.type];
                Color texColor = hasFocus ? itemColor : itemUnfocusedColor;
                Rectangle frame = Main.itemAnimations[storedItem.type] != null ? Main.itemAnimations[storedItem.type].GetFrame(tex) : tex.Frame();
                Vector2 position = rect.TopLeft();
                Vector2 origin = (frame.Size() - rect.Size()) / 2;
                ModItem modItem = storedItem.modItem;

                if (modItem != null)
                {
                    if (modItem.PreDrawInInventory(spriteBatch, position, frame, texColor, texColor, origin, 1f))//vanilla item loader has a similar named method, keep in mind
                        spriteBatch.Draw(tex, position, frame, texColor, 0f, origin, 1f, default, default);
                    modItem.PostDrawInInventory(spriteBatch, position, frame, texColor, texColor, origin, 1f);
                }
                else
                    spriteBatch.Draw(tex, position, frame, texColor, 0f, origin, 1f, default, default);

                if(storedItem.stack > 1)
                    Utils.DrawBorderString(spriteBatch, storedItem.stack.ToString(), rect.Center() - new Vector2(rect.Width / 3, 0), texColor);//TODO fix text offset

                if (this.MouseOver())
                {
                    Main.HoverItem = storedItem;
                    Main.instance.MouseTextHackZoom("", Main.rare, 1);
                }

            }
            else
            {
                Vector2 origin = (iconTexture.Size() - rect.Size()) / 2;
                spriteBatch.Draw(iconTexture, rect.TopLeft(), null, hasFocus ? iconColor : iconUnfocusedColor, 0f, origin, 1f, default, default);
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

    public class GUI : IUIElement
    {
        public IUIElement ParentUI { get; set; } = UIHandler.EmptyElement;
        public Vector2 Size { get; set; } = new Vector2(100, 100);
        public Vector2 Offset { get; set; } = Vector2.Zero;
        public Vector2 RealPosition { get => ParentUI.RealPosition + Offset; }

        public Texture2D backTexture = Main.blackTileTexture;
        public Color focusedColor = Color.White;
        public Color unfocusedColor = Color.Gray;

        public List<IUIElement> elements = new List<IUIElement>();

        public GUI()
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

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(backTexture, this.GetRect(), this.HasFocus() ? focusedColor : unfocusedColor);

            foreach (IUIElement element in elements)
            {
                element.Draw(spriteBatch);
            }
        }

        public virtual bool CheckHasInteracted(bool ClickReceived)
        {
                bool mouseOver = this.MouseOver();
                bool interacted = false;

                foreach (IUIElement element in elements)
                {
                    if (element.CheckHasInteracted(ClickReceived))
                        interacted = true;
                }

                if(mouseOver || interacted)
                    UIHandler.BlockItemUse();

                return (mouseOver && ClickReceived) || interacted;
        }
    }
}
