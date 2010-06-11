using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bramble.Core;
using Malison.Core;

using Amaranth.Util;
using Amaranth.UI;
using Amaranth.Engine;

namespace Amaranth.TermApp
{
    public class OverheadControl : RectControl
    {
        public Vec? Target
        {
            get { return mTarget; }
            set
            {
                if (mTarget != value)
                {
                    mTarget = value;
                    //### bob: repainting the whole control sucks
                    Repaint();
                }
            }
        }

        public TargetMode TargetMode
        {
            get { return mMode; }
            set
            {
                if (mMode != value)
                {
                    mMode = value;
                    //### bob: repainting the whole control sucks
                    Repaint();
                }
            }
        }

        public Game Game { get { return mGame; } }

        public OverheadControl(Game game, Rect bounds)
            : base(bounds)
        {
            mGame = game;

            ListenTo(mGame.Dungeon.Entities.EntityMoved, Game_EntityMoved);
            ListenTo(mGame.FloorChanged, Game_FloorChanged);

            ListenTo(mGame.Effects.ItemAdded, Effects_ItemAdded);
            ListenTo(mGame.Effects.ItemRemoved, Effects_ItemRemoved);

            ListenTo(mGame.Dungeon.TileChanged, Dungeon_TileChanged);

            ListenTo(mGame.Dungeon.Items.ItemAdded, Items_ItemAdded);
            ListenTo(mGame.Dungeon.Items.ItemRemoved, Items_ItemRemoved);

            ListenTo(mGame.Hero.Changed, Hero_Changed);
        }

        /// <summary>
        /// Called right before the game engine processes a turn. Lets
        /// the overhead control know that a visual transaction is
        /// beginning.
        /// </summary>
        public void StartGameUpdate()
        {
            mDirtyTiles.Clear();
            mPreviousCamera = mCamera;
        }

        /// <summary>
        /// Called after the game has processed a turn. Lets the overhead
        /// control refresh itself before the UI redraws.
        /// </summary>
        public void EndGameUpdate()
        {
            var terminal = Terminal;

            // scroll the view
            if (mCamera != mPreviousCamera)
            {
                terminal.Scroll(mPreviousCamera - mCamera,
                    pos => 
                        {
                            // convert screen to dungeon coordinates
                            var dungeonPos = pos + mCamera;

                            // since we're refreshing it now, we don't need to later
                            mDirtyTiles.Remove(dungeonPos);

                            // draw the tile
                            return GetTileCharacter(pos + mCamera);
                        });
            }

            // paint the dirty tiles
            foreach (var tile in mDirtyTiles.Keys)
            {
                Vec screenTile = tile - mCamera;

                // make sure the tile is onscreen
                if (terminal.Size.Contains(screenTile))
                {
                    RefreshScreenTile(screenTile, terminal);
                }
            }
        }

        protected override void Init()
        {
            base.Init();

            mCamera = new Vec();
            CenterOn(Terminal, mGame.Hero.Position);
        }

        protected override void OnPaint(ITerminal terminal)
        {
            Refresh(terminal);
        }

        private void Refresh(ITerminal terminal)
        {
            foreach (Vec screenTile in new Rect(Bounds.Size))
            {
                RefreshScreenTile(screenTile, terminal);
            }
        }

        private void RefreshScreenTile(Vec screenTile, ITerminal terminal)
        {
            //### bob: awful hack. painting shouldn't even be possible if the control isn't visible
            // don't paint if not visible
            if (!Visible) return;

            Vec dungeonTile = screenTile + mCamera;

            Character character = GetTileCharacter(dungeonTile);

            if (mTarget.HasValue)
            {
                Vec target = mTarget.Value;

                switch (mMode)
                {
                    case TargetMode.Direction:
                        if (dungeonTile == target + Direction.N) character = new Character(Glyph.ArrowUp, TermColor.Yellow, TermColor.Black);
                        else if (dungeonTile == target + Direction.NE) character = new Character(Glyph.Slash, TermColor.Yellow, TermColor.Black);
                        else if (dungeonTile == target + Direction.E) character = new Character(Glyph.ArrowRight, TermColor.Yellow, TermColor.Black);
                        else if (dungeonTile == target + Direction.SE) character = new Character(Glyph.Backslash, TermColor.Yellow, TermColor.Black);
                        else if (dungeonTile == target + Direction.S) character = new Character(Glyph.ArrowDown, TermColor.Yellow, TermColor.Black);
                        else if (dungeonTile == target + Direction.SW) character = new Character(Glyph.Slash, TermColor.Yellow, TermColor.Black);
                        else if (dungeonTile == target + Direction.W) character = new Character(Glyph.ArrowLeft, TermColor.Yellow, TermColor.Black);
                        else if (dungeonTile == target + Direction.NW) character = new Character(Glyph.Backslash, TermColor.Yellow, TermColor.Black);
                        break;

                    case TargetMode.Absolute:
                        if (dungeonTile == target + Direction.NE) character = new Character(Glyph.BarDownLeft, TermColor.Yellow, TermColor.Black);
                        else if (dungeonTile == target + Direction.SE) character = new Character(Glyph.BarUpLeft, TermColor.Yellow, TermColor.Black);
                        else if (dungeonTile == target + Direction.SW) character = new Character(Glyph.BarUpRight, TermColor.Yellow, TermColor.Black);
                        else if (dungeonTile == target + Direction.NW) character = new Character(Glyph.BarDownRight, TermColor.Yellow, TermColor.Black);
                        break;

                    case TargetMode.Entity:
                        if (dungeonTile == target + Direction.N) character = new Character(Glyph.BarUpLeftRight, TermColor.Yellow, TermColor.Black);
                        else if (dungeonTile == target + Direction.NE) character = new Character(Glyph.BarDownLeft, TermColor.Yellow, TermColor.Black);
                        else if (dungeonTile == target + Direction.E) character = new Character(Glyph.BarUpDownRight, TermColor.Yellow, TermColor.Black);
                        else if (dungeonTile == target + Direction.SE) character = new Character(Glyph.BarUpLeft, TermColor.Yellow, TermColor.Black);
                        else if (dungeonTile == target + Direction.S) character = new Character(Glyph.BarDownLeftRight, TermColor.Yellow, TermColor.Black);
                        else if (dungeonTile == target + Direction.SW) character = new Character(Glyph.BarUpRight, TermColor.Yellow, TermColor.Black);
                        else if (dungeonTile == target + Direction.W) character = new Character(Glyph.BarUpDownLeft, TermColor.Yellow, TermColor.Black);
                        else if (dungeonTile == target + Direction.NW) character = new Character(Glyph.BarDownRight, TermColor.Yellow, TermColor.Black);
                        break;
                }
            }

            /*
            // highlight it if targeted
            if (mTarget.HasValue && (mTarget.Value == dungeonTile))
            {
                // swap the fore and back colors
                character = new Character(character.Glyph, character.BackColor, character.ForeColor);
            }
            */

            terminal[screenTile].Write(character);
        }

