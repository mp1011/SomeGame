using NUnit.Framework;
using SomeGame.Main.Models;

namespace SomeGame.Main.Tests
{
    class RamValueTests
    {
        [Test]
        public void TestRamByte()
        {
            var ram = new RAM();
            var b = ram.DeclareByte();

            b.Set(128);

            Assert.AreEqual(128, (byte)b);
        }

        [Test]
        public void TestRamInt()
        {
            var ram = new RAM();
            var i = ram.DeclareInt();
            i.Set(2030);
            Assert.AreEqual(2030, (int)i);

            i.Set(-520);
            Assert.AreEqual(-520, (int)i);

        }

        [Test]
        public void TestRamSignedByte()
        {
            var ram = new RAM();
            var i = ram.DeclareSignedByte();
            i.Set(-50);
            Assert.AreEqual(-50, (int)i);

            i.Set(50);
            Assert.AreEqual(50, (int)i);
        }


        [Test]
        public void TestRamPixelValue()
        {
            var ram = new RAM();
            var i = ram.DeclarePixelValue(200, 100);
            i.Add(new PixelValue(200, 100));

            Assert.AreEqual(401, (int)i.Pixel);
            Assert.AreEqual(72, (int)i.SubPixel);
        }

        [Test]
        public void TestRamPixelValueAddSubpixel()
        {
            var ram = new RAM();
            var i = ram.DeclarePixelValue(0, 0);

            i.Add(new PixelValue(0, 10));

            Assert.AreEqual(0, (int)i.Pixel);
            Assert.AreEqual(10, (int)i.SubPixel);

            i.Add(new PixelValue(0, 110));
            Assert.AreEqual(0, (int)i.Pixel);
            Assert.AreEqual(120, (int)i.SubPixel);

            i.Add(new PixelValue(0, 20));
            Assert.AreEqual(1, (int)i.Pixel);
            Assert.AreEqual(12, (int)i.SubPixel);
        }

        [Test]
        public void TestRamPixelValueSubtractSubpixel()
        {
            var ram = new RAM();
            var i = ram.DeclarePixelValue(0, 0);

            i.Add(new PixelValue(0, -10));

            Assert.AreEqual(0, (int)i.Pixel);
            Assert.AreEqual(-10, (int)i.SubPixel);

            i.Add(new PixelValue(0, -110));
            Assert.AreEqual(0, (int)i.Pixel);
            Assert.AreEqual(-120, (int)i.SubPixel);

            i.Add(new PixelValue(0, -20));
            Assert.AreEqual(-1, (int)i.Pixel);
            Assert.AreEqual(-12, (int)i.SubPixel);
        }

    }
}
