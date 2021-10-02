namespace SomeGame.Main.Models
{
    struct BoundedInt
    {
        public int Value { get; }
        public int Max { get; }
        public BoundedInt(int value, int max)
        {
            Value = value;
            Max = max;

            if (Value < 0)
                Value = 0;
            if (Value > max)
                Value = max;
        }

        public BoundedInt Set(int value)
        {
            return new BoundedInt(value, Max);
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public static BoundedInt operator +(BoundedInt rb, int delta)
        {
            return new BoundedInt(rb.Value + delta, rb.Max);
        }

        public static BoundedInt operator +(int number, BoundedInt rb)
        {
            return new BoundedInt(number + rb.Value, rb.Max);
        }

        public static BoundedInt operator -(BoundedInt rb, int delta)
        {
            return new BoundedInt(rb.Value - delta, rb.Max);
        }

        public static BoundedInt operator -(int number, BoundedInt rb)
        {
            return new BoundedInt(number - rb.Value, rb.Max);
        }

        public static implicit operator int(BoundedInt b) => b.Value;
    }

}
