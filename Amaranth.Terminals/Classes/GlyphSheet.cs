using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace Amaranth.Terminals
{
    public class GlyphSheet
    {
        public static GlyphSheet Terminal6x10 { get { return LazyCreate(ref sTerminal6x10, Properties.Resources.Terminal6x10); } }
        public static GlyphSheet Terminal7x10 { get { return LazyCreate(ref sTerminal7x10, Properties.Resources.Terminal7x10); } }
        public static GlyphSheet Terminal8x12 { get { return LazyCreate(ref sTerminal8x12, Properties.Resources.Terminal8x12); } }
        public static GlyphSheet Terminal10x12 { get { return LazyCreate(ref sTerminal10x12, Properties.Resources.Terminal10x12); } }
        
        private static GlyphSheet LazyCreate(ref GlyphSheet sheet, Bitmap bitmap)
        {
            if (sheet == null)
            {
                sheet = new GlyphSheet(bitmap);
            }

            return sheet;
        }

        public int Width { get { return mBitmap.Width / GlyphsPerRow; } }
        public int Height { get { return mBitmap.Height / GlyphsRows; } }

        public Bitmap GetBitmap(Character character)
        {
            // use the previously cached one if there
            Bitmap bitmap = null;
            if (mCharacterCache.TryGetValue(character, out bitmap))
            {
                return bitmap;
            }

            // not there, so create it
            Bitmap characterBitmap = new Bitmap(Width, Height);
            using (Graphics g = Graphics.FromImage(characterBitmap))
            {
                byte glyph = (byte)character.Glyph;
                int column = glyph % GlyphsPerRow;
                int row = glyph / GlyphsPerRow;

                Rectangle destRect = new Rectangle(0, 0, Width, Height);

                ColorMap map = new ColorMap();
                map.OldColor = Color.Black;
                map.NewColor = character.ForeColor;

                ImageAttributes attributes = new ImageAttributes();
                attributes.SetRemapTable(new ColorMap[] { map });

                g.DrawImage(mBitmap, destRect,
                    column * Width, row * Height, Width, Height,
                    GraphicsUnit.Pixel, attributes);
            }

            // cache it
            mCharacterCache[character] = characterBitmap;

            return characterBitmap;
        }

        public void Draw(Graphics g, int x, int y, Character character)
        {
            // don't draw if it's a blank glyph
            if (character.Glyph == Glyph.Space) return;

            Bitmap characterBitmap = GetBitmap(character);

            Rectangle destRect = new Rectangle(x, y, Width, Height);
            g.DrawImageUnscaledAndClipped(characterBitmap, destRect);
        }

        private GlyphSheet(Bitmap bitmap)
        {
            mBitmap = bitmap;
            mCharacterCache = new Dictionary<Character, Bitmap>();
        }

        private const int GlyphsPerRow = 32;
        private const int GlyphsRows = 6;

        private static GlyphSheet sTerminal6x10;
        private static GlyphSheet sTerminal7x10;
        private static GlyphSheet sTerminal8x12;
        private static GlyphSheet sTerminal10x12;

        private Bitmap mBitmap;
        private Dictionary<Character, Bitmap> mCharacterCache;
    }
}
