namespace SomeGame.Main.Models
{
    struct BoundedInt
    {
        public int Value { get; }
        public int Min { get; }
        public int Max { get; }

        public BoundedInt(int value, int max) : this(value, 0, max) { }

        public BoundedInt(int value, int min, int max)
        {
            Value = value;
            Min = min;
            Max = max;

            if (Value < min)
                Value = min;
            if (Value > max)
                Value = max;
        }

        public BoundedInt Set(int value)
        {
            return new BoundedInt(value, Min, Max);
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public static BoundedInt operator +(BoundedInt rb, int delta)
        {
            return new BoundedInt(rb.Value + delta, rb.Min, rb.Max);
        }

        public static BoundedInt operator +(int number, BoundedInt rb)
        {
            return new BoundedInt(number + rb.Value, rb.Min, rb.Max);
        }

        public static BoundedInt operator -(BoundedInt rb, int delta)
        {
            return new BoundedInt(rb.Value - delta, rb.Min, rb.Max);
        }

        public static BoundedInt operator -(int number, BoundedInt rb)
        {
            return new BoundedInt(number - rb.Value, rb.Min, rb.Max);
        }

        public static implicit operator int(BoundedInt b) => b.Value;
    }
}
