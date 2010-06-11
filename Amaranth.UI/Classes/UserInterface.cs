using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Bramble.Core;
using Malison.Core;
using Malison.WinForms;

using Amaranth.Util;

namespace Amaranth.UI
{
    public class UserInterface
    {
        public event EventHandler Quitting;

        public string Title { get { return mForm.Text; } }

        public Vec Size { get { return mForm.Terminal.Size; } }

        public Screen CurrentScreen
        {
            get
            {
                if (mScreens.Count == 0) return null;

                return mScreens.Peek();
            }
        }

        public UserInterface(string title)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            mForm = new TerminalForm(title, 120, 50);
            mForm.TerminalControl.KeyDown += TerminalControl_KeyDown;
            mForm.TerminalControl.KeyUp += TerminalControl_KeyUp;
            mForm.FormClosing += Form_FormClosing;
        }

        public void Run(NotNull<Screen> startScreen)
        {
            SetScreen(startScreen);

            Application.Run(mForm);
        }

        public void Quit()
        {
            mForm.Close();
        }

        public void SetScreen(NotNull<Screen> screen)
        {
            // remove the current screen (if any)
            if (mScreens.Count > 0)
            {
                if (mScreens.Count == 0) throw new InvalidOperationException("Cannot pop when there are no screens on the stack.");

                // unbind the screen
                IUserInterfaceScreen oldScreen = (IUserInterfaceScreen)mScreens.Pop();
                oldScreen.Detach(this);
            }

            mScreens.Push(screen);

            IUserInterfaceScreen uiScreen = (IUserInterfaceScreen)screen.Value;
            uiScreen.Attach(this, mForm.Terminal);

            Repaint();
        }

        public void PushScreen(NotNull<Screen> screen)
        {
            if (CurrentScreen != null)
            {
                IUserInterfaceScreen oldScreen = CurrentScreen;
                oldScreen.Deactivate();
            }

            mScreens.Push(screen);

            IUserInterfaceScreen uiScreen = (IUserInterfaceScreen)screen.Value;
            uiScreen.Attach(this, mForm.Terminal);

            Repaint();
        }

        public void PopScreen()
        {
            if (mScreens.Count == 0) throw new InvalidOperationException("Cannot pop when there are no screens on the stack.");

            // unbind the screen
            Screen screen = mScreens.Pop();

            IUserInterfaceScreen uiScreen = (IUserInterfaceScreen)screen;
            uiScreen.Detach(this);

            if (CurrentScreen != null)
            {
                IUserInterfaceScreen newScreen = CurrentScreen;
                newScreen.Activate();
            }

            Repaint();
        }

        public void Repaint()
        {
            foreach (Screen screen in mScreens.Reverse())
            {
                screen.Paint(mForm.Terminal);
            }
        }

        public void Update()
        {
            mForm.Update();
        }

        public void Delay()
        {
            Thread.Sleep(100);
        }

        public bool CheckForCancel()
        {
            bool cancelled = false;

            // take over the keyboard handling so the ui doesn't try to
            // process key presses
            mListener = KeyListener.CheckForCancel;
            mLastKey = null;

            // process events for a bit
            //### bob: at some point, may want a slight delay here
            /*DateTime endTime = DateTime.Now + new TimeSpan(0, 0, 1);
            while (DateTime.Now < endTime)*/
            {
                Application.DoEvents();
            }

            // cancel if any key was pressed
            cancelled = (mLastKey != null);

            // return key handling back to the ui
            mListener = KeyListener.Screen;
            mLastKey = null;

            return cancelled;
        }

        public KeyInfo ReadKey()
        {
            mListener = KeyListener.ReadKey;
            mLastKey = null;

            while (mLastKey == null)
            {
                Application.DoEvents();
            }

            KeyInfo key = mLastKey.Value;

            // return key handling back to the ui
            mListener = KeyListener.Screen;
            mLastKey = null;

            return key;
        }

