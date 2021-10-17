namespace SomeGame.Main.Models
{
    struct RotatingInt
    {
        public int Value { get; }
        public int Max { get; }
        public RotatingInt(int value, int max)
        {
            Value = value;
            Max = max;

            if (Max <= 0)
                Value = 0;
            else
            {
                while (Value < 0)
                    Value += Max;
                while (Value >= Max)
                    Value -= Max;
            }
        }

        public RotatingInt Set(int value)
        {
            return new RotatingInt(value, Max);
        }
        public RotatingInt Set(double value)
        {
            return new RotatingInt((int)value, Max);
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public static RotatingInt operator ++(RotatingInt rb)
        {
            return new RotatingInt(rb.Value + 1, rb.Max);
        }

        public static RotatingInt operator --(RotatingInt rb)
        {
            return new RotatingInt(rb.Value - 1, rb.Max);
        }

        public static RotatingInt operator +(RotatingInt rb, int delta)
        {
            return new RotatingInt(rb.Value + delta, rb.Max);
        }

        public static RotatingInt operator +(int number, RotatingInt rb)
        {
            return new RotatingInt(number + rb.Value, rb.Max);
        }

        public static RotatingInt operator -(RotatingInt rb, int delta)
        {
            return new RotatingInt(rb.Value - delta, rb.Max);
        }

        public static RotatingInt operator -(int number, RotatingInt rb)
        {
            return new RotatingInt(number - rb.Value, rb.Max);
        }

        public static implicit operator int(RotatingInt b) => b.Value;
    }

}
