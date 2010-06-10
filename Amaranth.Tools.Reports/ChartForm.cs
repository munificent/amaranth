using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Bramble.Core;

using Amaranth.Engine;
using Amaranth.Util;

namespace Amaranth.Reports
{
    public partial class ChartForm : Form
    {
        public ChartForm()
        {
            InitializeComponent();

            mGraphs.Add(Plot(1));
            mGraphs.Add(Plot(3));
            mGraphs.Add(Plot(10));
            mGraphs.Add(Plot(20));
            mGraphs.Add(Plot(50));
            mGraphs.Add(Plot(90));
            mGraphs.Add(Plot(100));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            foreach (float[] graph in mGraphs)
            {
                for (int x = 0; x < 99; x++)
                {
                    Point pt1 = GraphToPixel(x, graph[x]);
                    Point pt2 = GraphToPixel(x + 1, graph[x + 1]);

                    e.Graphics.DrawLine(Pens.Blue, pt1, pt2);
                }
            }
        }

        private float[] Plot(int level)
        {
            float[] points = new float[100];

            for (int i = 0; i < 30000; i++)
            {
                int x = level;

                for (int j = 0; j < Math.Min(5, level); j++)
                {
                    x += Rng.TriangleInt(0, 1 + (level / 10));
                }

                /*
                int range = 2;
                for (int j = 0; j < level / 3; j++)
                {
                    x += Rng.Int(-range, range + 1);
                }
                */

                x = x.Clamp(0, points.Length - 1);
                points[x] += 0.01f;
            }

            return points;
        }

        private Point GraphToPixel(int x, float y)
        {
            x = x.Remap(0, 100, 0, ClientSize.Width);
            y = y.Remap(0, 100.0f, ClientSize.Height, 0);

            return new Point(x, (int)y);
        }

        private List<float[]> mGraphs = new List<float[]>();
    }
}