        private bool ConvertToKeyInfo(KeyEventArgs args, out KeyInfo info)
        {
            bool recognized = true;

            info = new KeyInfo();

            switch (args.KeyCode)
            {
                case Keys.Up:       info.Key = Key.Up; break;
                case Keys.Down:     info.Key = Key.Down; break;
                case Keys.Left:     info.Key = Key.Left; break;
                case Keys.Right:    info.Key = Key.Right; break;

                case Keys.A:        info.Key = Key.A; break;
                case Keys.B:        info.Key = Key.B; break;
                case Keys.C:        info.Key = Key.C; break;
                case Keys.D:        info.Key = Key.D; break;
                case Keys.E:        info.Key = Key.E; break;
                case Keys.F:        info.Key = Key.F; break;
                case Keys.G:        info.Key = Key.G; break;
                case Keys.H:        info.Key = Key.H; break;
                case Keys.I:        info.Key = Key.I; break;
                case Keys.J:        info.Key = Key.J; break;
                case Keys.K:        info.Key = Key.K; break;
                case Keys.L:        info.Key = Key.L; break;
                case Keys.M:        info.Key = Key.M; break;
                case Keys.N:        info.Key = Key.N; break;
                case Keys.O:        info.Key = Key.O; break;
                case Keys.P:        info.Key = Key.P; break;
                case Keys.Q:        info.Key = Key.Q; break;
                case Keys.R:        info.Key = Key.R; break;
                case Keys.S:        info.Key = Key.S; break;
                case Keys.T:        info.Key = Key.T; break;
                case Keys.U:        info.Key = Key.U; break;
                case Keys.V:        info.Key = Key.V; break;
                case Keys.W:        info.Key = Key.W; break;
                case Keys.X:        info.Key = Key.X; break;
                case Keys.Y:        info.Key = Key.Y; break;
                case Keys.Z:        info.Key = Key.Z; break;

                case Keys.D0:       info.Key = Key.Digit0; break;
                case Keys.D1:       info.Key = Key.Digit1; break;
                case Keys.D2:       info.Key = Key.Digit2; break;
                case Keys.D3:       info.Key = Key.Digit3; break;
                case Keys.D4:       info.Key = Key.Digit4; break;
                case Keys.D5:       info.Key = Key.Digit5; break;
                case Keys.D6:       info.Key = Key.Digit6; break;
                case Keys.D7:       info.Key = Key.Digit7; break;
                case Keys.D8:       info.Key = Key.Digit8; break;
                case Keys.D9:       info.Key = Key.Digit9; break;

                case Keys.Space:    info.Key = Key.Space; break;

                case Keys.Enter:    info.Key = Key.Enter; break;
                case Keys.Tab:      info.Key = Key.Tab; break;
                case Keys.Back:     info.Key = Key.Backspace; break;
                case Keys.Delete:   info.Key = Key.Delete; break;
                case Keys.Escape:   info.Key = Key.Escape; break;

                case Keys.OemSemicolon: info.Key = Key.Semicolon; break;
                case Keys.Oemcomma: info.Key = Key.Comma; break;
                case Keys.OemPeriod: info.Key = Key.Period; break;
                case Keys.OemQuestion: info.Key = Key.Slash; break;

                case Keys.F1: info.Key = Key.F1; break;
                case Keys.F2: info.Key = Key.F2; break;
                case Keys.F3: info.Key = Key.F3; break;
                case Keys.F4: info.Key = Key.F4; break;
                case Keys.F5: info.Key = Key.F5; break;
                case Keys.F6: info.Key = Key.F6; break;
                case Keys.F7: info.Key = Key.F7; break;
                case Keys.F8: info.Key = Key.F8; break;
                case Keys.F9: info.Key = Key.F9; break;
                case Keys.F10: info.Key = Key.F10; break;
                case Keys.F11: info.Key = Key.F11; break;
                case Keys.F12: info.Key = Key.F12; break;
                case Keys.F13: info.Key = Key.F13; break;
                case Keys.F14: info.Key = Key.F14; break;
                case Keys.F15: info.Key = Key.F15; break;
                case Keys.F16: info.Key = Key.F16; break;
                case Keys.F17: info.Key = Key.F17; break;
                case Keys.F18: info.Key = Key.F18; break;
                case Keys.F19: info.Key = Key.F19; break;
                case Keys.F20: info.Key = Key.F20; break;
                case Keys.F21: info.Key = Key.F21; break;
                case Keys.F22: info.Key = Key.F22; break;
                case Keys.F23: info.Key = Key.F23; break;
                case Keys.F24: info.Key = Key.F24; break;

                default: recognized = false; break;
            }

            info.Shift = args.Shift;
            info.Control = args.Control;

            return recognized;
        }

        private void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Quitting != null) Quitting(this, EventArgs.Empty);
        }

        private void TerminalControl_KeyDown(object sender, KeyEventArgs e)
        {
            KeyInfo info;

            if (ConvertToKeyInfo(e, out info))
            {
                info.Down = true;
                e.Handled = true;

                switch (mListener)
                {
                    case KeyListener.Screen:
                        if (CurrentScreen != null)
                        {
                            CurrentScreen.ScreenKeyPress(info);
                        }
                        break;

                    case KeyListener.ReadKey:
                    case KeyListener.CheckForCancel:
                        mLastKey = info;
                        break;
                }
            }
        }

        private void TerminalControl_KeyUp(object sender, KeyEventArgs e)
        {
            KeyInfo info;

            if (ConvertToKeyInfo(e, out info))
            {
                info.Down = false;
                e.Handled = true;

                switch (mListener)
                {
                    case KeyListener.Screen:
                        if (CurrentScreen != null)
                        {
                            CurrentScreen.ScreenKeyPress(info);
                        }
                        break;

                    case KeyListener.ReadKey:
                    case KeyListener.CheckForCancel:
                        // do nothing. only cancel on key down
                        break;
                }
            }
        }

        private enum KeyListener
        {
            Screen,
            CheckForCancel,
            ReadKey
        }

        private TerminalForm mForm;
        private readonly Stack<Screen> mScreens = new Stack<Screen>();

        private KeyListener mListener;
        private KeyInfo? mLastKey;
    }
}
