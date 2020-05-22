using PropertyTools.DataAnnotations;
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
    public class Settings
    {
        [Slidable(1,100, 1, 10, true, 10)]
        [HeaderPlacement(HeaderPlacement.Above)]
        public int StepsPerUpdate { get; set; } = 10;
        
        [Slidable(1,100, 1, 10, true, 10)]
        [HeaderPlacement(HeaderPlacement.Above)]
        public int StepsPerUpdateSolving { get; set; } = 10;

        public bool VizualizeStack { get; set; } = false;
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

        public MainWindow()
        {
            InitializeComponent();

            Settings = new Settings()
            {
                StepsPerUpdate = 10,
                StepsPerUpdateSolving = 10
                };

            PropertyGrid.SelectedObject = Settings;

            var mazeWidth = 100;
            _maze = new Maze(mazeWidth);
            _cellWidth = 2f/mazeWidth;

            _solvingAlgorithm = new AStarPathFindingAlgorithm(_maze);
            _algorithm = new DepthFirstRecursiveBacktrackingMazeAlgorithm(_maze);
            //_algorithm = new RandomizedPrimsMazeAlgorithm(_maze);


            _bestCellPaint = CreateFillPaint(SKColors.Yellow);
            _onStackPaint = CreateFillPaint(SKColors.Blue);
            _wallPaint = CreateStrokePaint(SKColors.Blue);
            _currentCellPaint = CreateFillPaint(SKColors.LightBlue);

            DoGameLoopAsync();
        }

        public Settings Settings { get; set; }

        private async Task DoGameLoopAsync()
        {
            while(true)
            {
                await Task.Run(() => Update());
                _noiseOffset += 0.01;
                SkiaElement.InvalidateVisual();
                await Task.Delay(16);
            }
        }

        private void SkiaControl_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
        {
            SKCanvas canvas = e.Surface.Canvas;

            Draw(e, canvas);
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
                var v = (float)((_noise.GetValue(cell.X/5d, cell.Y/5d, _noiseOffset)+1) *0.5);
                float v1 = (127 + (v * 128));
                _wallPaint.Color = new SKColor(0,0, (byte)Math.Clamp(v1, 0, 255));

                var topLeftX = cell.X * _cellWidth - 1;
                var topLeftY = cell.Y * _cellHeight - 1;

                if(cell.IsCurrent && (!_algorithm.IsFinished || !_solvingAlgorithm.IsFinished))
                {
                    canvas.DrawRect(topLeftX, topLeftY, _cellWidth, _cellHeight, _currentCellPaint);
                }

                if(cell.IsOnStack && Settings.VizualizeStack)
                {
                    float heightOffset = _cellHeight / 10;
                    float widthOffset = _cellWidth / 10;

                    heightOffset = 0;
                    widthOffset = 0;

                    canvas.DrawRect(topLeftX + widthOffset, topLeftY + heightOffset, _cellWidth - widthOffset, _cellHeight - heightOffset, _onStackPaint);
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
            return new SKPaint() { Color = color, Style = SKPaintStyle.Stroke };
        }

        private static SKPaint CreateFillPaint(SKColor color)
        {
            return new SKPaint() { Color = color, Style = SKPaintStyle.StrokeAndFill };
        }

        private void Update()
        {
            if(!_algorithm.IsFinished)
                _algorithm.Step(Settings.StepsPerUpdate);
            else if(!_solvingAlgorithm.IsFinished)
                _solvingAlgorithm.Step(Settings.StepsPerUpdateSolving);
            _noiseOffset += 0.01;
        }

        
    }
}
