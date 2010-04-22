using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

using Amaranth.Util;

[assembly: InternalsVisibleTo("Amaranth.Engine.Tests")]

namespace Amaranth.Engine
{
    [Serializable]
    public class Game
    {
        public const int MaxDepth = 100;

        public readonly GameEvent<object, EventArgs> FloorChanged = new GameEvent<object, EventArgs>();
        public readonly GameEvent<Store, EventArgs> StoreEntered = new GameEvent<Store, EventArgs>();

        /// <summary>
        /// Raises when something in the game, a <see cref="Item"/>, <see cref="Monster"/>, or otherwise
        /// has come to the <see cref="Hero"/>'s attention.
        /// </summary>
        public readonly GameEvent<Thing, EventArgs> ThingNoticed = new GameEvent<Thing, EventArgs>();

        public static string Version { get { return "0.0.0d"; } }

        public Content Content { get { return mContent; } }

        public Town Town { get { return mTown; } }
        public Dungeon Dungeon { get { return mDungeon; } }
        public Log Log { get { return mLog; } }

        public Hero Hero { get { return mHero; } }

        /// <summary>
        /// Gets the <see cref="Entity"/> who created the root <see cref="Action"/>  that
        /// is currently being processed. Will be <c>null</c> outside of the main Action processing loop.
        /// </summary>
        public Entity ActingEntity { get { return mActingEntity; } }

        public EffectCollection Effects { get { return mEffects; } }

        public int Depth { get { return mDepth; } }

        //### bob: move out of engine
        /// <summary>
        /// Gets the names of all of the saved Hero files that can be loaded.
        /// </summary>
        public static IEnumerable<string> Heroes
        {
            get
            {
                foreach (string path in Directory.GetFiles("Save", "*" + SaveFileExtension))
                {
                    yield return Path.GetFileNameWithoutExtension(path);
                }
            }
        }

        public Game(Hero hero, Content content)
        {
            mEffects = new EffectCollection();
            mLog = new Log();

            mTown = new Town();
            mTown.Init(content);

            mHero = hero;
            mContent = content;

            mDungeon = new Dungeon(this);

            mDepth = 0;

            //### bob: hack. assumes the hero is the only one whose light radius can change
            mHero.LightRadiusChanged += Hero_LightRadiusChanged;

            ChangeFloor(0);

            mLog.Write(LogType.Special, "Welcome to Amaranth.");

            mState = GameState.Playing;
        }

        public void InitDungeon(NotNull<Dungeon> dungeon)
        {
            mDungeon = dungeon;
            mState = GameState.Playing;
        }

        /// <summary>
        /// Gets whether or not a <see cref="Hero"/> with the given name can be loaded.
        /// </summary>
        /// <param name="name">Name of the Hero to load. Does not include path or file extension.</param>
        /// <returns><c>true</c> if a Hero with that name was found and can be loaded.</returns>
        public static bool CanLoad(string name)
        {
            string filePath = Path.Combine("Save", name + SaveFileExtension);

            return File.Exists(filePath);
        }

        /// <summary>
        /// Loads a previously saved game.
        /// </summary>
        /// <param name="name">Name of the hero to load.</param>
        /// <param name="content">Content to use with the game.</param>
        /// <returns>The loaded game or null if it could not be loaded.</returns>
        /// <remarks>
        /// <para>
        /// There are two kinds of data the game deals with: game data, and content.
        /// Game data describes a specific game: where the hero is, his equipment,
        /// what the dungeon looks like, etc. Content is the static data that makes
        /// the game data-driven: stats for monster races, item types, etc.
        /// </para>
        /// <para>
        /// Game data is what's saved and loaded by Game. Content is not stored in
        /// the save file. The tricky part is that game data freely references
        /// content. Each Item in the Hero's inventory (game data) references its
        /// ItemType (content). If we naively serialized everything, we'd get
        /// content too. To solve this, any time a game object references a content
        /// object, it does so through a ContentReference. ContentReference, at
        /// save time, just writes out the name of the referred to content. At load
        /// time, it will look up the actual referred to content object and store a
        /// real reference.
        /// </para>
        /// </remarks>
        public static Game Load(string name, Content content)
        {
            string filePath = Path.Combine("Save", name + SaveFileExtension);

            if (!File.Exists(filePath)) throw new FileNotFoundException("There is no game to load.");

            Game game = null;

            // load the file
            using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                // provide a streaming context that gives access to the game content
                var context = new StreamingContext(StreamingContextStates.File, content);
                var formatter = new BinaryFormatter(null, context);

                try
                {
                    game = (Game)formatter.Deserialize(stream);

                    // content is not serialized so bind it afterwards
                    game.mContent = content;
                }
                catch (SerializationException e)
                {
                    Console.WriteLine("Failed to load. Reason:");
                    Console.WriteLine(e.ToString());
                    //throw;
                }
            }

            return game;
        }

        public void Save()
        {
            if (mEffects.Count > 0) throw new InvalidOperationException("Cannot save while the game is processing.");

            // create the save directory
            if (!Directory.Exists("Save"))
            {
                Directory.CreateDirectory("Save");
            }

            // save the file
            using (FileStream stream = new FileStream(Path.Combine("Save", mHero.Name + SaveFileExtension), FileMode.Create))
            {
                IFormatter formatter = new BinaryFormatter();
                try
                {
                    formatter.Serialize(stream, this);
                }
                catch (SerializationException e)
                {
                    Console.WriteLine("Failed to save. Reason:");
                    Console.WriteLine(e.ToString());
                    //throw;
                }
            }
        }