        private Character GetTileCharacter(Vec dungeonTile)
        {
            if (mGame.Dungeon.Bounds.Contains(dungeonTile))
            {
                Tile tile = mGame.Dungeon.Tiles[dungeonTile];

                // effects and entities must be on a visible tile
                if (tile.IsVisible)
                {
                    Effect effect = mGame.Effects.GetAt(dungeonTile);
                    if (effect != null)
                    {
                        // show the effect
                        return GameArt.Get(effect);
                    }

                    // entities must be a on a lit tile
                    if (tile.IsLit)
                    {
                        Entity entity = mGame.Dungeon.Entities.GetAt(dungeonTile);
                        if (entity != null)
                        {
                            // show the entity
                            return GameArt.Get(entity);
                        }
                    }
                }

                // items must be seen
                if (tile.IsExplored)
                {
                    Item item = mGame.Dungeon.Items.GetAt(dungeonTile);

                    if (item != null)
                    {
                        return GameArt.Get(item);
                    }
                }

                // no entity, so show the dungeon tile
                return GameArt.Get(tile);
            }

            // outside the dungeon
            return new Character(Glyph.Space);
        }

        private void RefreshDungeonTile(Vec dungeonTile, ITerminal terminal)
        {
            // mark the tile as dirty. we'll refresh it at the end of the transaction.
            mDirtyTiles[dungeonTile] = true;
        }

        private void CenterOn(ITerminal terminal, Vec position)
        {
            // center the position in the terminal
            Vec halfView = terminal.Size / 2;
            mCamera = position - halfView;
        }

        private void Game_EntityMoved(Entity entity, ValueChangeEventArgs<Vec> e)
        {
            // in case the hero moved
            if (entity is Hero)
            {
                CenterOn(Terminal, entity.Position);
            }

            RefreshDungeonTile(e.Old, Terminal);
            RefreshDungeonTile(e.New, Terminal);
        }

        private void Game_FloorChanged(object obj, EventArgs e)
        {
            // in case the hero moved
            CenterOn(Terminal, mGame.Hero.Position);

            //### bob: wait until transaction ends to refresh?
            Refresh(Terminal);
        }

        private void Effects_ItemAdded(Effect effect, EventArgs e)
        {
            // refresh the tile to draw it
            RefreshDungeonTile(effect.Position, Terminal);
        }

        private void Effects_ItemRemoved(Effect effect, EventArgs e)
        {
            // refresh the tile to erase it
            RefreshDungeonTile(effect.Position, Terminal);
        }

        private void Items_ItemAdded(Item item, EventArgs e)
        {
            // refresh the tile to draw it
            RefreshDungeonTile(item.Position, Terminal);
        }

        private void Items_ItemRemoved(Item item, EventArgs e)
        {
            // refresh the tile to erase it
            RefreshDungeonTile(item.Position, Terminal);
        }

        private void Dungeon_TileChanged(Dungeon dungeon, TileEventArgs e)
        {
            // refresh the tile to erase it
            RefreshDungeonTile(e.Position, Terminal);
        }

        private void Hero_Changed(Entity entity, EventArgs e)
        {
            // refresh the hero
            RefreshDungeonTile(entity.Position, Terminal);
        }

        private Game mGame;
        private Vec mCamera;
        private Vec mPreviousCamera;
        private Vec? mTarget;
        private TargetMode mMode;

        private readonly Dictionary<Vec, bool> mDirtyTiles = new Dictionary<Vec,bool>();
    }
}
