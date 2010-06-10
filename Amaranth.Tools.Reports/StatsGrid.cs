using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Bramble.Core;

using Amaranth.Util;

namespace Amaranth.Reports
{
    public partial class StatsGrid : UserControl
    {
        public int Gain
        {
            get { return mGain; }
            set
            {
                if (mGain != value)
                {
                    mGain = value;
                    Invalidate();
                }
            }
        }

        public StatsGrid()
        {
            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);

            UpdateStyles();
        }

        public void SetStats(IEnumerable<IStatRow> stats)
        {
            mStats = stats;

            // set the size
            int width = HeaderWidth + mStats.First().Values.Count() * CellWidth;

            int height = 0;
            if (mStats != null)
            {
                foreach (IStatRow row in mStats)
                {
                    height += row.IsTall ? TallRowHeight : RowHeight;
                }
            }

            ClientSize = new Size(width, height);

            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(Color.Black);

            if (mStats != null)
            {
                Pen linePen = new Pen(Color.FromArgb(15, 15, 15));

                int x = HeaderWidth - 1;
                for (int i = 0; i < 10; i++)
                {
                    e.Graphics.DrawLine(linePen, x, 0, x, ClientSize.Height);
                    x += CellWidth * 10;
                }

                int y = 0;
                foreach (IStatRow row in mStats)
                {
                    int height = row.IsTall ? TallRowHeight : RowHeight;

                    e.Graphics.DrawLine(linePen, 0, y + height - 1, ClientSize.Width, y + height - 1);

                    e.Graphics.DrawString(row.Name, Font, Brushes.Gray, 3, y - 1);

                    x = HeaderWidth;
                    int column = 0;
                    foreach (int cell in row.Values)
                    {
                        // normalize the value
                        int max = row.Max(column);
                        float normal = 0.0f;
                        if (max > 0)
                        {
                            normal = (cell * mGain).Clamp(0, max).Normalize(0, max);
                        }

                        if (row.IsTall)
                        {
                            // draw a line at the right height
                            int lineY = y + height - (int)(normal * height);
                            e.Graphics.DrawLine(Pens.White, x, lineY, x + CellWidth, lineY);
                        }
                        else
                        {
                            // pick the color of the value
                            Color color = Color.FromArgb((int)(normal * 255.0f), row.Color);
                            Brush brush = new SolidBrush(color);

                            e.Graphics.FillRectangle(brush, x, y, CellWidth - 1, height - 1);
                        }

                        x += CellWidth;
                        column++;
                    }

                    y += height;
                }
            }
        }

        private const int RowHeight = 12;
        private const int TallRowHeight = 200;
        private const int CellWidth = 10;
        private const int HeaderWidth = 150;

        private int mGain = 1;
        private IEnumerable<IStatRow> mStats;
    }
}
