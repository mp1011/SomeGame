namespace SomeGame.Main.Models
{
    record PixelPoint(PixelValue X, PixelValue Y)
    {
        public PixelPoint(int x, int y) : this(new PixelValue(x, 0), new PixelValue(y, 0)) { }
        public PixelPoint(PixelValue x, int y) : this(x, new PixelValue(y, 0)) { }
        public PixelPoint(int x, PixelValue y) : this(new PixelValue(x, 0), y) { }

        public PixelPoint Offset(int offX, int offY) => new PixelPoint(X + offX, Y + offY);

        public PixelPoint Offset(Orientation orientation, int offset)
        {
            if (orientation == Orientation.Horizontal)
                return new PixelPoint(X + offset, Y);
            else
                return new PixelPoint(X, Y + offset);
        }

        public PixelPoint Offset(Orientation orientation, int pixelOffset, byte subPixelOffset)
        {
            if (orientation == Orientation.Horizontal)
                return new PixelPoint(X + new PixelValue(pixelOffset, subPixelOffset), Y);
            else
                return new PixelPoint(X, Y + new PixelValue(pixelOffset, subPixelOffset));
        }
    }

    record PixelValue(int Pixel, int SubPixel)
    {
        public static PixelValue operator +(PixelValue pv, int i)
        {
            return new PixelValue(pv.Pixel + i, pv.SubPixel);
        }

        public static PixelValue operator +(PixelValue p1, PixelValue p2)
        {
            int newSubPixel = p1.SubPixel + p2.SubPixel;
            int newPixel = p1.Pixel + p2.Pixel;
            
            while(newSubPixel > 255)
            {
                newSubPixel -= 255;
                newPixel++;
            }

            while (newSubPixel < -255)
            {
                newSubPixel += 255;
                newPixel--;
            }

            return new PixelValue(newPixel, newSubPixel);
        }

        public static PixelValue operator *(PixelValue p, int multiple)
        {
            return new PixelValue(p.Pixel * multiple, p.SubPixel * multiple);
        }

        public static implicit operator int(PixelValue pv) => pv.Pixel;
        public static implicit operator PixelValue(int v) => new PixelValue(v,0);


    }

}
