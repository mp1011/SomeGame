using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Content;
using SomeGame.Main.Models;
using SomeGame.Main.Services;

namespace SomeGame.Main.Modules
{
    class MusicEditorModule : EditorModule
    {
        private readonly AudioService _audioService;
        private readonly SongData _song;
        public MusicEditorModule(GameStartup gameStartup) : base(gameStartup)
        {
            _audioService = new AudioService(new ResourceLoader(gameStartup.ContentManager));

            _song = CreateSong1();
            DataSerializer.Save(_song);
   
            _audioService.LoadSong(_song);
        }

        private SongData CreateSong1()
        {
            return new SongData(MusicContentKey.Song1,
                new SongStem[]
                {
                    new SongStem('A',1,255),
                    new SongStem('A',2,255),
                    new SongStem('A',3,255),
                    new SongStem('B',1,150),
                    new SongStem('B',2,150),
                    new SongStem('C',1,255),
                    new SongStem('C',2,150)
                },
                new SongSection[]
                {
                    new SongSection(1,0),
                    new SongSection(1,0),
                    new SongSection(1,0),
                    new SongSection(1,0),
                    new SongSection(1,4),
                    new SongSection(2,0),
                    new SongSection(2,0),
                    new SongSection(3,5),
                    new SongSection(6,0),
                    new SongSection(6,7),
                }
                );
        }


        protected override void InitializeLayer(LayerIndex index, Layer layer)
        {
        }

        protected override void AfterInitialize()
        {
            _audioService.StartMusic();
        }

        protected override IndexedTilesetImage[] LoadVramImages(ResourceLoader resourceLoader)
        {
            using var fontImage = resourceLoader.LoadTexture(TilesetContentKey.Font);
            return new IndexedTilesetImage[] { fontImage.ToIndexedTilesetImage() };
        }

        protected override void Update()
        {
            _audioService.UpdateMusic();
        }
    }
}
