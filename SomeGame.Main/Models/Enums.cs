using System;

namespace SomeGame.Main.Models
{

    public enum InterfaceType
    {
        None,
        PlayerStatus
    }

    public enum Direction
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

    public enum InputButton
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

    public enum ButtonState
    {
        None=0,
        Down=1,
        Changed=2
    }

    public enum Flip
    {
        None=0,
        H=1,
        V=2,
        Both=3
    }

    public enum TileFlags
    {
        None=0,
        FlipH=1,
        FlipV=2,
        FlipHV=3,
        Solid=4,
        Collectible=8
    }

    public enum PaletteIndex
    {
        P1,
        P2,
        P3,
        P4
    }

    public enum LayerIndex
    {
        BG,
        FG,
        Interface
    }

    public enum SpriteIndex
    {
        Sprite1,
        Sprite2,
        Sprite3,
        Sprite4,
        Sprite5,
        Sprite6,
        Sprite7,
        Sprite8,
        Sprite9,
        Sprite10,
        Sprite11,
        Sprite12,
        Sprite13,
        Sprite14,
        Sprite15,
        Sprite16,

    }

    public enum SpritePriority
    {
        Back,
        Front
    }

    public enum AnimationKey : byte
    {
        Idle,
        Moving,
        Jumping,
        Attacking,
        Hurt
    }

    public enum Orientation
    {
        Vertical,
        Horizontal
    }

    public enum TileChoiceMode
    {
        Free,
        SemiStrict,
        Strict
    }

    public enum UIButtonState
    {
        None,
        MouseOver,
        Pressed
    }

    public enum LevelEditorMode
    {
        Free,
        Auto,
        Copy,
        Move,
        Relate,
        SetSolid
    }

    public enum StandardEnemyState
    {
        Idle,
        Moving,
        Attacking
    }

    [Flags]
    public enum ActorType
    {
        Player=1,
        Enemy=2,
        Character=4,
        Bullet=8,
        Item=16,
        Decoration=32
    }

    public enum AnimationState
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
        DeadSkeletonBone
    }

    public enum CollectibleId : byte
    {
        Coin = ActorId.Coin,
        Gem = ActorId.Gem,
        Apple = ActorId.Apple,
        Meat = ActorId.Meat,
    }

    public enum DestroyedState
    {
        None,
        Destroying,
        Destroyed
    }
}
