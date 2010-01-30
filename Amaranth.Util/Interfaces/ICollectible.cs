using System;
using System.Collections.Generic;
using System.Text;

namespace Amaranth.Util
{
    public interface ICollectible<TCollection, TItem> where TItem : ICollectible<TCollection, TItem>
    {
        void SetCollection(TCollection collection);
    }
}
