using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// A simple move that just displays a sentence.
    /// </summary>
    public class MessageMove : Move
    {
        public override string Description
        {
            //### bob: using second person here is hackish.
            get { return "It can ^g" + Sentence.FixSubjectVerbAgreement(Info.Verb, Person.Second) + "^- on you."; }
        }

        public override bool WillUseMove(Monster monster, Entity target)
        {
            // only if next to target
            return monster.Position.IsAdjacentTo(target.Position);
        }

        public override Action GetAction(Monster monster, Entity target)
        {
            //### bob: make formatting more flexible. Text param in MoveInfo?
            return new MessageAction(monster, "{subject} " + Info.Verb + " {object}.", target);
        }

        public override float GetExperience(Race race)
        {
            // actually results in negative experience, since this move reduces the odds of other moves happening
            return 0;
        }
    }
}
