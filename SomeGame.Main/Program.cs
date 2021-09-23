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
            //var module = new ImageSectionSplitter(ImageContentKey.Sheet, TilesetContentKey.Tiles);
            // var module = new GameSystemTestModule();
            // var module = new FontTestModule();
            var module = new LevelEditorModule();

            using (var game = new GameEngine(module))
                game.Run();
        }
    }
}
