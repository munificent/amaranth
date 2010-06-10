using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bramble.Core;

using Amaranth.Util;

namespace Amaranth.Engine
{
    public class BreedAction : DelegateAction
    {
        public BreedAction(Monster monster, BreedMoveInfo info)
            : base(monster)
        {
            SetCallback(
                () =>
                {
                    // try to find a place for the spawn
                    Vec pos;
                    if (Dungeon.TryFindOpenAdjacent(monster.Position, out pos))
                    {
                        // spawn a new monster
                        Monster child = new Monster(pos, monster.Race);

                        // increment this generation
                        info.Generation++;

                        // set the child's generation
                        BreedMoveInfo childInfo = child.GetMoveInstance<BreedMoveInfo>();
                        childInfo.Generation = info.Generation;

                        // add it to the dungeon
                        Dungeon.Entities.Add(child);
                    }

                    return ActionResult.Done;
                });
        }
    }
}
