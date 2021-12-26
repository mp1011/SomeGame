using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SomeGame.Main.Models
{
    public interface IRamViewer
    {
        void Initialize(RAM ram);
        void BeforeFrame();
        void MemoryChanged(int address, byte value);
        IReadOnlyDictionary<int,string> Labels { get; set; }
    }

    class EmptyRamViewer : IRamViewer
    {
        public void MemoryChanged(int address, byte value) { }

        public void Initialize(RAM ram)
        {
        }

        public void BeforeFrame()
        {
        }

        public IReadOnlyDictionary<int, string> Labels { get; set; } 
    }

    public class RAM
    {
        private GameSystem _gameSystem;
        private byte[] _memory = new byte[2000];
        private int _declareIndex = 0;
        private IRamViewer _ramViewer;

        public int SceneDataAddress { get; private set; }

        private Dictionary<int, string> _labels = new Dictionary<int, string>();

        internal RAM(GameSystem gameSystem, IRamViewer ramViewer)
        {
            _gameSystem = gameSystem;
            _ramViewer = ramViewer;
            _ramViewer.Labels = _labels;
            _ramViewer.Initialize(this);
        }

        public void MarkSceneDataAddress() => SceneDataAddress = _declareIndex;
        public void AddLabel(string label)
        {
            if (_labels.ContainsKey(_declareIndex))
                _labels[_declareIndex] = label;
            else 
                _labels.Add(_declareIndex, label);
        }

        public void ResetSceneData()
        {
            if (SceneDataAddress == 0)
                return;

            for(int i = SceneDataAddress; i < _memory.Length;i++)
            {
                _memory[i] = 0;
                _ramViewer.MemoryChanged(i, 0);
            }

            _declareIndex = SceneDataAddress;
        }

        public byte this[int index]
        {
            get => _memory[index];
            set
            {
                if(value != _memory[index])
                    _ramViewer.MemoryChanged(index, value);

                _memory[index] = value;                
            }
        }

        private void IncrementDeclareIndex(byte amount)
        {
            for(int i = _declareIndex; i < _declareIndex + amount;i++)
                _ramViewer.MemoryChanged(i, 0);

            _declareIndex += amount;
            if (_declareIndex >= _memory.Length)
                throw new Exception("Out of memory");


        }

        public RamByte DeclareByte(byte b=0)
        {
            var ret = new RamByte(_declareIndex, this);
            ret.Set(b);
            IncrementDeclareIndex(1);      
            return ret;
        }

        public RamInt DeclareInt(int value=0)
        {
            var ret = new RamInt(_declareIndex, this);
            ret.Set(value);
            IncrementDeclareIndex(2);
            return ret;
        }

        public BoundedRamInt DeclareBoundedInt(int value, int max)
        {
            var ret = new BoundedRamInt(DeclareInt(value), DeclareInt(max));
            return ret;
        }

        public BoundedRamByte DeclareBoundedByte(byte value, byte max)
        {
            return new BoundedRamByte(DeclareByte(value), DeclareByte(max));
        }

        public RamSignedByte DeclareSignedByte(int value = 0)
        {
            var ret = new RamSignedByte(_declareIndex, this);
            ret.Set(value);
            IncrementDeclareIndex(1);
            return ret;
        }

        public RamEnum<T> DeclareEnum<T>(T value) where T:Enum
        {
            var ret = new RamEnum<T>(_declareIndex, this);
            ret.Set(value);
            IncrementDeclareIndex(1);
            return ret;
        }

        internal RamRectangle DeclareRectangle(Rectangle rec)
        {
            var ret = new RamRectangle(this);
            ret.Set(rec);
            return ret;
        }

        internal RamPoint DeclarePoint()
        {
            return new RamPoint(DeclareInt(), DeclareInt());
        }

        internal BoundedGameRectangle DeclareBoundedRectangle(int x, int y, int width, int height, int maxX, int maxY)
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

        internal RamGameRectangle DeclareGameRectangleWithSubpixels(int x, int y, byte width, byte height)
        {
            var ret = DeclareGameRectangleWithSubpixels(width, height);
            ret.X.Set(x);
            ret.Y.Set(y);
            return ret;
        }

        internal RamGameRectangle DeclareGameRectangleWithSubpixels(byte width, byte height)
        {
            var ret = new RamGameRectangle(
                                X: new RamPixelValue(DeclareInt(), DeclareSignedByte()),
                                Y: new RamPixelValue(DeclareInt(), DeclareSignedByte()),
                                Width: DeclareByte(width),
                                Height: DeclareByte(height));
            return ret;
        }

        internal RamPalette DeclarePalette(Palette systemPalette)
        {
            return new RamPalette(systemPalette,
                Enumerable.Range(0, _gameSystem.ColorsPerPalette).Select(e => DeclareByte()));
        }
    }

    public abstract class RamValue
    {
        protected readonly int Index;
        protected readonly RAM Memory;

        public RamValue(int index, RAM ram)
        {
            Index = index;
            Memory = ram;
        }
    }

    public class RamByte : RamValue
    {
        public RamByte(int index, RAM ram) : base(index, ram) { }

        public static RamByte operator ++(RamByte r)
        {
            r.Memory[r.Index]++;
            return r;
        }

        public RamByte Inc()
        {
            Memory[Index]++;
            return this;
        }

        public RamByte Dec()
        {
            Memory[Index]--;
            return this;
        }

        public void Add(int value) => Set(this + value);
        public void Subtract(int value) => Set(this - value);

        public static implicit operator byte(RamByte r)=> r.Memory[r.Index];

        public RamByte Set(byte newValue)
        {
            Memory[Index] = newValue;
            return this;
        }
        public RamByte Set(int newValue) => Set((byte)newValue);

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

        public override string ToString()
        {
            return ((byte)this).ToString("X2");
        }
    }

    public class RamEnum<T> : RamByte where T : Enum
    {
        public RamEnum(int index, RAM ram) : base(index, ram)
        {
        }

        public RamByte Set(T newValue) => Set((byte)(object)newValue);

        public bool GetFlag(T flag)
        {
            var flagByte = (byte)(object)flag;
            var thisByte = Memory[Index];
            return (thisByte & flagByte) > 0;

        }

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
        public RamSignedByte(int index, RAM ram) : base(index, ram)
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
        public RamInt(int index, RAM ram) : base(index, ram)
        {
        }

        public void Add(int value) => Set(this + value);
        public void Subtract(int value) => Set(this - value);

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

        public string ToString(string format)
        {
            int i = this;
            return i.ToString(format);
        }

        public override string ToString() => ((int)this).ToString();
    }

    public class BoundedRamByte
    {
        private readonly RamByte _limit;
        private readonly RamByte _value;

        public int Max
        {
            get => _limit;
            set => _limit.Set(value);
        }

        public BoundedRamByte(RamByte value, RamByte limit)
        {
            _limit = limit;
            _value = value;
        }

        public BoundedRamByte Set(int value)
        {
            if (value > Max)
                value = Max;
            else if (value < 0)
                value = 0;

            _value.Set(value);
            return this;
        }

        public void Add(int value) => Set(this + value);
        public void Subtract(int value) => Set(this - value);

        public static implicit operator byte(BoundedRamByte r) => r._value;
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
    record RamPoint(RamInt X, RamInt Y)
    {
        public void Set(Point p)
        {
            X.Set(p.X);
            Y.Set(p.Y);
        }

        public static implicit operator Point(RamPoint r)=> new Point(r.X,r.Y);
    }

    class RamPalette
    {
        private Palette _systemPalette;
        public RamByte[] Colors { get; }

        public RamPalette(Palette systemPalette, IEnumerable<RamByte> colors)
        {
            _systemPalette = systemPalette;
            Colors = colors.ToArray();
        }

        public byte GetIndex(Color color)
        {
            byte i = 0;
            while(i < Colors.Length)
            {
                if (this[i] == color)
                    return i;

                i++;
            }

            return 0;
        }

        public void Set(Palette colors)
        {
            Set(colors.Select(c => _systemPalette.GetIndex(c)));
        }

        public void Set(IEnumerable<byte> data)
        {
            int i = 0;
            foreach (var item in data)
                Colors[i++].Set(item);
        }

        public Color this[byte index] => (index < Colors.Length) ? _systemPalette[Colors[index]] : Color.Black;
    
        public static implicit operator Palette(RamPalette ramPalette)
        {
            return new Palette(Enumerable.Range(0, ramPalette.Colors.Length)
                .Select(i => ramPalette[(byte)i]));
        }
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
