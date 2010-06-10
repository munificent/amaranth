using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bramble.Core;

using Amaranth.Util;
using Amaranth.Terminals;

namespace Amaranth.UI
{
    public class Screen : Control, IUserInterfaceScreen
    {
        public event EventHandler FocusChanged;

        public UserInterface UI { get { return mUI; } }

        public IFocusable FocusControl { get { return mFocus; } }

        public bool IsCurrent { get { return UI.CurrentScreen == this; } }

        public override ITerminal Terminal
        {
            get
            {
                return mTerminal;
            }
        }

        public Screen(string title) : base(title) { }

        public Screen() : this(String.Empty) { }

        public void FocusFirst()
        {
            foreach (Control control in Controls)
            {
                IFocusable focusable = control as IFocusable;

                if (focusable != null)
                {
                    Focus(focusable);
                    break;
                }
            }
        }

        public void Focus(IFocusable control)
        {
            if (mFocus != control)
            {
                IFocusable oldFocus = mFocus;
                mFocus = control;

                if (oldFocus != null)
                {
                    oldFocus.LoseFocus();
                }

                if (mFocus != null)
                {
                    mFocus.GainFocus();
                }

                if (FocusChanged != null) FocusChanged(this, EventArgs.Empty);
            }
        }

        public void ScreenKeyPress(KeyInfo key)
        {
            bool handled;

            if (mFocus == null)
            {
                // broadcast to controls
                handled = ControlKeyPress(key);
            }
            else
            {
                // just send to the focus control
                if (key.Down)
                {
                    handled = mFocus.KeyDown(key);
                }
                else
                {
                    handled = mFocus.KeyUp(key);
                }
            }

            // send to the screen if nothing else handled it
            if (!handled)
            {
                IInputHandler handler = this as IInputHandler;

                if (handler != null)
                {
                    if (key.Down)
                    {
                        handler.KeyDown(key);
                    }
                    else
                    {
                        handler.KeyUp(key);
                    }
                }
            }
        }

        protected override Rect GetBounds()
        {
            return new Rect(mUI.Size);
        }

        protected virtual void OnActivate()
        {
            // do nothing
        }

        protected virtual void OnDeactivate()
        {
            // do nothing
        }

        #region IUserInterfaceScreen Members

        void IUserInterfaceScreen.Attach(NotNull<UserInterface> ui, ITerminal terminal)
        {
            if (mUI != null) throw new InvalidOperationException("Can only attach a Screen to a UI once.");

            mUI = ui;
            mTerminal = terminal;

            Init();
        }

        void IUserInterfaceScreen.Detach(NotNull<UserInterface> ui)
        {
            if (mUI == null) throw new InvalidOperationException("Cannot detach an unattached Screen.");

            mUI = null;

            End();
        }

        void IUserInterfaceScreen.Activate()
        {
            OnActivate();
        }

        void IUserInterfaceScreen.Deactivate()
        {
            OnDeactivate();
        }

        #endregion

        private UserInterface mUI;
        private ITerminal mTerminal;
        private IFocusable mFocus;
    }
}
