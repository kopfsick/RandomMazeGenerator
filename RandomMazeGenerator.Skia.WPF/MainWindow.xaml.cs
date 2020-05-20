using RandomMazeGenerator.WPF;
using SharpNoise.Modules;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RandomMazeGenerator.Skia.WPF
{
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
        private int _stepsPerUpdate = 1;
        private int _stepsPerUpdateSolving;
        private bool _visualizeStack = false;
        private Module _noise = new Simplex();
        private double _noiseOffset = 0;
        private SKPaint _bestCellPaint;
        private SKPaint _wallPaint;
        private SKPaint _currentCellPaint;

        public MainWindow()
        {
            InitializeComponent();

            _visualizeStack = false;
            _stepsPerUpdate = 30;
            _stepsPerUpdateSolving = 10;
            var mazeWidth = 100;
            _maze = new Maze(mazeWidth);
            _cellWidth = 2f/mazeWidth;

            _solvingAlgorithm = new AStarPathFindingAlgorithm(_maze);
            _algorithm = new DepthFirstRecursiveBacktrackingMazeAlgorithm(_maze);
            _algorithm = new RandomizedPrimsMazeAlgorithm(_maze);


            _bestCellPaint = CreateFillPaint(SKColors.Yellow);
            _wallPaint = CreateStrokePaint(SKColors.Blue);
            _currentCellPaint = CreateFillPaint(SKColors.LightBlue);

            DoGameLoopAsync();
        }

        private async Task DoGameLoopAsync()
        {
            while(true)
            {
                await Task.Run(() => Update());
                SkiaElement.InvalidateVisual();
                await Task.Delay(16);
            }
        }

        private void SkiaElement_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
        {
            SKCanvas canvas = e.Surface.Canvas;

            Draw(e, canvas);
        }

        private void Draw(SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e, SKCanvas canvas)
        {
            canvas.Clear(SKColors.Black);

            _cellWidth = ((float)e.Info.Width) / _maze.Width;
            _cellHeight = ((float)e.Info.Height) / _maze.Height;

            foreach(var cell in _maze.Cells)
            {
                var topLeftX = cell.X * _cellWidth - 1;
                var topLeftY = cell.Y * _cellHeight - 1;

                if(cell.IsCurrent && (!_algorithm.IsFinished || !_solvingAlgorithm.IsFinished))
                {
                    canvas.DrawRect(topLeftX, topLeftY, _cellWidth, _cellHeight, _currentCellPaint);
                }

                if(cell.IsOnStack && _visualizeStack)
                {
                    //Visualize on stack
                }

                if(cell.HasBeenVisited)
                {
                    if(cell.HasLeftWall)
                    {
                        canvas.DrawLine(topLeftX, topLeftY, topLeftX, topLeftY + _cellHeight, _wallPaint);
                    }
                    if(cell.HasTopWall)
                    {
                        canvas.DrawLine(topLeftX, topLeftY, topLeftX + _cellWidth, topLeftY, _wallPaint);
                    }
                    if(cell.HasRightWall)
                    {
                        canvas.DrawLine(topLeftX + _cellWidth, topLeftY, topLeftX + _cellWidth, topLeftY + _cellHeight, _wallPaint);
                    }
                    if(cell.HasBottomWall)
                    {
                        canvas.DrawLine(topLeftX, topLeftY + _cellHeight, topLeftX + _cellWidth, topLeftY + _cellHeight, _wallPaint);
                    }
                }

                if(_solvingAlgorithm.IsOnBestPath(cell))
                {
                    float heightOffset = _cellHeight / 10;
                    float widthOffset = _cellWidth / 10;


                    canvas.DrawRect(topLeftX + widthOffset, topLeftY + heightOffset, _cellWidth - widthOffset, _cellHeight - heightOffset, _bestCellPaint);
                }
            }
        }

        private static SKPaint CreateStrokePaint(SKColor color)
        {
            return new SKPaint() { Color = SKColors.HotPink, IsStroke = true, StrokeWidth = 1, Shader = SKShader.CreateColor(color) };
        }

        private static SKPaint CreateFillPaint(SKColor color)
        {
            return new SKPaint() { Color = SKColors.HotPink, IsStroke = false, StrokeWidth = 1, Shader = SKShader.CreateColor(color) };
        }

        private void Update()
        {
            if(!_algorithm.IsFinished)
                _algorithm.Step(_stepsPerUpdate);
            else if(!_solvingAlgorithm.IsFinished)
                _solvingAlgorithm.Step(_stepsPerUpdateSolving);
            _noiseOffset += 0.01;
        }
    }
}
