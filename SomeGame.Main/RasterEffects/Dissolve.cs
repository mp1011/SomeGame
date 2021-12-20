using SomeGame.Main.Services;

namespace SomeGame.Main.RasterEffects
{
    class Dissolve : IRasterEffect
    {
        public byte Amount { get; set; }

        private bool ShouldFadePixel(int x, int y)
        {
            var product = (x*x + y*y) % 255;
            return product < Amount;
        }

        public int AdjustX(int currentX, int currentY, int frame)
        {            
            if (ShouldFadePixel(currentX,currentY))
                return 4;
            else
                return currentX;
        }

        public int AdjustY(int currentX, int currentY, int frame)
        {
            if (ShouldFadePixel(currentX, currentY))
                return 4;
            else
                return currentY;
        }
    }
}
