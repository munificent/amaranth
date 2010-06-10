using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

using Bramble.Core;

using Amaranth.Util;

namespace Amaranth.Engine
{
    [Serializable]
    public class EntityCollection : IEnumerable<Entity>
    {
        public readonly GameEvent<Entity, ValueChangeEventArgs<Vec>> EntityMoved = new GameEvent<Entity, ValueChangeEventArgs<Vec>>();
        public readonly GameEvent<Entity, EventArgs> EntityAdded = new GameEvent<Entity, EventArgs>();
        public readonly GameEvent<Entity, CollectionItemEventArgs<Entity>> EntityRemoved = new GameEvent<Entity, CollectionItemEventArgs<Entity>>();

        public int Count { get { return mEntities.Count; } }

        public Dungeon Dungeon { get { return mDungeon; } }

        public Entity this[int index]
        {
            get { return mEntities[index]; }
        }

        public EntityCollection(Dungeon dungeon)
        {
            mDungeon = dungeon;
            mEntities = new List<Entity>();
        }

        public int IndexOf(Entity entity)
        {
            return mEntities.IndexOf(entity);
        }

        /// <summary>
        /// Removes all <see cref="Entity">Entities</see> from the collection. Does
        /// not raise events.
        /// </summary>
        public void Clear()
        {
            mEntities.Clear();
        }

        public void Add(Entity entity)
        {
            // add it to the collection and track its movement
            mEntities.Add(entity);

            ((ICollectible<EntityCollection, Entity>)entity).SetCollection(this);

            EntityAdded.Raise(entity, EventArgs.Empty);

            // raise an event as if the entity just moved there
            EntityMoved.Raise(entity, new ValueChangeEventArgs<Vec>(entity.Position, entity.Position));
        }

        public void ForEach(Action<Entity> action)
        {
            mEntities.ForEach(action);
        }

        public void Remove(Entity entity)
        {
            int index = IndexOf(entity);

            ((ICollectible<EntityCollection, Entity>)entity).SetCollection(null);

            mEntities.Remove(entity);

            EntityRemoved.Raise(entity, new CollectionItemEventArgs<Entity>(entity, index));
        }

        #region IEnumerable<Entity> Members

        public IEnumerator<Entity> GetEnumerator()
        {
            return mEntities.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        private readonly Dungeon mDungeon;
        private List<Entity> mEntities;
    }
}
