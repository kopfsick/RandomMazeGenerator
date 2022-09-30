using Kopfsick.CreativeCoding.Algorithms;
using Kopfsick.CreativeCoding.Gameloops;
using PropertyTools.DataAnnotations;
using RandomMazeGenerator.Core;
using SharpNoise.Modules;
using SkiaSharp;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace RandomMazeGenerator.Skia.WPF
{

    public class Settings
    {
        [Slidable(1,500, 1, 10, true, 10)]
        [HeaderPlacement(HeaderPlacement.Above)]
        public int StepsPerUpdate { get; set; } = 10;
        
        [Slidable(1,500, 1, 10, true, 10)]
        [HeaderPlacement(HeaderPlacement.Above)]
        public int StepsPerUpdateSolving { get; set; } = 10;

        public bool VizualizeStack { get; set; } = false;
        public bool DisplayFPS { get; set; } = true;
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Maze _maze;
        private IStepableAlgorithm _algorithm;
        private AStarPathFindingAlgorithm _solvingAlgorithm;
        private float _cellWidth;
        private float _cellHeight;
        private Module _noise = new Simplex();
        private double _noiseOffset = 0;
        private SKPaint _bestCellPaint;
        private SKPaint _onStackPaint;
        private SKPaint _wallPaint;
        private SKPaint _currentCellPaint;
        private SKPaint _fpsCounterPaint;
        //private SimpleGameLoop _gameLoop;

        public MainWindow()
        {
            InitializeComponent();

            Settings = new Settings()
            {
                StepsPerUpdate = 10,
                StepsPerUpdateSolving = 10
            };

            PropertyGrid.SelectedObject = Settings;

            var mazeWidth = 50;
            _maze = new Maze(mazeWidth);
            _cellWidth = 2f/mazeWidth;

            _solvingAlgorithm = new AStarPathFindingAlgorithm(_maze);
            _algorithm = new DepthFirstRecursiveBacktrackingMazeAlgorithm(_maze);
            _algorithm = new RandomizedPrimsMazeAlgorithm(_maze);


            _bestCellPaint = CreateFillPaint(SKColors.Yellow);
            _onStackPaint = CreateFillPaint(SKColors.Blue);
            _wallPaint = CreateStrokePaint(SKColors.Blue);
            _currentCellPaint = CreateFillPaint(SKColors.LightBlue);
            _fpsCounterPaint = CreateFillPaint(SKColors.LightBlue);
            _fpsCounterPaint.TextSize = 13;

            CompositionTarget.Rendering += hjhgj;

            //_gameLoop = new SimpleGameLoop(60, DoGameLoopAsync);
            //_gameLoop.Run();
        }

        private void hjhgj(object sender, EventArgs e)
        {
            DoGameLoopAsync();
        }

        public Settings Settings { get; set; }

        private Task DoGameLoopAsync()
        {
            if(!_algorithm.IsFinished)
                _algorithm.Step(Settings.StepsPerUpdate);
            else if(!_solvingAlgorithm.IsFinished)
                _solvingAlgorithm.Step(Settings.StepsPerUpdateSolving);
            _noiseOffset += 0.01;

            //SkiaElement.Dispatcher.Invoke(() => SkiaElement.InvalidateVisual());
            SkiaElement.InvalidateVisual();
            return Task.CompletedTask;
        }

        private void SkiaElement_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
        {
            Draw(e.Info.Width, e.Info.Height, e.Surface.Canvas);
        }

        private void Draw(int width, int height, SKCanvas canvas)
        {
            canvas.Clear(SKColors.Black);

            _cellWidth = ((float)width) / _maze.Width;
            _cellHeight = ((float)height) / _maze.Height;

            Parallel.ForEach(_maze.Cells, cell => { 
            {
                //var v = (float)((_noise.GetValue(cell.X/5d, cell.Y/5d, _noiseOffset)+1) *0.5);
                //float v1 = (127 + (v * 128));
                //SKColor noiseColor = new SKColor(0, 0, (byte)Clamp(v1, 0, 255));
                //_wallPaint.Color = noiseColor;
                //_onStackPaint.Color = noiseColor;

                var topLeftX = cell.X * _cellWidth - 1;
                var topLeftY = cell.Y * _cellHeight - 1;

                if(cell.IsCurrent && (!_algorithm.IsFinished || !_solvingAlgorithm.IsFinished))
                    canvas.DrawRect(topLeftX, topLeftY, _cellWidth, _cellHeight, _currentCellPaint);

                if(cell.IsOnStack && Settings.VizualizeStack)
                    canvas.DrawRect(topLeftX, topLeftY, _cellWidth, _cellHeight, _onStackPaint);

                if(cell.HasBeenVisited)
                {
                    if(cell.HasLeftWall)
                        canvas.DrawLine(topLeftX, topLeftY, topLeftX, topLeftY + _cellHeight, _wallPaint);
                    if(cell.HasTopWall)
                        canvas.DrawLine(topLeftX, topLeftY, topLeftX + _cellWidth, topLeftY, _wallPaint);
                    if(cell.HasRightWall)
                        canvas.DrawLine(topLeftX + _cellWidth, topLeftY, topLeftX + _cellWidth, topLeftY + _cellHeight, _wallPaint);
                    if(cell.HasBottomWall)
                        canvas.DrawLine(topLeftX, topLeftY + _cellHeight, topLeftX + _cellWidth, topLeftY + _cellHeight, _wallPaint);
                }
            }
                });

            var bestPath = _solvingAlgorithm.CurrentBestPath;

            for(int i = 0;i < bestPath.Count - 1;i++)
            {
                var currentCellX = CellMiddleX(bestPath[i], _cellWidth);
                var currentCellY = CellMiddleY(bestPath[i], _cellHeight);

                var nextCellX = CellMiddleX(bestPath[i+1], _cellWidth);
                var nextCellY = CellMiddleY(bestPath[i+1], _cellHeight);

                canvas.DrawLine(currentCellX, currentCellY, nextCellX, nextCellY, _bestCellPaint);
            }
            
            if(Settings.DisplayFPS)
            {
                //canvas.DrawText(_gameLoop.CurrentAverageFPS.ToString(), new SKPoint(20, 20), _fpsCounterPaint);
            }

            canvas.Flush();
        }

        private float CellMiddleX(MazeCell cell, float cellWidth) => (cell.X + 0.5f) * cellWidth;
        private float CellMiddleY(MazeCell cell, float cellHeight) => (cell.Y + 0.5f) * cellHeight;

        private float Clamp(float v1, float min, float max)
        {
            return Math.Min(Math.Max(v1, min), max);
        }

        private static SKPaint CreateStrokePaint(SKColor color)
        {
            return new SKPaint() { Color = color, Style = SKPaintStyle.Stroke };
        }

        private static SKPaint CreateFillPaint(SKColor color)
        {
            return new SKPaint() { Color = color, Style = SKPaintStyle.StrokeAndFill };
        }
    }
}
