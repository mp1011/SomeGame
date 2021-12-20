using System;

namespace SomeGame.Main.Content
{
    public enum SoundContentKey : byte
    {
        None,
        Collect,
        Destroy,
        Hurt,
        Jump,
        Shoot,
        PlayerLose
    }

    public enum MusicContentKey : byte
    {
        None,
        Song1
    }

    public enum ImageContentKey : byte
    {
        None,
        Sheet,
        Characters_7,
        Hero,
        Skeleton,
        Bullet,
        Hud,
        SystemPalette,
        Items,
        Gizmos,
        Tiles1,
        Tiles2,
        Tiles3,
        Tiles4,
        Tiles5,
        Tiles6,
        Ghost,
        Bullet2,
        Bullet3,
        Bat,
        Mountains
    }

    public enum TilesetContentKey : byte
    {
        None,
        Tiles,
        Font,
        Hero,
        Skeleton,
        Bullet,
        Hud,
        Items,
        Gizmos,
        Tiles1,
        Tiles2,
        Tiles3,
        Tiles4,
        Tiles5,
        Tiles6,
        Ghost,
        Bullet2,
        Bullet3,
        Bat,
        Mountains
    }

    public enum LevelContentKey : byte
    {
        None,
        TestLevel,
        TestLevel2,
        TestLevelBG,
        LongMapTest,
        TestLevel3,
        TestLevel3BG
    }

    public enum SceneContentKey : byte
    {
        None,
        Level1,
        LevelTitleCard,
        Level1TitleCard,
        GameOver
    }

    public static class SceneContentKeyExtensions
    {
        public static SceneContentKey GetSceneAfterTitleCard(this SceneContentKey sceneContentKey)
        {
            switch(sceneContentKey)
            {
                case SceneContentKey.Level1TitleCard: return SceneContentKey.Level1;
                case SceneContentKey.GameOver: return SceneContentKey.Level1TitleCard;
                default: return SceneContentKey.None;
            }
        }

        public static SceneContentKey GetTitleCardFromScene(this SceneContentKey sceneContentKey)
        {
            switch (sceneContentKey)
            {
                case SceneContentKey.Level1: return SceneContentKey.Level1TitleCard;
                default: return SceneContentKey.None;
            }
        }
    }
}
