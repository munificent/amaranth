using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Engine
{
    public class EnterStoreAction : Action
    {
        public EnterStoreAction(Hero hero, TileType storeType)
            : base(hero)
        {
            mStoreType = storeType;
        }

        protected override ActionResult OnProcess()
        {
            // pick the store
            Store store = Game.Town.GetStore(mStoreType);

            Log(LogType.Message, "{subject} enter[s] {object}.", store);

            Game.StoreEntered.Raise(store, EventArgs.Empty);

            return ActionResult.Done;
        }

        private TileType mStoreType;
    }
}
