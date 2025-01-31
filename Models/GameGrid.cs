using System;

namespace Exam12_Pu5L.Models
{
    public class GameGrid
    {
        public int rows { get; private set; }
        public int columns { get; private set; }
        private int?[,] cells; // State of the grid (null = empty, otherwise stores the Player's ID)

        public GameGrid(int _rows, int _columns)
        {
            if (_rows <= 5 || _columns <= 5)
            {
                throw new ArgumentException("Grid dimensions must be greater than 5x5.");
            }

            rows = _rows;
            columns = _columns;
            cells = new int?[_rows, _columns];
        }

        //-------LOGIC
        public bool Is_cell_available(int _row, int _column)
        {
            Validate_cell(_row, _column);
            return cells[_row, _column] == null;
        }

        public void Mark_cell(int _row, int _column, int _playerId)
        {
            Validate_cell(_row, _column);

            if (!Is_cell_available(_row, _column))
            {
                throw new InvalidOperationException("Cell is already occupied.");
            }

            cells[_row, _column] = _playerId;
        }

        public void Unmark_cell(int row, int column)
        {
            cells[row, column] = null; // Reset the cell value to indicate it is no longer occupied.
        }

        public int[] UpdateCells(int[] _cell)
        {
            if (_cell.Length != rows * columns)
            {
                throw new ArgumentException("The length of the input array must match the total number of cells in the grid.");
            }

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    cells[i, j] = _cell[i * columns + j];
                }
            }
            return _cell;
        }

        public int? Get_cell_owner(int _row, int _column)
        {
            Validate_cell(_row, _column);
            return cells[_row, _column];
        }

        public void Reset_grid()
        {
            cells = new int?[rows, columns];
        }

        private void Validate_cell(int _row, int _column)
        {
            if (_row < 0 || _row >= rows || _column < 0 || _column >= columns)
            {
                throw new ArgumentOutOfRangeException("Cell coordinates are out of bounds.");
            }
        }
    }
}
