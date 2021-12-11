using Microsoft.Xna.Framework;
using System;

namespace SomeGame.Main.Models
{
    class RAM
    {
        private byte[] _memory = new byte[5000];
        private int _declareIndex = 0;

        private void IncrementDeclareIndex(byte amount)
        {
            _declareIndex += amount;
            if (_declareIndex >= _memory.Length)
                throw new Exception("Out of memory");
        }

        public RamByte DeclareByte(byte b=0)
        {
            var ret = new RamByte(_declareIndex, _memory);
            ret.Set(b);
            IncrementDeclareIndex(1);      
            return ret;
        }

        public RamInt DeclareInt(int value=0)
        {
            var ret = new RamInt(_declareIndex, _memory);
            ret.Set(value);
            IncrementDeclareIndex(2);
            return ret;
        }

        public BoundedRamInt DeclareBoundedInt(int value, int max)
        {
            var ret = new BoundedRamInt(DeclareInt(value), DeclareInt(max));
            return ret;
        }

        public RamSignedByte DeclareSignedByte(int value = 0)
        {
            var ret = new RamSignedByte(_declareIndex, _memory);
            ret.Set(value);
            IncrementDeclareIndex(1);
            return ret;
        }

        public RamEnum<T> DeclareEnum<T>(T value) where T:Enum
        {
            var ret = new RamEnum<T>(_declareIndex, _memory);
            ret.Set(value);
            IncrementDeclareIndex(1);
            return ret;
        }

        public RamRectangle DeclareRectangle(Rectangle rec)
        {
            var ret = new RamRectangle(this);
            ret.Set(rec);
            return ret;
        }

        public BoundedGameRectangle DeclareBoundedRectangle(int x, int y, int width, int height, int maxX, int maxY)
        {
            return new BoundedGameRectangle(
                x: DeclareBoundedInt(x, maxX),
                y: DeclareBoundedInt(y, maxY),
                width: DeclareInt(width),
                height: DeclareInt(height));
        }

        public RamPixelValue DeclarePixelValue(int pixel, int subPixel)
        {
            return new RamPixelValue(DeclareInt(pixel), DeclareSignedByte(subPixel));
        }

        public RamPixelPoint DeclarePixelPoint() => new RamPixelPoint(DeclarePixelValue(0, 0), DeclarePixelValue(0, 0));

        public RamGameRectangle DeclareGameRectangleWithSubpixels(int x, int y, byte width, byte height)
        {
            var ret = DeclareGameRectangleWithSubpixels(width, height);
            ret.X.Set(x);
            ret.Y.Set(y);
            return ret;
        }

        public RamGameRectangle DeclareGameRectangleWithSubpixels(byte width, byte height)
        {
            var ret = new RamGameRectangle(
                                X: new RamPixelValue(DeclareInt(), DeclareSignedByte()),
                                Y: new RamPixelValue(DeclareInt(), DeclareSignedByte()),
                                Width: DeclareByte(width),
                                Height: DeclareByte(height));
            return ret;
        }
    }

    public abstract class RamValue
    {
        protected readonly int Index;
        protected readonly byte[] Memory;
        public RamValue(int index, byte[] ram)
        {
            Index = index;
            Memory = ram;
        }
    }

    public class RamByte : RamValue
    {
        public RamByte(int index, byte[] ram) : base(index, ram) { }

        public static RamByte operator ++(RamByte r)
        {
            r.Memory[r.Index]++;
            return r;
        }

        public static implicit operator byte(RamByte r)=> r.Memory[r.Index];

        public RamByte Set(byte newValue)
        {
            Memory[Index] = newValue;
            return this;
        }

        public static bool operator ==(RamByte r, byte value) => r.Memory[r.Index] == value;
        public static bool operator !=(RamByte r, byte value) => r.Memory[r.Index] != value;

        public override bool Equals(object obj)
        {
            if (obj is RamByte r)
                return Memory[Index] == r.Memory[r.Index];
            else
                return false;
        }