        public GameResult Process()
        {
            // create the enumerator
            if (mProcessEnumerator == null)
            {
                mProcessEnumerator = CreateProcessEnumerable().GetEnumerator();
            }

            // advance one step
            if (!mProcessEnumerator.MoveNext())
            {
                // no more results to enumerate, game must have ended
                return new GameResult(GameResultFlags.GameOver | GameResultFlags.NeedsPause);
            }

            return mProcessEnumerator.Current;
        }

        /// <summary>
        /// Tells the Behavior of the last Entity that returned CheckForCancel to cancel
        /// what it's doing.
        /// </summary>
        public void CancelAction()
        {
            if (mCancelEntity != null)
            {
                mCancelEntity.Behavior.Cancel();

                // only cancel once
                mCancelEntity = null;
            }
        }

        public void Lose()
        {
            mState = GameState.Over;
        }

        public void ChangeFloor(int change)
        {
            SetFloor(mDepth + change);
        }

        public void SetFloor(int floor)
        {
            int change = floor - mDepth;
            mDepth = floor;

            mDungeon.Generate((change > 0), mDepth);

            mHero.NoteDepth(floor);

            FloorChanged.Raise(null, EventArgs.Empty);

            mDungeon.DirtyVisibility();
            mDungeon.RefreshView(this);
        }

        /// <summary>
        /// Creates an enumerable collection of the entire series of GameResults for the
        /// game.
        /// </summary>
        private IEnumerable<GameResult> CreateProcessEnumerable()
        {
            // used to process the items at slow speed
            Energy itemEnergy = new Energy(Energy.MinSpeed);

            while (mState == GameState.Playing)
            {
                for (int entityIndex = 0; entityIndex < mDungeon.Entities.Count; entityIndex++)
                {
                    Entity entity = mDungeon.Entities[entityIndex];

                    // make sure the entity has enough energy to move
                    // once it does, keep trying until it makes a successful action
                    while (entity.Energy.HasEnergy)
                    {
                        // bail if we need to wait for the ui to provide an action
                        while (entity.Behavior.NeedsUserInput)
                        {
                            mEffects.Clear();
                            yield return new GameResult(GameResultFlags.NeedsUserInput, entity);
                        }

                        // get the entity's actions
                        foreach (Action action in entity.TakeTurn())
                        {
                            // process it and everything it leads to
                            foreach (GameResult result in ProcessAction(action))
                            {
                                yield return result;
                            }
                        }
                    }

                    // entity was killed, so it will be removed from the collection and the next
                    // one shifted up.
                    if (!(entity is Hero) && !entity.IsAlive)
                    {
                        entityIndex--;
                    }
                }

                // process all of the items
                if (itemEnergy.HasEnergy)
                {
                    foreach (Item item in mDungeon.Items.Concat(mHero.Inventory).Concat(mHero.Equipment))
                    {
                        // skip null items because the hero's inventory and equipment can contain empty slots
                        if (item != null)
                        {
                            Action itemAction = item.TakeTurn(this);

                            if (itemAction != null)
                            {
                                // process it and everything it leads to
                                foreach (GameResult result in ProcessAction(itemAction))
                                {
                                    yield return result;
                                }
                            }
                        }
                    }

                    itemEnergy.Spend();
                }

                // a turn has completed, so give everything energy
                mDungeon.Entities.ForEach(entity => entity.Energy.Gain());
                itemEnergy.Gain();
            }
        }

        private IEnumerable<GameResult> ProcessAction(Action theAction)
        {
            Queue<Action> actions = new Queue<Action>();
            actions.Enqueue(theAction);

            while (actions.Count > 0)
            {
                // clear the effects
                mEffects.Clear();

                Action action = actions.Peek();

                // track who owns this sequence of actions
                mActingEntity = action.Entity;

                ActionResult result = action.Process(mEffects, actions);

                // cascade through the alternates until we hit the "real" action to process
                while (result.Alternate != null)
                {
                    result = result.Alternate.Process(mEffects, actions);
                }

                // remove it if complete
                if (result.IsDone)
                {
                    actions.Dequeue();
                }

                // run the post step
                if (result.Success)
                {
                    action.AfterSuccess();
                }

                mActingEntity = null;

                // make sure the lighting and visibility are up to date
                mDungeon.RefreshView(this);

                // assume we do not need to cancel
                mCancelEntity = null;

                if (result.NeedsPause)
                {
                    // return and pause
                    yield return new GameResult(GameResultFlags.NeedsPause);
                }
                else if (result.NeedsCheckForCancel)
                {
                    // return and check for a cancel
                    mCancelEntity = action.Entity;
                    yield return new GameResult(GameResultFlags.CheckForCancel);
                }
                else if (mEffects.Count > 0)
                {
                    // show the effects if there are any
                    yield return new GameResult(GameResultFlags.NeedsPause);
                }

                // if the game ended, just continue to return that infinitely
                while (mState == GameState.Over)
                {
                    yield return new GameResult(GameResultFlags.GameOver | GameResultFlags.NeedsPause);

                    mEffects.Clear();
                }
            }
        }

        private void Hero_LightRadiusChanged(object sender, EventArgs e)
        {
            mDungeon.DirtyLighting();
        }

        private enum GameState
        {
            Playing,
            Over
        }

        private const string SaveFileExtension = ".hero";

        private GameState mState;
        //### bob: move into dungeon
        private int mDepth;

        private Hero mHero;
        private Town mTown;
        private Dungeon mDungeon;

        private Log mLog;

        //### bob: why is this serialized?
        private EffectCollection mEffects;

        [NonSerialized]
        private IEnumerator<GameResult> mProcessEnumerator;

        [NonSerialized]
        private Entity mActingEntity;

        [NonSerialized]
        private Entity mCancelEntity;

        [NonSerialized]
        private Content mContent;
    }
}
