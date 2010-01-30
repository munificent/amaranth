using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    [Serializable]
    public class Log
    {
        public readonly GameEvent<Log, LogEventArgs> Logged = new GameEvent<Log, LogEventArgs>();

        public ReadOnlyCollection<LogEntry> Entries { get { return new ReadOnlyCollection<LogEntry>(mEntries); } }

        public Log()
        {
            mEntries = new List<LogEntry>();
        }

        public void Write(LogType type, string text)
        {
            // sentence case the text
            text = text.Substring(0, 1).ToUpper() + text.Substring(1);

            // see if it's a repeat entry
            if ((mEntries.Count > 0) && (mEntries[mEntries.Count - 1].Text == text))
            {
                // increment the count
                mEntries[mEntries.Count - 1].Count++;

                Logged.Raise(this, new LogEventArgs(mEntries[mEntries.Count - 1], true));
            }
            else
            {
                // new entry
                LogEntry entry = new LogEntry(type, text);
                mEntries.Add(entry);

                Logged.Raise(this, new LogEventArgs(entry, false));
            }
        }

        public void Message(string text)
        {
            Write(LogType.Message, text);
        }

        public void Good(string text)
        {
            Write(LogType.Good, text);
        }

        public void Bad(string text)
        {
            Write(LogType.Bad, text);
        }

        public void PermanentGood(string text)
        {
            Write(LogType.PermanentGood, text);
        }

        public void TemporaryGood(string text)
        {
            Write(LogType.TemporaryGood, text);
        }

        public void WearOff(string text)
        {
            Write(LogType.WearOff, text);
        }

        public void Resist(string text)
        {
            Write(LogType.Resist, text);
        }

        public void BadState(string text)
        {
            Write(LogType.BadState, text);
        }

        public void Fail(string text)
        {
            Write(LogType.Fail, text);
        }

        public void Write(string format, IDictionary<string, object> properties)
        {
            Message(format.FormatNames(properties));
        }

        private readonly List<LogEntry> mEntries;
    }

    //### bob: make a log type for fail-like things that do consume energy? like casting restore when
    // the stat is restored already?
    public enum LogType
    {
        /// <summary>
        /// Information that is very common or doesn't have any specific connotation:
        /// melee combat.
        /// </summary>
        Message,

        /// <summary>
        /// Basic good message: inflicted damage, etc.
        /// </summary>
        Good,

        /// <summary>
        /// Basic bad message: took damage.
        /// </summary>
        Bad,

        /// <summary>
        /// A permanent positive change to the hero has taken effect: leveling up,
        /// stat gain, etc.
        /// </summary>
        PermanentGood,

        /// <summary>
        /// A temporary positive change: healing, haste, etc.
        /// </summary>
        TemporaryGood,

        /// <summary>
        /// A temporary positive change or condition has ended.
        /// </summary>
        WearOff,

        /// <summary>
        /// An attack has been resisted: elemental resist, saving throw, etc.
        /// </summary>
        Resist,

        /// <summary>
        /// A negative change: poison, disease, etc.
        /// </summary>
        BadState,

        /// <summary>
        /// The attempt to do something has failed but energy was consumed. For example, using a restore potion when
        /// the Stat was not drained.
        /// </summary>
        DidNotWork,

        /// <summary>
        /// The attempt to do something has failed and no energy was consumed. For example, trying to walk into a wall.
        /// </summary>
        Fail,

        /// <summary>
        /// Something "out of game" has occurred.
        /// </summary>
        Special
    }

    [Serializable]
    public class LogEntry
    {
        public LogType Type;
        public string Text;
        public int Count;

        public LogEntry(LogType type, string text)
        {
            Type = type;
            Text = text;
            Count = 1;
        }
    }

    public class LogEventArgs : EventArgs
    {
        public LogEntry Entry { get { return mEntry; } }
        public bool CountChanged { get { return mCountChanged; } }

        public LogEventArgs(LogEntry entry, bool countChanged)
        {
            mEntry = entry;
            mCountChanged = countChanged;
        }

        private LogEntry mEntry;
        private bool mCountChanged;
    }
}
