namespace RandomMazeGenerator.Core
{
    public class MazeCell : NotifyPropertyChangedBase
    {
        private bool _hasLeftWall;
        private bool _hasTopWall;
        private bool _hasRightWall;
        private bool _hasBottomWall;
        private bool _hasBeenVisited;
        private bool _isCurrent;
        private bool _isOnStack;

        public MazeCell(int x, int y)
        {
            X = x;
            Y = y;

            HasLeftWall = true;
            HasTopWall = true;
            HasRightWall = true;
            HasBottomWall = true;
        }

        public int X { get; }
        public int Y { get; }

        public bool IsOnStack
        {
            get { return _isOnStack; }
            set
            {
                if(_isOnStack != value)
                {
                    _isOnStack = value;
                    OnPropertyChanged(nameof(IsOnStack));
                }
            }
        }

        public bool IsCurrent
        {
            get { return _isCurrent; }
            set
            {
                if(_isCurrent != value)
                {
                    _isCurrent = value;
                    OnPropertyChanged(nameof(IsCurrent));
                }
            }
        }

        public bool HasBeenVisited
        {
            get { return _hasBeenVisited; }
            set
            {
                if(_hasBeenVisited != value)
                {
                    _hasBeenVisited = value;
                    OnPropertyChanged(nameof(HasBeenVisited));
                }
            }
        }


        public bool HasLeftWall
        {
            get { return _hasLeftWall; }

            set
            {
                if(_hasLeftWall != value)
                {
                    _hasLeftWall = value;
                    OnPropertyChanged(nameof(HasLeftWall));
                }
            }
        }

        public bool HasTopWall
        {
            get { return _hasTopWall; }

            set
            {
                if(_hasTopWall != value)
                {
                    _hasTopWall = value;
                    OnPropertyChanged(nameof(HasTopWall));
                }
            }
        }

        public bool HasRightWall
        {
            get { return _hasRightWall; }

            set
            {
                if(_hasRightWall != value)
                {
                    _hasRightWall = value;
                    OnPropertyChanged(nameof(HasRightWall));
                }
            }
        }

        public bool HasBottomWall
        {
            get { return _hasBottomWall; }

            set
            {
                if(_hasBottomWall != value)
                {
                    _hasBottomWall = value;
                    OnPropertyChanged(nameof(HasBottomWall));
                }
            }
        }
    }
}