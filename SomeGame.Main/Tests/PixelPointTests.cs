using NUnit.Framework;
using SomeGame.Main.Models;

namespace SomeGame.Main.Tests
{
    class PixelPointTests
    {
        [TestCase(100,50,100,50,0,90,0,90)]
        [TestCase(-100, 50, 100, 50, 0, -90, 0, 90)]
        public void TestNormalizePoint(int xPixel, int xSubPixel, int yPixel, int ySubPixel, 
            int normalizedXPixel, int normalizedXSubPixel, int normalizedYPixel, int normalizedYSubPixel)
        {
            var pixelPoint = new PixelPoint(new PixelValue(xPixel, xSubPixel), new PixelValue(yPixel, ySubPixel));
            var normalized = pixelPoint.Normalize();
            Assert.AreEqual(normalizedXPixel, normalized.X.Pixel);
            Assert.AreEqual(normalizedXSubPixel, normalized.X.SubPixel);
            Assert.AreEqual(normalizedYPixel, normalized.Y.Pixel);
            Assert.AreEqual(normalizedYSubPixel, normalized.Y.SubPixel);
        }
    }
}
