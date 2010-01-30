using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;
using Amaranth.Terminals;
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
            int startIndex = Math.Max(mLog.Entries.Count - terminal.Height, 0);
            for (int index = startIndex; index < mLog.Entries.Count; index++)
            {
                WriteLog(terminal, mLog.Entries[index]);
            }
        }

        private void WriteLog(ITerminal terminal, LogEntry entry)
        {
            using (new DisposableTerminalState(terminal))
            {
                switch (entry.Type)
                {
                    case LogType.Good:          terminal.State.ForeColor = TerminalColors.Green; break;
                    case LogType.Bad:           terminal.State.ForeColor = TerminalColors.Red; break;
                    case LogType.PermanentGood: terminal.State.ForeColor = TerminalColors.Gold; break;
                    case LogType.TemporaryGood: terminal.State.ForeColor = TerminalColors.Blue; break;
                    case LogType.WearOff:       terminal.State.ForeColor = TerminalColors.DarkCyan; break;
                    case LogType.Resist:        terminal.State.ForeColor = TerminalColors.Cyan; break;
                    case LogType.BadState:      terminal.State.ForeColor = TerminalColors.Orange; break;
                    case LogType.DidNotWork:    terminal.State.ForeColor = TerminalColors.Yellow; break;
                    case LogType.Fail:          terminal.State.ForeColor = TerminalColors.Gray; break;
                    case LogType.Special:       terminal.State.ForeColor = TerminalColors.Purple; break;
                    default:                    terminal.State.ForeColor = TerminalColors.White; break;
                }

                string text = entry.Text;

                // add the repeat count
                if (entry.Count > 1)
                {
                    text += " (x" + entry.Count.ToString() + ")";
                }

                foreach (string line in Text.WordWrap(text, terminal.Width))
                {
                    terminal.Scroll(0, -1, pos => new Character(Glyph.Space));
                    terminal[0, -1].Write(line);
                }
            }
        }

        private Log mLog;
    }
}
