using Kopfsick.CreativeCoding.Algorithms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using RandomMazeGenerator.Core;

namespace RandomMazeGenerator.MonoGame;

public class MazeGeneratorGame : Game
{
    private readonly GraphicsDeviceManager _graphicsDeviceManager;
    private SpriteBatch? _spriteBatch;

    private Maze _maze;
    private IStepableAlgorithm _algorithm;
    private AStarPathFindingAlgorithm _solvingAlgorithm;
    private float _cellWidth;
    private int _stepsPerUpdate;
    private int _stepsPerUpdateSolving;
    private bool _visualizeStack;

    public MazeGeneratorGame()
    {
        _graphicsDeviceManager = new GraphicsDeviceManager(this);
        IsMouseVisible = true;

        _visualizeStack = true;
        _stepsPerUpdate = 10;
        _stepsPerUpdateSolving = 3;
        var mazeWidth = 100;
        _maze = new Maze(mazeWidth);
        _cellWidth = 800/mazeWidth;
        _solvingAlgorithm = new AStarPathFindingAlgorithm(_maze);
        _algorithm = new DepthFirstRecursiveBacktrackingMazeAlgorithm(_maze);
        //_algorithm = new RandomizedPrimsMazeAlgorithm(_maze);
    }

    protected override void Initialize()
    {
        base.Initialize();
        _spriteBatch = new SpriteBatch(_graphicsDeviceManager.GraphicsDevice);

        _graphicsDeviceManager.PreferredBackBufferWidth = 800;
        _graphicsDeviceManager.PreferredBackBufferHeight = 800;
        _graphicsDeviceManager.IsFullScreen = false;
        _graphicsDeviceManager.ApplyChanges();


    }

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        if(!_algorithm.IsFinished)
            _algorithm.Step(_stepsPerUpdate);
        else if(!_solvingAlgorithm.IsFinished)
            _solvingAlgorithm.Step(_stepsPerUpdateSolving);
    }

    protected override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        _graphicsDeviceManager.GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin();

        foreach (var cell in _maze.Cells)
        {
            var topLeftX = cell.X * _cellWidth - 1;
            var topLeftY = cell.Y * _cellWidth - 1;

            if (cell.IsCurrent && (!_algorithm.IsFinished || !_solvingAlgorithm.IsFinished))
                _spriteBatch.DrawRectangle(topLeftX, topLeftY, _cellWidth, _cellWidth, Color.Red, 5);

            if (cell.IsOnStack && _visualizeStack)
                _spriteBatch.DrawCircle(topLeftX + _cellWidth / 2, topLeftY + _cellWidth / 2, _cellWidth / 4, 50,
                    Color.Red);

            if (cell.HasBeenVisited)
            {
                var lineColor = Color.DarkOrange;
                var lineThickness = 1f;

                if (cell.HasLeftWall)
                    _spriteBatch.DrawLine(topLeftX, topLeftY, topLeftX, topLeftY + _cellWidth, lineColor, lineThickness);

                if (cell.HasTopWall)
                    _spriteBatch.DrawLine(topLeftX, topLeftY, topLeftX + _cellWidth, topLeftY, lineColor, lineThickness);

                if (cell.HasRightWall)
                    _spriteBatch.DrawLine(topLeftX + _cellWidth, topLeftY, topLeftX + _cellWidth, topLeftY + _cellWidth, lineColor, lineThickness);

                if (cell.HasBottomWall)
                    _spriteBatch.DrawLine(topLeftX, topLeftY + _cellWidth, topLeftX + _cellWidth, topLeftY + _cellWidth, lineColor, lineThickness);
            }
            //else
            //{
            //    var v = (float)((_noise.GetValue(cell.X/5d, cell.Y/5d, _noiseOffset)+1) *0.5);
            //    GL.Color3((1-v)*0.333,(1-v)*0.666, v);
            //    GL.Color3(0,0, 0.5 + v*0.5);
            //    GL.Begin(PrimitiveType.Quads);
            //    GL.Vertex2(topLeftX, topLeftY);
            //    GL.Vertex2(topLeftX+_cellWidth, topLeftY);
            //    GL.Vertex2(topLeftX+_cellWidth, topLeftY+_cellWidth);
            //    GL.Vertex2(topLeftX, topLeftY+_cellWidth);
            //    GL.End();
            //}


            //Solving visualization
            //if(_solvingAlgorithm.IsInOpenSet(cell))
            //{
            //    GL.Color3(Color.Red);
            //    GL.Begin(PrimitiveType.Quads);
            //    GL.Vertex2(topLeftX, topLeftY);
            //    GL.Vertex2(topLeftX+_cellWidth, topLeftY);
            //    GL.Vertex2(topLeftX+_cellWidth, topLeftY+_cellWidth);
            //    GL.Vertex2(topLeftX, topLeftY+_cellWidth);
            //    GL.End();
            //}
            if (_solvingAlgorithm.IsOnBestPath(cell))
            {
                var offset = _cellWidth / 10;
                _spriteBatch.DrawRectangle(topLeftX + offset, topLeftY + offset, _cellWidth - offset,
                    _cellWidth - offset, Color.Blue);
            }
        }

        _spriteBatch.End();
    }
}