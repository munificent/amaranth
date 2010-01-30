using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Amaranth.Terminals
{
    public partial class TerminalForm : Form
    {
        public TerminalControl TerminalControl
        {
            get { return mTerminalControl; }
        }

        public ITerminal Terminal
        {
            get { return mTerminalControl.Terminal; }
            set
            {
                if (value == null) throw new ArgumentNullException("value");

                mTerminalControl.Terminal = value;
            }
        }

        public TerminalForm()
        {
            InitializeComponent();
        }

        public TerminalForm(ITerminal terminal)
            : this()
        {
            if (terminal == null) throw new ArgumentNullException("terminal");

            mTerminalControl.Terminal = terminal;
            ResizeToFitTerminal();
        }

        public TerminalForm(string text, int width, int height)
            : this(new Terminal(width, height))
        {
            Text = text;
        }

        private void ResizeToFitTerminal()
        {
            Size terminalSize = mTerminalControl.PreferredSize;
            ClientSize = new Size(terminalSize.Width, terminalSize.Height + mMenuStrip.Height);
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FontToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            m6x10ToolStripMenuItem.Checked = (mTerminalControl.GlyphSheet == GlyphSheet.Terminal6x10);
            m7x10ToolStripMenuItem.Checked = (mTerminalControl.GlyphSheet == GlyphSheet.Terminal7x10);
            m8x12ToolStripMenuItem.Checked = (mTerminalControl.GlyphSheet == GlyphSheet.Terminal8x12);
        }

        private void Font6x10ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mTerminalControl.GlyphSheet = GlyphSheet.Terminal6x10;
            ResizeToFitTerminal();
        }

        private void Font7x10ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            mTerminalControl.GlyphSheet = GlyphSheet.Terminal7x10;
            ResizeToFitTerminal();
        }

        private void Font8x12ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mTerminalControl.GlyphSheet = GlyphSheet.Terminal8x12;
            ResizeToFitTerminal();
        }

        private void Font10x12ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mTerminalControl.GlyphSheet = GlyphSheet.Terminal10x12;
            ResizeToFitTerminal();
        }
    }
}