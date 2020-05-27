namespace RandomMazeGenerator.Core
{
    public class Maze
    {
        public Maze(int width) : this(width, width) { }

        public Maze(int width, int height)
        {
            Width = width;
            Height = height;
            Cells = new MazeCell[width * height];

            for(int x = 0;x < width;x++)
                for(int y = 0;y < height;y++)
                    Cells[y * width + x] = new MazeCell(x, y);
        }

        public MazeCell[] Cells { get; set; }
        public int Width { get; }
        public int Height { get; }
    }
}
