using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Amaranth.Reports
{
    public class Chart
    {
        public int Width = 500;
        public int Height = 300;

        public IList<IList<int>> Values = new List<IList<int>>();

        public void AddValues()
        {
            Random r = new Random();

            for (int j = 0; j < 10; j++)
            {
                List<int> list = new List<int>();
                Values.Add(list);

                for (int i = 0; i < 100; i++)
                {
                    list.Add(r.Next(100));
                }
            }
        }

        public void Write(string path)
        {
            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.WriteLine("<html>");
                writer.WriteLine("<head>");
                writer.WriteLine("<title>Chart</title>");
                writer.WriteLine("</head>");

                writer.WriteLine("<body>");
                
                writer.WriteLine("<p>Here it is.</p>");

                //writer.WriteLine("<p><img src=\"http://chart.apis.google.com/chart?cht=p3&chd=s:hW&chs=250x100&chl=Hello|World\" /></p>");

                string chart = "http://chart.apis.google.com/chart?";

                // chart type
                chart += "cht=lc";
                chart += "&";

                // data
                chart += "chd=s:" + SimpleEncode(Values) + "&";

                // chart size
                chart += "chs=" + Width.ToString() + "x" + Height.ToString();
                chart += "&";

                // more stuff...
                chart += "chl=Hello|World";

                writer.WriteLine("<p><img src=\"" + chart + "\" /></p>");

                writer.WriteLine("</body>");
                writer.WriteLine("</html>");
            }
        }

        private string SimpleEncode(IList<IList<int>> values)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < values.Count; i++)
            {
                builder.Append(SimpleEncode(values[i]));
                if (i < values.Count - 1)
                {
                    builder.Append(",");
                }
            }

            return builder.ToString();
        }

        private string SimpleEncode(IList<int> values)
        {
            StringBuilder builder = new StringBuilder();

            // find the max
            int max = 0;
            foreach (int value in values)
            {
                max = Math.Max(value, max);
            }

            Console.WriteLine("max " + max);
            string encode = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            foreach (int value in values)
            {
                int normal = (61 * value) / max;

                builder.Append(encode[normal]);
            }

            return builder.ToString();
        }
    }
}
