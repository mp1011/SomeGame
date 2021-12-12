using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SomeGame.Main.Models
{
    public record GameStartup(ContentManager ContentManager, GraphicsDevice GraphicsDevice, IRamViewer RamViewer);

}
