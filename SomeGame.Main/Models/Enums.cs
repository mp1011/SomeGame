using System;

namespace SomeGame.Main.Models
{

    public enum InputQueue : byte
    {
        None=0,
        Jump=1,
        Attack=2
    }

    public enum InterfaceType : byte
    {
        None,
        PlayerStatus,
        TitleCard
    }

    public enum Direction : byte
    {
        None,
        Up,
        UpRight,
        Right,
        DownRight,
        Down,
        DownLeft,
        Left,
        UpLeft
    }

    public enum Angle : byte
    {
        Right=0,
        Up=64,
        Left=128,
        Down=192
    }

    public enum InputButton : byte
    {
        None,
        Up,
        Down,
        Left,
        Right,
        Start,
        A,
        B
    }

    public enum ButtonState : byte
    {
        None=0,
        Down=1,
        Changed=2
    }

    public enum Flip : byte
    {
        None = 0,
        FlipH = 1,
        H =1,
        FlipV = 2,
        V=2,
        Both = 3,
    }

    public enum SpriteFlags: byte
    {
        None=0,
        FlipH=1,
        FlipV=2,
        Visible=4,
        Priority=8
    }

    public enum ActorState : byte
    {
        WaitingForActivation,
        Active,
        Destroying,
        Destroyed
    }

    public enum ActorFlags : byte
    {
        None=0,
        FlipH=1,
        FlipV=2,
        Visible=4,
        IsAnimationFinished=8,
        SpriteRequired=16,
    }

    public enum TileFlags : byte
    {
        None=0,
        FlipH=1,
        FlipV=2,
        FlipHV=3,
        Solid=4,
        Collectible=8
    }

    public enum PaletteIndex : byte
    {
        P1,
        P2,
        P3,
        P4
    }

    public static class PaletteIndexExtensions
    {
        public static PaletteIndex Next(this PaletteIndex p)
        {
            if (p == PaletteIndex.P4)
                return PaletteIndex.P1;
            else
                return p + 1;
        }
    }

    public enum LayerIndex : byte
    {
        BG,
        FG,
        Interface
    }

    public enum SpriteIndex : byte
    {
        Sprite1,
        Sprite2,
        Sprite3,
        Sprite4,
        Sprite5,
        Sprite6,
        Sprite7,
        Sprite8
    }

    public enum AnimationKey : byte
    {
        Idle,
        Moving,
        Jumping,
        Attacking,
        Hurt
    }

    public enum Orientation : byte
    {
        Vertical,
        Horizontal
    }

    public enum TileChoiceMode : byte
    {
        Free,
        SemiStrict,
        Strict
    }

    public enum UIButtonState : byte
    {
        None,
        MouseOver,
        Pressed,
        Pressed2
    }

    public enum LevelEditorMode : byte
    {
        Free,
        Auto,
        Copy,
        Move,
        Relate,
        SetSolid,
        Objects,   
        Collectibles
    }

    public enum StandardEnemyState : byte
    {
        Idle,
        Moving,
        Attacking
    }

    [Flags]
    public enum ActorType : byte
    {
        Player=1,
        Enemy=2,
        Character=4,
        Bullet=8,
        Item=16,
        Decoration=32,
        Gizmo=64
    }

    public enum AnimationState : byte
    {
        None,
        Playing,
        Finished
    }

    public enum ActorId : byte
    {
        Player,
        PlayerBullet,
        Skeleton,
        SkeletonBone,
        Coin,
        Gem,
        Apple,
        Meat,
        Skull,
        DeadSkeletonBone,
        Key,
        MovingPlatform,
        Bat,
        Ghost,
        GhostBullet,
        Spring
    }

    public enum CollectibleId : byte
    {
        Coin = ActorId.Coin,
        Gem = ActorId.Gem,
        Apple = ActorId.Apple,
        Meat = ActorId.Meat,
        Key = ActorId.Key
    }

    public enum DestroyedState : byte
    {
        None,
        Destroying,
        Destroyed
    }
}
