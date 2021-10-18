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
           // var module = new AnimationDefinitionModule();
           // var module = new TextureCreatorModule(ImageContentKey.Items, TilesetContentKey.Items);
          //  var module = new PaletteCreatorModule();
         // var module = new ThemeDefinerModule(ImageContentKey.Sheet, TilesetContentKey.Tiles);
            // var module = new GameSystemTestModule();
            //var module = new FontTestModule();
         // var module = new LevelEditorModule(LevelContentKey.TestLevel);
         
            var module = new LevelModule();
          // var module = new TileNeighborModule(new TileSetService());
         //  var module = new SpriteEditorModule(TilesetContentKey.Items);

            using (var game = new GameEngine(module))
                game.Run();
        }
    }
}
