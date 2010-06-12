using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bramble.Core;
using Malison.Core;

using Amaranth.Util;

namespace Amaranth.UI
{
    public abstract class Control : ICollectible<ControlCollection, Control>
    {
        public string Title { get { return mTitle; } }

        public Rect Bounds
        {
            get { return GetBounds(); }
        }

        public ControlCollection Controls { get { return mControls; } }

        public Control Parent
        {
            get
            {
                if (mCollection != null) return mCollection.Parent;
                return null;
            }
        }

        public Screen Screen
        {
            get
            {
                Screen screen = this as Screen;
                if (screen != null) return screen;

                if (Parent != null) return Parent.Screen;

                return null;
            }
        }

        public bool HasFocus
        {
            get
            {
                // cannot have focus if not on a screen
                if (Screen == null) return false;

                return Screen.FocusControl == this;
            }
        }

        /// <summary>
        /// Gets whether or not this Control can lose focus (by giving it to another Control).
        /// </summary>
        public bool CanChangeFocus
        {
            get
            {
                // cannot if not on a screen
                if (Parent == null) return false;

                foreach (Control sibling in Parent.Controls)
                {
                    // found another focusable?
                    if ((sibling != this) && (sibling is IFocusable)) return true;
                }

                return false;
            }
        }

        public ColorPair TitleColor
        {
            get
            {
                if (HasFocus)
                {
                    return new ColorPair(TermColor.Yellow, TermColor.Black);
                }
                else
                {
                    return new ColorPair(TermColor.Gray, TermColor.Black);
                }
            }
        }

        public ColorPair TextColor
        {
            get
            {
                if (HasFocus)
                {
                    return new ColorPair(TermColor.White, TermColor.Black);
                }
                else
                {
                    return new ColorPair(TermColor.DarkGray, TermColor.Black);
                }
            }
        }

        public ColorPair SelectionColor
        {
            get
            {
                if (HasFocus)
                {
                    return new ColorPair(TermColor.Black, TermColor.Yellow);
                }
                else
                {
                    return new ColorPair(TermColor.White, TermColor.Black);
                }
            }
        }

        public virtual ITerminal Terminal
        {
            get
            {
                return Parent.Terminal[Bounds];
            }
        }

        public bool Visible
        {
            get { return mVisible; }
            set
            {
                if (mVisible != value)
                {
                    mVisible = value;

                    if (mVisible)
                    {
                        // showing the control, so just paint it
                        Repaint();
                    }
                    else if ((Screen != null) && (Screen.UI != null))
                    {
                        // hiding the control, so repaint the entire ui to make it disappear
                        //### bob: not optimal. :(
                        Screen.UI.Repaint();
                    }
                }
            }
        }

        public void Repaint()
        {
            // only if attached
            if ((Parent != null) && (Parent.Terminal != null))
            {
                Paint(Parent.Terminal);
            }
        }

        public void Paint(ITerminal parentTerminal)
        {
            using (Profiler.Block("control paint"))
            {
                if (mVisible && (Bounds.Area > 0))
                {
                    ITerminal localTerminal = parentTerminal[Bounds];

                    // paint this control
                    OnPaint(localTerminal);

                    // paint the child controls
                    foreach (Control control in mControls)
                    {
                        control.Paint(localTerminal);
                    }
                }
            }
        }

        public void FocusPrevious()
        {
            ChangeFocus(-1);
        }

        public void FocusNext()
        {
            ChangeFocus(1);
        }

        public bool ControlKeyPress(KeyInfo key)
        {
            bool handled = false;

            // send to this control
            IInputHandler handler = this as IInputHandler;
            if (handler != null)
            {
                if (key.Down)
                {
                    handled = handler.KeyDown(key);
                }
                else
                {
                    handled = handler.KeyUp(key);
                }
            }

            // send to child controls
            if (!handled)
            {
                foreach (Control control in mControls)
                {
                    handled = control.ControlKeyPress(key);
                }
            }

            return handled;
        }

        protected Control(string title)
        {
            mTitle = title;

            mControls = new ControlCollection(this);
            mEndCallbacks = new List<Action>();
        }

        protected Control()
            : this(String.Empty)
        {
        }

        protected void ListenTo<TSender, TArgs>(IEvent<TSender, TArgs> eventObj, Action<TSender, TArgs> handler)
        {
            /*
            // wrap it in a listener that will only forward on the event
            // if the control's screen is active. prevents (for example,
            // updating the inventory panel, which overwrites the modal
            // popup, when buying something in a store).
            Action<TSender, TArgs> activeScreenHandler = (sender, args) =>
                {
                    if (Screen.IsCurrent) handler(sender, args);
                };

            // listen to it
            eventObj.Add(activeScreenHandler);

            // and keep track of it to unregister later
            mEndCallbacks.Add(() => eventObj.Remove(activeScreenHandler));
            */

            // listen to it
            eventObj.Add(handler);

            // and keep track of it to unregister later
            mEndCallbacks.Add(() => eventObj.Remove(handler));
        }

        protected void RepaintOn<TSender, TArgs>(IEvent<TSender, TArgs> eventObj)
        {
            ListenTo(eventObj, (sender, args) => Repaint());
        }

        protected abstract Rect GetBounds();

        protected virtual void OnPaint(ITerminal terminal)
        {
            terminal.Clear();
        }

        protected virtual void OnAttach(Control parent)
        {
        }

        protected virtual void Init()
        {
            // send to child controls
            foreach (Control child in Controls)
            {
                child.Init();
            }
        }

        protected virtual void End()
        {
            // run all the stored shutdown callbacks
            mEndCallbacks.ForEach((callback) => callback());

            // send to child controls
            foreach (Control child in Controls)
            {
                child.End();
            }
        }

        private void ChangeFocus(int offset)
        {
            if (Screen != null)
            {
                int count = Parent.Controls.Count;
                int index = Parent.Controls.IndexOf(this);

                while (true)
                {
                    index = (index + count + offset) % count;

                    Control control = Parent.Controls[index];

                    if (control == this)
                    {
                        // wrapped around
                        break;
                    }

                    IFocusable focusable = control as IFocusable;

                    if (focusable != null)
                    {
                        Screen.Focus(focusable);
                        break;
                    }
                }
            }
        }

        #region ICollectible<ControlCollection,Control> Members

        void ICollectible<ControlCollection, Control>.SetCollection(ControlCollection collection)
        {
            mCollection = collection;

            if (mCollection != null)
            {
                OnAttach(Parent);
            }
        }

        #endregion

        private ControlCollection mCollection;

        private string mTitle;
        private bool mVisible = true;

        private readonly ControlCollection mControls;
        private readonly List<Action> mEndCallbacks;
    }
}
