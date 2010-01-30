using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Amaranth.Engine;
using Amaranth.Terminals;

namespace Amaranth.Reports
{
    public partial class StatsForm : Form
    {
        public StatsForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            mStats = new AsyncStats();
            mStats.Updated += Stats_Updated;

            statsGrid1.SetStats(mStats.StatRows);

            ClientSize = new Size(statsGrid1.Width + 20, ClientSize.Height);

            mStats.Run();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            mStats.Updated -= Stats_Updated;
            mStats.Stop();
        }

        void Stats_Updated(object sender, EventArgs e)
        {
            statsGrid1.Invalidate();
        }

        private AsyncStats mStats;

        private void toolStripDropDownButton1_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            int gain = Int32.Parse(e.ClickedItem.Tag.ToString());
            statsGrid1.Gain = gain;
        }

        private void totalMaxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            totalMaxToolStripMenuItem.Checked = true;
            maxPerLevelToolStripMenuItem.Checked = false;

            mStats.MaxPerLevel = false;
        }

        private void maxPerLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            totalMaxToolStripMenuItem.Checked = false;
            maxPerLevelToolStripMenuItem.Checked = true;

            mStats.MaxPerLevel = true;
        }
    }
}
