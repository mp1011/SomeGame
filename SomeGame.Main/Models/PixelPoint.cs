namespace SomeGame.Main.Models
{
    public record PixelPoint(PixelValue X, PixelValue Y)
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

        public PixelPoint Offset(Orientation orientation, int pixelOffset, int subPixelOffset)
        {
            if (orientation == Orientation.Horizontal)
                return new PixelPoint(X + new PixelValue(pixelOffset, subPixelOffset), Y);
            else
                return new PixelPoint(X, Y + new PixelValue(pixelOffset, subPixelOffset));
        }
    }

    public record PixelValue(int Pixel, int SubPixel)
    {
        public PixelValue NearestPixel(int direction)
        {
            if (direction > 0)
            {
                if (SubPixel > 0)
                    return new PixelValue(Pixel + 1, 0);
                else
                    return new PixelValue(Pixel, 0);
            }
            else if (direction < 0)
            {
                if (SubPixel < 0)
                    return new PixelValue(Pixel - 1, 0);
                else
                    return new PixelValue(Pixel, 0);
            }
            else
                return new PixelValue(Pixel, 0);
        }

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

        public static PixelValue operator -(PixelValue p1, PixelValue p2)
        {
            int newSubPixel = p1.SubPixel - p2.SubPixel;
            int newPixel = p1.Pixel - p2.Pixel;

            while (newSubPixel > 255)
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

        public static bool operator >(PixelValue p, int i)
        {
            return p.Pixel > i || ((p.Pixel == i) && (p.SubPixel > 0));
        }

        public static bool operator <(PixelValue p, int i)
        {
            return p.Pixel < i || ((p.Pixel == i) && (p.SubPixel < 0));
        }

        public static bool operator >(PixelValue p1, PixelValue p2)
        {
            return p1.Pixel > p2.Pixel || ((p1.Pixel == p2.Pixel) && (p1.SubPixel > p2.SubPixel));
        }

        public static bool operator <(PixelValue p1, PixelValue p2)
        {
            return p1.Pixel < p2.Pixel || ((p1.Pixel == p2.Pixel) && (p1.SubPixel < p2.SubPixel));
        }

        public static bool operator ==(PixelValue p, int i)
        {
            return p.Pixel == i && p.SubPixel == 0;
        }

        public static bool operator !=(PixelValue p, int i)
        {
            if (p.Pixel == i)
                return p.SubPixel != 0;
            else
                return true;
        }

        public override string ToString()
        {
            return $"{Pixel}.{SubPixel}";
        }
    }

}
