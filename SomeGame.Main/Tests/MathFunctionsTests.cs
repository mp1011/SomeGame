using NUnit.Framework;
using SomeGame.Main.Models;
using SomeGame.Main.Services;

namespace SomeGame.Main.Tests
{
    class MathFunctionsTests
    {
        [TestCase(Angle.Right, Angle.Up, 10, (Angle)10)]
        [TestCase(Angle.Right, Angle.Down, 10, (Angle)246)]
        [TestCase(Angle.Left, Angle.Up, 10, (Angle)118)]
        [TestCase(Angle.Left, Angle.Down, 10, (Angle)138)]
        [TestCase(Angle.Right, Angle.Up, 128, Angle.Up)]

        public void TestRotateAngle(Angle a1, Angle a2, byte amount, Angle expected)
        {
            var result = a1.RotateToward(a2, amount);
            Assert.AreEqual(expected, result);
        }
    }
}
