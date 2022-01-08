using NUnit.Framework;
using SomeGame.Main.Content;
using SomeGame.Main.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomeGame.Main.Tests
{
    class TiledObjectTests
    {
        private Layer _testLayer;

        [OneTimeSetUp]
        public void TestSetup()
        {
            var system = new GameSystem(null);
            _testLayer = new Layer(system, new TileMap(system, LevelContentKey.None, 100, 100), PaletteIndex.P1, 
                new RotatingInt(0, 800), 
                new RotatingInt(0, 800), 8);
        }

        [TestCase(0, 0, 0, 0)]
        [TestCase(4, 4, 0, 0)]
        [TestCase(8, 4, 1, 0)]
        [TestCase(8, 8, 1, 1)]
        [TestCase(400, 400, 50, 50)]
        public void TestTilePointFromScreenPixelPoint(int screenX, int screenY, int expectedTileX, int expectedTileY)
        {
            var tilePoint = _testLayer.TilePointFromScreenPixelPoint(screenX, screenY);
            Assert.AreEqual(tilePoint.X, expectedTileX);
            Assert.AreEqual(tilePoint.Y, expectedTileY);
        }

        [TestCase(400, 400, 100000, 2)]
        public void TestTilePointFromScreenPixelPointPerformance(int screenX, int screenY, int trials, int expectedMS)
        {
            var sw = new Stopwatch();
            int i = trials;
            sw.Start();
            while(i-- > 0)
                _testLayer.TilePointFromScreenPixelPoint(screenX, screenY);
            sw.Stop();

            Debug.WriteLine($"{trials} trials in {sw.ElapsedMilliseconds} ms");
            Assert.IsTrue(sw.ElapsedMilliseconds <= expectedMS, $"Duration: {sw.ElapsedMilliseconds} ms");            
        }
    }
}
