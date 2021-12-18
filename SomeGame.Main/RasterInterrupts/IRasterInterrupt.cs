using SomeGame.Main.Models;

namespace SomeGame.Main.RasterInterrupts
{
    interface IRasterInterrupt
    {
        int VerticalLine { get; }
        void Handle();
    }

   
}
