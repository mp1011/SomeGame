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

        private static GameRectangleWithSubpixels _lastPosition = new GameRectangleWithSubpixels(0,0,0,0);
        public static void ShowPosition(Actor actor)
        {
            if(!_lastPosition.Equals(actor.WorldPosition))
            {
                System.Diagnostics.Debug.WriteLine(actor.WorldPosition);
                _lastPosition = actor.WorldPosition.Copy();
            }
        }

        public static void CheckBadPosition(Actor actor)
        {
            if(actor.WorldPosition.X > 88 && actor.WorldPosition.Y > 98)
            {
               // System.Diagnostics.Debug.WriteLine("!!!");
            }
        }
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
