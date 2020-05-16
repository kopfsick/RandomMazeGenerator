namespace RandomMazeGenerator.WPF
{
    public abstract class StepableMazeAlgorithmBase : StepableAlgorithmBase
    {
        private MazeCell _currentCell;

        protected void SetCurrentCell(MazeCell cell)
        {
            if(_currentCell != null)
                _currentCell.IsCurrent = false;
            if(cell != null)
                cell.IsCurrent = true;
            _currentCell = cell;
        }
    }
}
