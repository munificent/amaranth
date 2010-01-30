using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Amaranth.Util;
using Amaranth.Engine;

namespace Amaranth.Tools.Dungeons
{
    public partial class DungeonView : UserControl
    {
        public Dungeon Dungeon { get { return mDungeon; } }

        public DungeonView()
        {
            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        public void SetDungeon(Dungeon dungeon)
        {
            mDungeon = dungeon;

            Invalidate();
        }

        public void InvalidateTile(Vec pos)
        {
            Rectangle rect = new Rectangle(pos.X * TileSize, pos.Y * TileSize, TileSize, TileSize);

            Invalidate(rect);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (mDungeon != null)
            {
                foreach (Vec pos in mDungeon.Bounds)
                {
                    Rectangle rect = new Rectangle(pos.X * TileSize, pos.Y * TileSize, TileSize, TileSize);

                    if (e.ClipRectangle.IntersectsWith(rect))
                    {
                        Brush brush = Brushes.Black;

                        if (mDungeon.Entities.GetAt(pos) != null)
                        {
                            brush = Brushes.Red;
                        }
                        else if (mDungeon.Items.GetAt(pos) != null)
                        {
                            brush = Brushes.Blue;
                        }
                        else
                        {
                            switch (mDungeon.Tiles[pos].Type)
                            {
                                case TileType.Floor:
                                    if (mDungeon.Tiles[pos].IsLit)
                                    {
                                        brush = Brushes.LightYellow;
                                    }
                                    else
                                    {
                                        brush = Brushes.White;
                                    }
                                    break;

                                case TileType.LowWall: brush = Brushes.LightGray; break;
                                case TileType.Wall: brush = Brushes.DarkGray; break;
                                case TileType.DoorOpen: brush = Brushes.LightGreen; break;
                                case TileType.DoorClosed: brush = Brushes.Green; break;
                                case TileType.StairsDown: brush = Brushes.LightBlue; break;
                                case TileType.StairsUp: brush = Brushes.LightBlue; break;

                                default: brush = Brushes.Magenta; break;
                            }
                        }

                        e.Graphics.FillRectangle(brush, rect);
                        e.Graphics.DrawRectangle(Line, rect);
                    }
                }
            }
        }

        private static Pen Line = new Pen(Color.FromArgb(32, 0, 0, 0));

        private static Brush FloorFill = Brushes.White;
        private static Brush WallFill = new SolidBrush(Color.FromArgb(64, 64, 64));

        private const int TileSize = 4;

        private Dungeon mDungeon;
    }
}
