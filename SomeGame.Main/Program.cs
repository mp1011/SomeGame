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
            byte b = 255;
            byte b2 = (byte)(b + (byte)10);

            //var module = new ImageSectionSplitter(ImageContentKey.Sheet, TilesetContentKey.Tiles);
           // var module = new GameSystemTestModule();
            //var module = new FontTestModule();
            //var module = new LevelEditorModule();
            //var module = new TextureCreatorModule();
            var module = new LevelModule();

            using (var game = new GameEngine(module))
                game.Run();
        }
    }
}
