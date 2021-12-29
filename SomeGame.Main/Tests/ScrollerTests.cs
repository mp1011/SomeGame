using Microsoft.Xna.Framework;
using NUnit.Framework;
using SomeGame.Main.Content;
using SomeGame.Main.Models;
using SomeGame.Main.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomeGame.Main.Tests
{
    class ScrollerTests
    {
        private Scroller CreateScroller(GameSystem gameSystem)
        {
            var scroller = new Scroller(gameSystem);

            scroller.SetTileMaps(new TileMap(LevelContentKey.None, gameSystem.LayerTileWidth * 2, gameSystem.LayerTileHeight * 2),
                                 new TileMap(LevelContentKey.None, gameSystem.LayerTileWidth * 2, gameSystem.LayerTileHeight * 2));

            return scroller;
        }

        [TestCase(100,0,100)]
        [TestCase(100, 10, 90)]
        [TestCase(-8, 0, 632)]

        public void TestSpriteScreenPosition(int actorWorldX, int cameraX, int expectedActorLayerX)
        {
            var gameSystem = new GameSystem(new GameStartup(null,null,null));
            var actor = new Actor(gameSystem, ActorType.Character, TilesetContentKey.None, null, null, null, new Rectangle(0, 0, 16, 16), null);
            actor.WorldPosition.X.Set(actorWorldX);

            var scroller = CreateScroller(gameSystem);
            var sprite = new Sprite(gameSystem, 640, 480, 8);

            scroller.Camera.X = cameraX;
            scroller.ScrollActorSprite(actor, sprite);

            Assert.IsTrue(actor.Visible);
            Assert.AreEqual(expectedActorLayerX, (int)sprite.ScrollX);
        }
    }
}
