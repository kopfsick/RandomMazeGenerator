using Kopfsick.CreativeCoding.Algorithms;

namespace RandomMazeGenerator.Core
{
    public abstract class StepableMazeAlgorithmBase : StepableAlgorithmBase
    {
        protected MazeCell CurrentCell { get; private set; }

        protected void SetCurrentCell(MazeCell cell)
        {
            if(CurrentCell != null)
                CurrentCell.IsCurrent = false;
            if(cell != null)
                cell.IsCurrent = true;
            CurrentCell = cell;
        }
    }
}
