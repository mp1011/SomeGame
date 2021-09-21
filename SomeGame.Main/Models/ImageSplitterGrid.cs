namespace SomeGame.Main.Models
{
    class ImageSplitterGrid 
    {
        private readonly Grid<bool> _grid;

        public ImageSplitterGrid(int width, int height)
        {
            _grid = new Grid<bool>(width, height);
        }
    }
}
