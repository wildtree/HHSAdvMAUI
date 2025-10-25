// Graphics library for SDL2-CS
//

using SkiaSharp;
using SkiaSharp.Views.Maui.Controls.Hosting;


namespace HHSAdvMAUI
{
    public class Canvas
    {
        private class Point
        {
            public int x { get; set; }
            public int y { get; set; }
            public Point(int x, int y) { this.x = x; this.y = y; }
        }

        public SKBitmap Bitmap { get; private set; }
        private Rect viewport;
        private readonly SKColor[] palette = {
            new SKColor(0xff000000), // black
            new SKColor(0xff0000ff), // blue
            new SKColor(0xffff0000), // red
            new SKColor(0xffff00ff), // magenta
            new SKColor(0xff00ff00), // green
            new SKColor(0xff00ffff), // cyan
            new SKColor(0xffffff00), // yellow
            new SKColor(0xffffffff), // white
        };

        private static readonly float[] blueFilter = {
            0.0F, 0.0F, 0.1F,
            0.0F, 0.0F, 0.2F,
            0.0F, 0.0F, 0.7F,
        };

        private readonly float[] redFilter = {
            0.7F, 0.0F, 0.0F,
            0.2F, 0.0F, 0.0F,
            0.1F, 0.0F, 0.0F,
        };

        private readonly float[] sepiaFilter = {
            0.269021F, 0.527950F, 0.103030F,
            0.209238F, 0.410628F, 0.080135F,
            0.119565F, 0.234644F, 0.045791F,
        };


        public Canvas()
        {
            //var dpi = VisualTreeHelper.GetDpi(System.Windows.Application.Current.MainWindow);
            Bitmap = new SKBitmap(256, 152, SKColorType.Rgba8888, SKAlphaType.Premul);
            viewport = new Rect { X = 0, Y = 0, Width = 256, Height = 152 };
        }
        public void Cls(SKColor c)
        {
            for (int y = 0; y < viewport.Height; y++)
            {
                for (int x = 0; x < viewport.Width; x++)
                {
                    Bitmap.SetPixel(x, y, c);
                }
            }
    }

        private int GetColorIndex(SKColor c) =>
            ((c.Blue == 0) ? 0 : 1) + ((c.Red == 0) ? 0 : 2) + ((c.Green == 0) ? 0 : 4);
        public void Draw()
        {
        }
        public void Cls()
        {
            Cls(palette[0]);
        }
        public void Invalidate() => Draw();

        public void pset(int x, int y, SKColor c)
        {
            if (x >= viewport.Width || y >= viewport.Height || x < 0 || y < 0) return;
            Bitmap.SetPixel(x, y, c);
        }
        public void pset(int x, int y, int col) => pset(x, y, palette[col]);

        public SKColor pget(int x, int y)
        {
            if (x >= viewport.Width || y >= viewport.Height || x < 0 || y < 0) return palette[0];
            SKColor c = Bitmap.GetPixel(x, y);
            return c;
        }
        public void line(int x1, int y1, int x2, int y2, SKColor col)
        {
            int dx, ddx, dy, ddy;
            int wx, wy;
            int x, y;
            dy = y2 - y1;
            ddy = 1;
            if (dy < 0)
            {
                dy = -dy;
                ddy = -1;
            }
            wy = dy / 2;
            dx = x2 - x1;
            ddx = 1;
            if (dx < 0)
            {
                dx = -dx;
                ddx = -1;
            }
            wx = dx / 2;
            if (dx == 0 && dy == 0)
            {
                pset(x1, y1, col);
                return;
            }
            if (dy == 0)
            {
                for (x = x1; x != x2; x += ddx) pset(x, y1, col);
                pset(x2, y1, col);
                return;
            }
            if (dx == 0)
            {
                for (y = y1; y != y2; y += ddy) pset(x1, y, col);
                pset(x1, y2, col);
                return;
            }
            pset(x1, y1, col);
            if (dx > dy)
            {
                y = y1;
                for (x = x1; x != x2; x += ddx)
                {
                    pset(x, y, col);
                    wx -= dy;
                    if (wx < 0)
                    {
                        wx += dx;
                        y += ddy;
                    }
                }
            }
            else
            {
                x = x1;
                for (y = y1; y != y2; y += ddy)
                {
                    pset(x, y, col);
                    wy -= dx;
                    if (wy < 0)
                    {
                        wy += dy;
                        x += ddx;
                    }
                }
            }
            pset(x2, y2, col);
        }

        public void line(int x1, int y1, int x2, int y2, int col) => line(x1, y1, x2, y2, palette[col]);

