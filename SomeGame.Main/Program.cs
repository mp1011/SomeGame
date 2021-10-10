using SomeGame.Main.Content;
using SomeGame.Main.Modules;
using SomeGame.Main.Services;
using System;

namespace SomeGame.Main
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {

            //var module = new ImageSectionSplitter(ImageContentKey.Sheet, TilesetContentKey.Tiles);
            // var module = new GameSystemTestModule();
            //var module = new FontTestModule();
            //  var module = new LevelEditorModule();
          // var module = new TextureCreatorModule(ImageContentKey.Skeleton, TilesetContentKey.Skeleton);
           var module = new LevelModule();
            //  var module = new TileNeighborModule(new TileSetService());
           // var module = new SpriteEditorModule(TilesetContentKey.Skeleton);

            using (var game = new GameEngine(module))
                game.Run();
        }
    }
}