        public override int GetHashCode()
        {
            return Memory[Index];
        }
    }

    public class RamEnum<T> : RamByte where T : Enum
    {
        public RamEnum(int index, byte[] ram) : base(index, ram)
        {
        }

        public RamByte Set(T newValue) => Set((byte)(object)newValue);

        public RamByte SetFlag(T flag, bool on)
        {
            var flagByte = (byte)(object)flag;
            var thisByte = Memory[Index];

            if (on)
                Memory[Index] = (byte)(thisByte | flagByte);
            else
                Memory[Index] = (byte)(thisByte & ~flagByte);

            return this;
        }

        public override string ToString()
        {
            var e = (T)this;
            return e.ToString();
        }

        public static bool operator ==(RamEnum<T> r, T value) => r.Memory[r.Index] == (byte)(object)value;
        public static bool operator !=(RamEnum<T> r, T value) => r.Memory[r.Index] != (byte)(object)value;

        public static implicit operator T (RamEnum<T> ramEnum) => (T)Enum.ToObject(typeof(T), ramEnum.Memory[ramEnum.Index]);

        public override bool Equals(object obj)
        {
            if (obj is RamEnum<T> r)
                return Memory[Index] == r.Memory[r.Index];
            else
                return false;
        }

        public override int GetHashCode()
        {
            return Memory[Index];
        }
    }

    public class RamSignedByte : RamValue
    {
        public RamSignedByte(int index, byte[] ram) : base(index, ram)
        {
        }

        public RamSignedByte Set(int value)
        {
            if (value < -128)
                value = -128;
            if (value >= 128)
                value = 127;

            value += 128;
            Memory[Index] = (byte)value;
            return this;
        }

        public static implicit operator int(RamSignedByte r)
        {
            var value = (int)r.Memory[r.Index];
            return value - 128;
        }

        public override string ToString() => ((int)this).ToString();
    }

    public class RamInt : RamValue
    {
        public RamInt(int index, byte[] ram) : base(index, ram)
        {
        }

        public RamInt Set(int value)
        {
            value = value + 32768;

            //only lower 2 bytes used
            Memory[Index] = (byte)value;
            Memory[Index+1] = (byte)(value>>8);
            return this;
        }

        public static implicit operator int(RamInt r)
        {
            var low = (int)r.Memory[r.Index];
            var high = (int)r.Memory[r.Index + 1];

            var unsigned = (high << 8) + low;

            return unsigned - 32768;
        }

        public override string ToString() => ((int)this).ToString();
    }

    public class BoundedRamInt
    {
        private readonly RamInt _limit;
        private readonly RamInt _value;

        public int Max
        {
            get => _limit;
            set => _limit.Set(value);
        }

        public BoundedRamInt(RamInt value, RamInt limit)
        {
            _limit = limit;
            _value = value;
        }

        public BoundedRamInt Set(int value)
        {
            if (value > Max)
                value = Max;
            else if (value < 0)
                value = 0;

            _value.Set(value);
            return this;
        }

        public static implicit operator int(BoundedRamInt r) => r._value;
    }


    class RamRectangle : IGameRectangle
    {
        public RamInt X { get; }
        public RamInt Y { get; }
        public RamInt Width { get; }
        public RamInt Height { get; }
        int IGameRectangle.X { get => X; set => X.Set(value); }
        int IGameRectangle.Y { get => Y; set => Y.Set(value); }
        int IGameRectangle.Width => Width;
        int IGameRectangle.Height => Height;

        public Point  Center 
        { 
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public RamRectangle(RAM ram)
        {
            X = ram.DeclareInt();
            Y = ram.DeclareInt();
            Width = ram.DeclareInt();
            Height = ram.DeclareInt();
        }

        public void Set(Rectangle rectangle)
        {
            X.Set(rectangle.X);
            Y.Set(rectangle.Y);
            Width.Set(rectangle.Width);
            Height.Set(rectangle.Height);
        }
    }
}
