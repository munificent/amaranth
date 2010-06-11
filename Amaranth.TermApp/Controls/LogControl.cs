using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using Bramble.Core;
using Malison.Core;

using Amaranth.Util;
using Amaranth.UI;
using Amaranth.Engine;

namespace Amaranth.TermApp
{
    public class LogControl : RectControl
    {
        public LogControl(Log log, Rect bounds)
            : base(bounds)
        {
            mLog = log;

            RepaintOn(mLog.Logged);
        }

        protected override void OnPaint(ITerminal terminal)
        {
            terminal.Clear();

            // write the last few entries
            int startIndex = Math.Max(mLog.Entries.Count - terminal.Size.Y, 0);
            for (int index = startIndex; index < mLog.Entries.Count; index++)
            {
                WriteLog(terminal, mLog.Entries[index]);
            }
        }

        private void WriteLog(ITerminal terminal, LogEntry entry)
        {
            TermColor color = terminal.ForeColor;
            switch (entry.Type)
            {
                case LogType.Good:          color = TermColor.Green; break;
                case LogType.Bad:           color = TermColor.Red; break;
                case LogType.PermanentGood: color = TermColor.Gold; break;
                case LogType.TemporaryGood: color = TermColor.Blue; break;
                case LogType.WearOff:       color = TermColor.DarkCyan; break;
                case LogType.Resist:        color = TermColor.Cyan; break;
                case LogType.BadState:      color = TermColor.Orange; break;
                case LogType.DidNotWork:    color = TermColor.Yellow; break;
                case LogType.Fail:          color = TermColor.Gray; break;
                case LogType.Special:       color = TermColor.Purple; break;
                default:                    color = TermColor.White; break;
            }

            terminal = terminal[color].CreateWindow();

            string text = entry.Text;

            // add the repeat count
            if (entry.Count > 1)
            {
                text += " (x" + entry.Count.ToString() + ")";
            }

            foreach (string line in text.WordWrap(terminal.Size.X))
            {
                terminal.Scroll(0, -1, pos => new Character(Glyph.Space));
                terminal[0, -1].Write(line);
            }
        }

        private Log mLog;
    }
}
