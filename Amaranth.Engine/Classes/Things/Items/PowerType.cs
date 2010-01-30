using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// Represents a type of magical power that can be added to an <see cref="Item"/>.
    /// </summary>
    public class PowerType : ContentType
    {
        public object Appearance { get; private set; }

        /// <summary>
        /// Gets whether or not the name of the power comes before the item name.
        /// </summary>
        public bool IsPrefix { get; private set; }

        /// <summary>
        /// Gets the strike bonus provided by this PowerType.
        /// </summary>
        public Roller StrikeBonus { get; set; }

        /// <summary>
        /// Gets the damage bonus provided by this PowerType. Note that
        /// the damage bonus here is represented by tenths (i.e. 10
        /// means no damage bonus). Elsewhere, it is a straight multiplier.
        /// </summary>
        public Roller DamageBonus  { get; set; }

        /// <summary>
        /// Gets the armor bonus provided by this PowerType.
        /// </summary>
        public Roller ArmorBonus { get; set; }

        /// <summary>
        /// Gets the Stat bonus provided by this PowerType.
        /// </summary>
        public Roller StatBonus { get; set; }

        /// <summary>
        /// Gets the Speed bonus provided by this PowerType.
        /// </summary>
        public Roller SpeedBonus { get; set; }

        /// <summary>
        /// Gets the Elemental brand provided by this PowerType.
        /// </summary>
        public Element? Element { get; set; }

        public FlagCollection Flags { get; private set; }

        public ReadOnlyCollection<string> Categories
        {
            get { return new ReadOnlyCollection<string>(mCategories); }
        }

        public PowerType(Content content, string name, bool isPrefix, IEnumerable<string> categories, object appearance)
            : base(content)
        {
            mName = name;
            IsPrefix = isPrefix;

            mCategories.AddRange(categories);
            Appearance = appearance;

            // default to no bonus
            StrikeBonus = Roller.Fixed(0);
            DamageBonus = Roller.Fixed(0);
            ArmorBonus = Roller.Fixed(0);
            StatBonus = Roller.Fixed(0);
            SpeedBonus = Roller.Fixed(0);

            Element = null;

            Flags = new FlagCollection();
        }

        #region INamed Members

        public override string Name { get { return mName; } }

        #endregion

        private string mName;
        private readonly List<string> mCategories = new List<string>();
    }
}
