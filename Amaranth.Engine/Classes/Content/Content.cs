using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// Contains all of the data-derived objects needed by the Game.
    /// </summary>
    public class Content
    {
        /// <summary>
        /// Gets all of the <see cref="HeroRace"/> instances.
        /// </summary>
        public IList<HeroRace> HeroRaces { get { return mHeroRaces; } }

        /// <summary>
        /// Gets all of the <see cref="Monster"/> <see cref="Race"/> instances.
        /// </summary>
        public RaceChooser Races { get { return mRaces; } }

        /// <summary>
        /// Gets all of the <see cref="ItemType"/> instances.
        /// </summary>
        public IList<ItemType> Items { get { return mItems; } }

        /// <summary>
        /// Gets all of the <see cref="PowerType"/> instances.
        /// </summary>
        public PowerChooser Powers { get { return mPowers; } }

        /// <summary>
        /// Gets all of the <see cref="StoreType"/> instances.
        /// </summary>
        public IList<StoreType> Stores { get { return mStores; } }

        /// <summary>
        /// Gets drop for creating a dungeon feature.
        /// </summary>
        public IDrop<CreateFeature> Features { get { return mFeatures; } }

        public void SetFeatures(IDrop<CreateFeature> drop)
        {
            mFeatures = drop;
        }

        private readonly RaceChooser mRaces = new RaceChooser();
        private readonly List<HeroRace> mHeroRaces = new List<HeroRace>();
        private readonly List<ItemType> mItems = new List<ItemType>();
        private readonly PowerChooser mPowers = new PowerChooser();
        private readonly List<StoreType> mStores = new List<StoreType>();
        private IDrop<CreateFeature> mFeatures;
    }
}
