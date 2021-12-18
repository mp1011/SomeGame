using Microsoft.Xna.Framework;
using SomeGame.Main.Content;
using SomeGame.Main.GameInterface;
using SomeGame.Main.SceneControllers;
using SomeGame.Main.Scenes;
using System.Collections.Generic;
using System.Linq;

namespace SomeGame.Main.Models
{
    record ActorStart(ActorId ActorId, PixelPoint Position, PaletteIndex Palette);
    record CollectiblePlacement(CollectibleId Id, Point Position);
    record LayerInfo(LevelContentKey Key, PaletteIndex Palette, byte ScrollFactor);
    record SoundInfo(SoundContentKey Key, byte MaxOccurences);
    record SceneTransitions(
        SceneContentKey Left = SceneContentKey.None, 
        SceneContentKey Right = SceneContentKey.None, 
        SceneContentKey Up = SceneContentKey.None,
        SceneContentKey Down = SceneContentKey.None, 
        SceneContentKey Door1 = SceneContentKey.None, 
        SceneContentKey Door2 = SceneContentKey.None);


    record TransitionInfo(SceneContentKey NextScene, Rectangle ExitSide, Point PlayerExitPosition, Direction Direction)
    {
        public TransitionInfo() : this(SceneContentKey.None, Rectangle.Empty, Point.Zero, Direction.None) { }
    }

    record SceneLoadResult(bool NewScene, IGameInterface GameInterface, ISceneController Controller)
    {
        public SceneLoadResult() : this(false, null, null) { }
    }

    record SceneInfo(
        LayerInfo BgMap,
        LayerInfo FgMap,
        InterfaceType InterfaceType,
        MusicContentKey Song,
        Rectangle Bounds,
        byte BackgroundColor,
        TilesetContentKey[] VramImagesP1,
        TilesetContentKey[] VramImagesP2,
        TilesetContentKey[] VramImagesP3,
        TilesetContentKey[] VramImagesP4,
        SoundInfo[] Sounds,
        ActorStart[] Actors,
        CollectiblePlacement[] CollectiblePlacements,
        SceneTransitions Transitions)
    {

        public SceneInfo SetActorsAndCollectibles(IEnumerable<ActorStart> actors, IEnumerable<CollectiblePlacement> collectibles)
        {
            return new SceneInfo(BgMap, FgMap,  InterfaceType, Song, Bounds, BackgroundColor, 
                VramImagesP1, VramImagesP2, VramImagesP3, VramImagesP4,
                Sounds, 
                actors.ToArray(),
                collectibles.ToArray(), 
                Transitions);
        }
    }
}
