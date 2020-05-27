using Kopfsick.CreativeCoding.Algorithms;
using MvvmCross.Commands;
using RandomMazeGenerator.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RandomMazeGenerator.WPF
{
    public class MainViewModel : NotifyPropertyChangedBase
    {
        private Maze _maze;
        private int _width;

        public MainViewModel()
        {
            Width = 30;
            StartCommand = new MvxAsyncCommand(Start, CanStart, true);
            SelectedAlgorithm = Algorithms.First();
        }

        public IDictionary<string, Func<Maze, IStepableAlgorithm>> Algorithms {get;} = new Dictionary<string, Func<Maze, IStepableAlgorithm>>
        {
            { "Recursive Backtracker", m => new DepthFirstRecursiveBacktrackingMazeAlgorithm(m) },
            { "Randomized Prim's Algorithm", m => new RandomizedPrimsMazeAlgorithm(m) }
        };

        public KeyValuePair<string, Func<Maze, IStepableAlgorithm>> SelectedAlgorithm {get;set;}

        private bool CanStart()
        {
            return true;
        }

        private async Task Start()
        {
            Maze = new Maze(Width);
            await Task.Run(async () => await SelectedAlgorithm.Value(Maze).Run(2000/(Maze.Cells.Length*2), 1)).ConfigureAwait(false);
        }

        public ICommand StartCommand { get; set; }

        public int Width
        {
            get { return _width; }
            set
            {
                if(_width != value)
                {
                    _width = value;
                    OnPropertyChanged(nameof(Width));
                }
            }
        }

        public Maze Maze
        {
            get { return _maze; }
            set
            {
                if(_maze != value)
                {
                    _maze = value;
                    OnPropertyChanged(nameof(Maze));
                }
            }
        }
    }
}
