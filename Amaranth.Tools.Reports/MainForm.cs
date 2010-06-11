using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Malison.Core;

using Amaranth.Util;
using Amaranth.Data;
using Amaranth.Engine;

namespace Amaranth.Reports
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            mContent = DataFiles.Load();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            StatsForm form = new StatsForm();
            form.ShowDialog(this);
        }

        private void chartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChartForm form = new ChartForm();
            form.ShowDialog(this);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            foreach (Race race in mContent.Races.Where(thisRace => thisRace.Drop != null))
            {
                Dictionary<string, int> dropped = new Dictionary<string, int>();

                for (int i = 0; i < 10000; i++)
                {
                    foreach (Item item in race.Drop.Create(race.Depth))
                    {
                        string name = item.ToString(ItemStringOptions.None);

                        if (!dropped.ContainsKey(name)) dropped[name] = 0;
                        dropped[name]++;
                    }
                }

                Console.WriteLine(race.Name);
                foreach (KeyValuePair<string, int> pair in dropped)
                {
                    Console.WriteLine("  {0,-5} {1}", pair.Value, pair.Key);
                }
            }
        }

        private Content mContent;
    }
}
