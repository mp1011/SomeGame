using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Models;
using System.Collections.Generic;

namespace SomeGame.Main.Services
{
    internal static class DebugService
    {
        public static void NoOp() { }

        public static GameSystem GameSystem { get; set; }
        public static List<Actor> Actors { get; } = new List<Actor>();

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
                var pos = actor.WorldPosition.ToXNARec();
                var layer = GameSystem.GetLayer(LayerIndex.FG);

                pos.X += layer.ScrollX;
                pos.Y += layer.ScrollY;

                spriteBatch.Draw(debugTexture, pos, Color.Green);
            }
        }
    }
}
