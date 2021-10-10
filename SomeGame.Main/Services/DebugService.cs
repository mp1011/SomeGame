using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Models;
using System.Collections.Generic;

namespace SomeGame.Main.Services
{
    internal static class DebugService
    {
        public static GameSystem GameSystem { get; set; }
        public static List<Actor> Actors { get; } = new List<Actor>();


        public static void DrawHitboxes(SpriteBatch spriteBatch, Texture2D debugTexture)
        {
            foreach (var actor in Actors)
            {
                var pos = (Rectangle)actor.WorldPosition;
                var layer = GameSystem.GetLayer(LayerIndex.FG);

                pos.X += layer.ScrollX;
                pos.Y += layer.ScrollY;

                spriteBatch.Draw(debugTexture, pos, Color.Green);
            }
        }
    }
}
