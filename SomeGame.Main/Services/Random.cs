using System;

namespace SomeGame.Main.Services
{
    public static class RandomUtil
    {
        private static Random _random = new Random();

        public static T RandomItem<T>(params T[] choices)
        {
            int index = _random.Next(0, choices.Length);
            return choices[index];
        }

        public static bool RandomBit()
        {
            return _random.Next(0, 2) == 0;
        }
    }
}