        public void paint(int x, int y, SKColor f, SKColor b)
        {
            int l, r;
            int wx;
            Queue<Point> q = new Queue<Point>();
            SKColor c = pget(x, y);
            if (c.Equals(f) || c.Equals(b))
            {
                return;
            }
            q.Enqueue(new Point(x, y));
            while (q.Count > 0)
            {
                Point p = q.Dequeue();
                c = pget(p.x, p.y);
                if (c.Equals(f) || c.Equals(b)) continue;
                for (l = p.x - 1; l >= 0; l--)
                {
                    c = pget(l, p.y);
                    if (c.Equals(f) || c.Equals(b)) break;
                }
                ++l;
                for (r = p.x + 1; r < viewport.Width; r++)
                {
                    c = pget(r, p.y);
                    if (c.Equals(f) || c.Equals(b)) break;
                }
                --r;
                line(l, p.y, r, p.y, f);
                for (wx = l; wx <= r; wx++)
                {
                    int uy = p.y - 1;
                    if (uy >= 0)
                    {
                        c = pget(wx, uy);
                        if (!c.Equals(f) && !c.Equals(b))
                        {
                            if (wx == r)
                            {
                                q.Enqueue(new Point(wx, uy));
                            }
                            else
                            {
                                c = pget(wx + 1, uy);
                                if (c.Equals(f) || c.Equals(b)) q.Enqueue(new Point(wx, uy));
                            }
                        }
                    }
                    int ly = p.y + 1;
                    if (ly < viewport.Height)
                    {
                        c = pget(wx, ly);
                        if (!c.Equals(f) && !c.Equals(b))
                        {
                            if (wx == r)
                            {
                                q.Enqueue(new Point(wx, ly));
                            }
                            else
                            {
                                c = pget(wx + 1, ly);
                                if (c.Equals(f) || c.Equals(b)) q.Enqueue(new Point(wx, ly));
                            }
                        }
                    }
                }
            }
        }
        public void paint(int x, int y, int fc, int bc)
        {
            paint(x, y, palette[fc], palette[bc]);
        }
        public void tonePaint(byte[] tone, bool tiling = false)
        {
            SKColor[] pat = new SKColor[8];
            SKColor[] col = new SKColor[pat.Length];
            Array.Copy(palette, pat, pat.Length);
            Array.Copy(pat, col, pat.Length);
            int p = 0;
            int n = (int)tone[p++];
            for (int i = 1; i <= n; i++)
            {
                byte pb = tone[p++];
                byte pr = tone[p++];
                byte pg = tone[p++];
                pat[i] = new SKColor(green: pg, red: pr, blue: pb, alpha: 0xff);

                UInt32 r = 0, g = 0, b = 0;
                for (int bit = 0; bit < 8; bit++)
                {
                    byte mask = (byte)(1 << bit);
                    if ((pr & mask) != 0) r++;
                    if ((pg & mask) != 0) g++;
                    if ((pb & mask) != 0) b++;
                }
                if (r > 0) r = r * 32 - 1;
                if (g > 0) g = g * 32 - 1;
                if (b > 0) b = b * 32 - 1;
                col[i] = new SKColor(green: (byte)g, red: (byte)r, blue: (byte)b, alpha: 0xff);
            }
            for (int wy = 0; wy < viewport.Height; wy++)
            {
                for (int wx = 0; wx < viewport.Width; wx++)
                {
                    int ci = GetColorIndex(Bitmap.GetPixel(wx, wy));
                    if (ci > 0 && ci <= n)
                    {
                        Bitmap.SetPixel(wx, wy, col[ci]);
                    }
                }
            }
        }
        public enum FilterType
        {
            None,
            Blue,
            Red,
            Sepia
        };

        private FilterType filterType = FilterType.None;
        public FilterType ColorFilterType
        {
            get { return filterType; }
            set 
            {
                filterType = value;
                colorFilter();
            }
        }
        public void colorFilter()
        {
            float[] f;
            switch (ColorFilterType)
            {
                case FilterType.Blue:
                    f = blueFilter;
                    break;
                case FilterType.Red:
                    f = redFilter;
                    break;
                case FilterType.Sepia:
                    f = sepiaFilter;
                    break;
                default:
                    return;
            }

            for (int y = 0; y < viewport.Height; y++)
            {
                for (int x = 0; x < viewport.Width; x++)
                {
                    Bitmap.SetPixel(x, y, applyFilter(Bitmap.GetPixel(x, y) , f));
                }
            }
        }
        private SKColor applyFilter(SKColor c, float[] f)
        {
            UInt32 b = c.Blue;
            UInt32 r = c.Red;
            UInt32 g = c.Green;
            UInt32 nr = (UInt32)(r * f[0] + g * f[1] + b * f[2]);
            UInt32 ng = (UInt32)(r * f[3] + g * f[4] + b * f[5]);
            UInt32 nb = (UInt32)(r * f[6] + g * f[7] + b * f[8]);
            if (nr > 255) nr = 255;
            if (ng > 255) ng = 255;
            if (nb > 255) nb = 255;
            return new SKColor(green: (byte)ng, blue: (byte)nb, red: (byte)nr, alpha: 0xff);
        }
        public void drawRect(int x0, int y0, int x1, int y1, SKColor c)
        {
            line(x0, y0, x1, y0, c);
            line(x1, y0, x1, y1, c);
            line(x0, y1, x1, y1, c);
            line(x0, y1, x0, y0, c);
        }

        public void fillRect(int x0, int y0, int x1, int y1, SKColor c)
        {
            if (y0 > y1)
            {
                int y = y0;
                y0 = y1;
                y1 = y;
            }
            for (int y = y0; y <= y1; y++)
            {
                line(x0, y, x1, y, c);
            }
        }
        
        public SKColor GetPaletteColor(int i) => palette[i];
    }
}
