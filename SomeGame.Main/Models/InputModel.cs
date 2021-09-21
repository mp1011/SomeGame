namespace SomeGame.Main.Models
{
    record InputModel(ButtonState Up, ButtonState Down, ButtonState Left, ButtonState Right,
        ButtonState Start, ButtonState A, ButtonState B, int MouseX, int MouseY);
}
