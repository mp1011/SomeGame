namespace SomeGame.Main.RasterEffects
{
    interface IRasterEffect
    {
        int AdjustX(int currentX, int currentY, int frame);
        int AdjustY(int currentX, int currentY, int frame);
    }
}
