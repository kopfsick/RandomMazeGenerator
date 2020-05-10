using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using RandomMazeGenerator.WPF;
using System;
using System.Drawing;

namespace RandomMazeGenerator.OpenTK
{
    class Program
    {
        static void Main(string[] args)
        {
            var window = new Window();

            window.Run();
        }
    }

    public class Window : GameWindow
    {
        private Maze _maze;
        private IStepableAlgorithm _algorithm;
        private float _cellWidth;
        private int _stepsPerUpdate = 1;
        private bool _visualizeStack = false;

        public Window() : base(720, 720, GraphicsMode.Default, "Random Maze Generator")
        {
            VSync = VSyncMode.On;
            TargetRenderFrequency = 60;
            TargetUpdateFrequency = 60;

            _visualizeStack = false;
            _stepsPerUpdate = 1;
            var mazeWidth = 30;
            _maze = new Maze(mazeWidth);
            _cellWidth = 2f/mazeWidth;

            _algorithm = new DepthFirstRecursiveBacktrackingMazeAlgorithm(_maze);
            _algorithm = new RandomizedPrimsMazeAlgorithm(_maze);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(ClientRectangle);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            _algorithm.Step(_stepsPerUpdate);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            foreach(var cell in _maze.Cells)
            {
                var topLeftX = cell.X * _cellWidth -1;
                var topLeftY = cell.Y * _cellWidth -1;

                if(cell.IsCurrent)
                {
                    GL.Color3(Color.LightBlue);
                    GL.Begin(PrimitiveType.Quads);
                    GL.Vertex2(topLeftX, topLeftY);
                    GL.Vertex2(topLeftX+_cellWidth, topLeftY);
                    GL.Vertex2(topLeftX+_cellWidth, topLeftY+_cellWidth);
                    GL.Vertex2(topLeftX, topLeftY+_cellWidth);
                    GL.End();
                }

                if(cell.IsOnStack && _visualizeStack)
                {
                    var color = Color.Gray;
                    var radius = _cellWidth / 4;
                    DrawCircle(topLeftX+ _cellWidth/2, topLeftY+_cellWidth/2, radius, new Color4(color.R, color.G, color.B, color.A));
                }

                if(cell.HasBeenVisited)
                {
                    GL.Begin(PrimitiveType.Lines);
                    GL.Color3(Color.Blue);

                    if(cell.HasLeftWall)
                    {
                        GL.Vertex2(topLeftX, topLeftY);
                        GL.Vertex2(topLeftX, topLeftY + _cellWidth);
                    }

                    if(cell.HasTopWall)
                    {
                        GL.Vertex2(topLeftX, topLeftY);
                        GL.Vertex2(topLeftX + _cellWidth, topLeftY);
                    }
                    if(cell.HasRightWall)
                    {
                        GL.Vertex2(topLeftX + _cellWidth, topLeftY);
                        GL.Vertex2(topLeftX + _cellWidth, topLeftY + _cellWidth);
                    }
                    if(cell.HasBottomWall)
                    {
                        GL.Vertex2(topLeftX, topLeftY + _cellWidth);
                        GL.Vertex2(topLeftX + _cellWidth, topLeftY + _cellWidth);
                    }
                    GL.End(); 
                }
                else
                {
                    GL.Color3(Color.Blue);
                    GL.Begin(PrimitiveType.Quads);
                    GL.Vertex2(topLeftX, topLeftY);
                    GL.Vertex2(topLeftX+_cellWidth, topLeftY);
                    GL.Vertex2(topLeftX+_cellWidth, topLeftY+_cellWidth);
                    GL.Vertex2(topLeftX, topLeftY+_cellWidth);
                    GL.End();
                }
            }

            SwapBuffers();
        }

        public static void DrawCircle(float x, float y, float radius, Color4 c)
    {
        GL.Begin(PrimitiveType.TriangleFan);
        GL.Color4(c);

        GL.Vertex2(x, y);
        for (int i = 0; i < 360; i++)
        {
            GL.Vertex2(x + Math.Cos(i) * radius, y + Math.Sin(i) * radius);
        }

        GL.End();
    }
    }
}
