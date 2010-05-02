using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Amaranth.Data;
using Amaranth.Util;
using Amaranth.Engine;

namespace Amaranth.Tools.Dungeons
{
    public partial class DungeonForm : Form
    {
        public DungeonForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            mContent = DataFiles.Load();

            mPropertyGrid.SelectedObject = new FeatureCreepGeneratorOptions();
        }

        private void MakeButton_Click(object sender, EventArgs e)
        {
            // seed it
            if (mReseedCheckBox.Checked)
            {
                int seed = Rng.Int(Int32.MaxValue);
                mSeedTextBox.Text = seed.ToString();
            }

            Rng.Seed(Int32.Parse(mSeedTextBox.Text));

            mDungeonView.SetDungeon(null);

            Game game = new Game(Hero.CreateTemp(), mContent);

            // fill the dungeon with default tiles
            game.Dungeon.Tiles.Fill((pos) => new Tile(TileType.Wall));

            IDungeonGenerator generator = new FeatureCreepGenerator();
            generator.Create(game.Dungeon, true, mLevelTrackBar.Value, mPropertyGrid.SelectedObject);

            mDungeonView.SetDungeon(game.Dungeon);
        }

        private void mLevelTrackBar_ValueChanged(object sender, EventArgs e)
        {
            mDepthLabel.Text = "Depth: " + mLevelTrackBar.Value.ToString();
        }

        private void mDungeonView_MouseMove(object sender, MouseEventArgs e)
        {
            int x = e.X / 4;
            int y = e.Y / 4;
            string type = String.Empty;
            string who = String.Empty;

            if ((mDungeonView.Dungeon != null) && mDungeonView.Dungeon.Bounds.Contains(new Vec(x, y)))
            {
                type = mDungeonView.Dungeon.Tiles[x, y].Type.ToString();

                var entity = mDungeonView.Dungeon.Entities.GetAt(new Vec(x, y));
                if (entity != null)
                {
                    who = entity.Name;
                }
            }

            mPosLabel.Text = String.Format("({0},{1}) {2} {3}", x, y, type, who);
        }

        private Content mContent;
    }
}
