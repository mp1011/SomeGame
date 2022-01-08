namespace SomeGame.Main.Models
{
    class ImageSplitterGrid 
    {
        private readonly MemoryGrid<bool> _grid;

        public ImageSplitterGrid(int width, int height)
        {
            _grid = new MemoryGrid<bool>(width, height);
        }
    }
}
