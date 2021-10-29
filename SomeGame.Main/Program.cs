using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Content;
using SomeGame.Main.Modules;
using System;

namespace SomeGame.Main
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new GameEngine(CreateModule))
                game.Run();
        }

        private static IGameModule CreateModule(ContentManager cm, GraphicsDevice gd)
        {
          //return new SceneDefinitionModule();
          //  return new LevelEditorModule(LevelContentKey.TestLevel, cm, gd);
          //  return new SpriteEditorModule(TilesetContentKey.Skeleton, cm, gd);
         //  return new AnimationDefinitionModule();
      return new SceneModule(SceneContentKey.Test1, cm, gd);

            // var module = new TextureCreatorModule(ImageContentKey.Items, TilesetContentKey.Items);
            //    var module = new PaletteCreatorModule();
            // var module = new ThemeDefinerModule(ImageContentKey.Sheet, TilesetContentKey.Tiles);
            // var module = new GameSystemTestModule();
            //var module = new FontTestModule();
            //   var module = new LevelEditorModule(LevelContentKey.TestLevelBG);
            //  var module = new SceneModule();
            // var module = new TileNeighborModule(new TileSetService());
        }
    }
}
