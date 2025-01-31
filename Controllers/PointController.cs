using Exam12_Pu5L.Models;
using System.Diagnostics;
using System.Windows;

namespace Exam12_Pu5L.Controllers
{
    public class PointController
    {
        public HashSet<(int X, int Y)> alignedPoints { get; private set; }
        public List<HashSet<(int X, int Y)>> previousAlignments = new List<HashSet<(int X, int Y)>>();
        public (int X, int Y)? suggestedMove { get; private set; }


        public PointController()
        {
            alignedPoints = new HashSet<(int X, int Y)>();
            suggestedMove = null;
        }

        //-----OLD
        //-----------ALIGNEMENT
        public bool CheckForAlignment(int x, int y, Player _player, int _nbRow, int _nbCol, int _howManyAligned)
        {
            if (CheckHorizontal((x, y), _player, _nbRow, _howManyAligned) || CheckVertical((x, y), _player, _nbCol, _howManyAligned) ||
                CheckDiagonal((x, y), _player, _howManyAligned)) return true;
            return false;
        }

        private bool CheckVertical((int x, int y) point, Player _player, int _nbRow, int _howManyAligned)
        {
            int count = 1;
            alignedPoints.Clear();
            alignedPoints.Add((point.x, point.y));

            for (int i = 1; i <= _howManyAligned - 1 && point.y - i >= 0; i++)
            {
                if (_player.Has_point(point.x, point.y - i))
                {
                    count++;
                    alignedPoints.Add((point.x, point.y - i));
                }
                else break;
            }

            if (count >= _howManyAligned) return true;
            for (int i = 1; i <= _howManyAligned - 1 && point.y + i < _nbRow; i++)
            {
                if (_player.Has_point(point.x, point.y + i))
                {
                    count++;
                    alignedPoints.Add((point.x, point.y + i));
                }
                else break;
            }

            return count >= _howManyAligned;
        }

        private bool CheckHorizontal((int x, int y) point, Player _player, int _nbCol, int _howManyAligned)
        {
            int count = 1;
            alignedPoints.Clear();
            alignedPoints.Add((point.x, point.y));

            for (int i = 1; i <= _howManyAligned - 1 && point.x - i >= 0; i++)
            {
                if (_player.Has_point(point.x - i, point.y))
                {
                    count++;
                    alignedPoints.Add((point.x - i, point.y));
                }
                else break;
            }

            if (count >= _howManyAligned) return true;
            for (int i = 1; i <= _howManyAligned - 1 && point.x + i < _nbCol; i++)
            {
                if (_player.Has_point(point.x + i, point.y))
                {
                    count++;
                    alignedPoints.Add((point.x + i, point.y));
                }
                else break;
            }

            return count >= _howManyAligned;
        }

        private bool CheckDiagonal((int x, int y) point, Player _player, int _howManyAligned)
        {
            //------- Check top-left to bottom-right ↖↘
            int count = 1;
            alignedPoints.Clear();
            alignedPoints.Add((point.x, point.y));
            // Check top-left diagonal
            for (int i = 1; i <= _howManyAligned - 1; i++)
            {
                if (_player.Has_point(point.x - i, point.y - i))
                {
                    count++;
                    alignedPoints.Add((point.x - i, point.y - i));
                }
                else break;
            }

            // Check bottom-right diagonal
            for (int i = 1; i <= _howManyAligned - 1; i++)
            {
                if (_player.Has_point(point.x + i, point.y + i))
                {
                    count++;
                    alignedPoints.Add((point.x + i, point.y + i));
                }
                else break;
            }
            if (count >= _howManyAligned) return true;

            //------- Check top-right to bottom-left ↗↙
            count = 1;
            alignedPoints.Clear();
            alignedPoints.Add((point.x, point.y));
            // Check top-right diagonal
            for (int i = 1; i <= _howManyAligned - 1; i++)
            {
                if (_player.Has_point(point.x - i, point.y + i))
                {
                    count++;
                    alignedPoints.Add((point.x - i, point.y + i));
                }
                else break;
            }

            // Check bottom-left diagonal
            for (int i = 1; i <= _howManyAligned - 1; i++)
            {
                if (_player.Has_point(point.x + i, point.y - i))
                {
                    count++;
                    alignedPoints.Add((point.x + i, point.y - i));
                }
                else break;
            }
            return count >= _howManyAligned;
        }


        //-----------SUGGESTION
        public (int X, int Y)? FindCorrectMoveOld(Player you, Player opponent, int nbRow, int nbCol, int alignmentNeeded)
        {
            alignmentNeeded += 1;
            // Check each empty cell on the grid
            for (int x = 0; x < nbCol; x++)
            {
                for (int y = 0; y < nbRow; y++)
                {
                    if (!you.Has_point(x, y) && !opponent.Has_point(x, y))
                    {
                        if (CheckForAlignment(x, y, you, nbRow, nbCol, alignmentNeeded))
                        {
                            return (x, y);
                        }
                    }
                }
            }
            return null;
        }

        public (int X, int Y)? GetSuggestionOld(Player currentPlayer, Player opponent, int nbRow, int nbCol)
        {
            var checkmateMove = FindCorrectMove(currentPlayer, opponent, nbRow, nbCol, 4);
            if (checkmateMove.HasValue)
            {
                suggestedMove = checkmateMove;
                return checkmateMove;
            }

            // First priority: Check if opponent is about to win (has 4 aligned)
            var blockingMove = FindCorrectMove(opponent, currentPlayer, nbRow, nbCol, 4);
            if (blockingMove.HasValue)
            {
                suggestedMove = blockingMove;
                return blockingMove;
            }

            // Second priority: Check if we can win (get 3 aligned)
            var winningMove = FindCorrectMove(currentPlayer, opponent, nbRow, nbCol, 3);
            if (winningMove.HasValue)
            {
                suggestedMove = winningMove;
                return winningMove;
            }

            return null;
        }


        //------EXA12
        //-----------ALIGNEMENT
        public bool CheckForAlignmentL(int x, int y, Player _player, int _nbRow, int _nbCol, int _howManyAligned)
        {
            return (CheckLShape((y, x), _player, _nbRow, _nbCol, _howManyAligned));
        }

        public bool CheckLShape((int x, int y) point, Player _player, int _nbRow, int _nbCol, int _howManyAligned)
        {
            //Debug.WriteLine($"Checking L-shape for point: ({point.x}, {point.y})");

            //--- Clear alignedPoints before starting
            alignedPoints.Clear();

            //--- Check for horizontal alignment first
            //Debug.WriteLine("Checking horizontal alignment...heeeeeeere");
            if (CheckHorizontalLine(point, _player, _nbCol, _howManyAligned))
            {
                //Debug.WriteLine($"Horizontal alignment found: {FormatAlignedPoints(alignedPoints)}");

                if(alignedPoints.Count > 1)
                {
                    //--- Iterate through each point in the horizontal alignment and check for vertical perpendicular
                    foreach (var alignedPoint in alignedPoints.ToList())
                    {
                        //Debug.WriteLine($"Checking perpendicular for point: ({alignedPoint.X}, {alignedPoint.Y})");
                        if (CheckVerticalPerpendicular(alignedPoint, _player, _nbRow, _howManyAligned - alignedPoints.Count))
                        {
                            //Debug.WriteLine($"L-shape found! Aligned points: {FormatAlignedPoints(alignedPoints)}");
                            return true;
                        }
                    }
                }
            }

            //--- Clear alignedPoints before checking vertical alignment
            alignedPoints.Clear();

            //--- Check for vertical alignment next
            //Debug.WriteLine("Checking vertical alignment...bruhhhhhhhhh");
            if (CheckVerticalLine(point, _player, _nbRow, _howManyAligned))
            {
                //Debug.WriteLine($"Vertical alignment found: {FormatAlignedPoints(alignedPoints)}");

                if (alignedPoints.Count > 1)
                {
                    // Iterate through each point in the vertical alignment and check for horizontal perpendicular
                    foreach (var alignedPoint in alignedPoints.ToList())
                    {
                        //Debug.WriteLine($"Checking perpendicular for point: ({alignedPoint.X}, {alignedPoint.Y})");
                        if (CheckHorizontalPerpendicular(alignedPoint, _player, _nbCol, _howManyAligned - alignedPoints.Count))
                        {
                            //Debug.WriteLine($"L-shape found! Aligned points: {FormatAlignedPoints(alignedPoints)}");
                            return true;
                        }
                    }
                }
            }

            //Debug.WriteLine("No L-shape found.");
            return false;
        }

        private bool CheckHorizontalLine((int x, int y) point, Player _player, int _nbCol, int length)
        {
            //Debug.WriteLine($"Checking horizontal line for point: ({point.x}, {point.y})");

            int count = 1;
            alignedPoints.Clear();
            alignedPoints.Add((point.x, point.y));

            // Check to the left
            for (int i = 1; i < length && point.x - i >= 0; i++)
            {
                if (_player.Has_point(point.x - i, point.y))
                {
                    count++;
                    alignedPoints.Add((point.x - i, point.y));
                    //Debug.WriteLine($"Added point to alignedPoints horizontal alignment: ({point.x - i}, {point.y})");
                }
                else break;
            }

            // Check to the right
            for (int i = 1; i < length && point.x + i < _nbCol; i++)
            {
                if (_player.Has_point(point.x + i, point.y))
                {
                    count++;
                    alignedPoints.Add((point.x + i, point.y));
                    //Debug.WriteLine($"Added point to alignedPoints horizontal alignment: ({point.x + i}, {point.y})");
                }
                else break;
            }

            //Debug.WriteLine($"Horizontal alignment count: {count}");
            return count < length;
        }

        private bool CheckVerticalLine((int x, int y) point, Player _player, int _nbRow, int length)
        {
            //Debug.WriteLine($"Checking vertical line for point: ({point.x}, {point.y})");

            int count = 1;
            alignedPoints.Clear();
            alignedPoints.Add((point.x, point.y));

            // Check upwards
            for (int i = 1; i < length && point.y - i >= 0; i++)
            {
                if (_player.Has_point(point.x, point.y - i))
                {
                    count++;
                    alignedPoints.Add((point.x, point.y - i));
                    //Debug.WriteLine($"Added point to alignedPoints vertical alignment: ({point.x}, {point.y - i})");
                }
                else break;
            }

            // Check downwards
            for (int i = 1; i < length && point.y + i < _nbRow; i++)
            {
                if (_player.Has_point(point.x, point.y + i))
                {
                    count++;
                    alignedPoints.Add((point.x, point.y + i));
                    //Debug.WriteLine($"Added point to alignedPoints vertical alignment: ({point.x}, {point.y + i})");
                }
                else break;
            }

            //Debug.WriteLine($"Vertical alignment count: {count}");
            return count < length;
        }

        private bool CheckHorizontalPerpendicular((int x, int y) point, Player _player, int _nbCol, int length)
        {
            //Debug.WriteLine($"Checking horizontal perpendicular for point: ({point.x}, {point.y})");

            int count = 0;
            var tempPoints = new HashSet<(int X, int Y)>();

            // Check to the left (excluding the current point)
            for (int i = 1; i <= length && point.x - i >= 0; i++)
            {
                if (_player.Has_point(point.x - i, point.y))
                {
                    count++;
                    tempPoints.Add((point.x - i, point.y));
                    //Debug.WriteLine($"Added point to horizontal perpendicular: ({point.x - i}, {point.y})");
                }
                else break;
            }

            // Check to the right (excluding the current point)
            for (int i = 1; i <= length && point.x + i < _nbCol; i++)
            {
                if (_player.Has_point(point.x + i, point.y))
                {
                    count++;
                    tempPoints.Add((point.x + i, point.y));
                    //Debug.WriteLine($"Added point to horizontal perpendicular: ({point.x + i}, {point.y})");
                }
                else break;
            }

            //Debug.WriteLine($"Horizontal perpendicular count: {count}");
            if (count >= length)
            {
                // Add the perpendicular points to alignedPoints
                foreach (var tempPoint in tempPoints)
                {
                    alignedPoints.Add(tempPoint);
                }
                return true;
            }

            return false;
        }

        private bool CheckVerticalPerpendicular((int x, int y) point, Player _player, int _nbRow, int length)
        {
            //Debug.WriteLine($"Checking vertical perpendicular for point: ({point.x}, {point.y})");

            int count = 0;
            var tempPoints = new HashSet<(int X, int Y)>();

            // Check upwards (excluding the current point)
            for (int i = 1; i <= length && point.y - i >= 0; i++)
            {
                if (_player.Has_point(point.x, point.y - i))
                {
                    count++;
                    tempPoints.Add((point.x, point.y - i));
                    //Debug.WriteLine($"Added point to vertical perpendicular: ({point.x}, {point.y - i})");
                }
                else break;
            }

            // Check downwards (excluding the current point)
            for (int i = 1; i <= length && point.y + i < _nbRow; i++)
            {
                if (_player.Has_point(point.x, point.y + i))
                {
                    count++;
                    tempPoints.Add((point.x, point.y + i));
                    //Debug.WriteLine($"Added point to vertical perpendicular: ({point.x}, {point.y + i})");
                }
                else break;
            }

            //Debug.WriteLine($"Vertical perpendicular count: {count}");
            if (count >= length)
            {
                // Add the perpendicular points to alignedPoints
                foreach (var tempPoint in tempPoints)
                {
                    alignedPoints.Add(tempPoint);
                }
                return true;
            }

            return false;
        }

        private string FormatAlignedPoints(HashSet<(int X, int Y)> alignedPoints)
        {
            return string.Join(", ", alignedPoints.Select(p => $"({p.X}, {p.Y})"));
        }

        //-----------SUGGESTION
        public (int X, int Y)? FindCorrectMove(Player you, Player opponent, int nbRow, int nbCol, int alignmentNeeded)
        {
            alignmentNeeded += 1;
            // Check each empty cell on the grid
            for (int x = 0; x < nbCol; x++)
            {
                for (int y = 0; y < nbRow; y++)
                {
                    if (!you.Has_point(x, y) && !opponent.Has_point(x, y))
                    {
                        if (CheckForAlignmentL(x, y, you, nbRow, nbCol, alignmentNeeded))
                        {
                            return (x, y);
                        }
                    }
                }
            }
            return null;
        }

        public (int X, int Y)? GetSuggestion(Player currentPlayer, Player opponent, int nbRow, int nbCol)
        {
            // First priority: Check if we can win (complete an L-shape of 5 points)
            var winningMove = FindCorrectMove(currentPlayer, opponent, nbRow, nbCol, 4);
            if (winningMove.HasValue)
            {
                suggestedMove = winningMove;
                return winningMove;
            }

            // Second priority: Check if the opponent is about to win (has an L-shape of 4 points)
            var blockingMove = FindCorrectMove(opponent, currentPlayer, nbRow, nbCol, 4);
            if (blockingMove.HasValue)
            {
                suggestedMove = blockingMove;
                return blockingMove;
            }

            // Third priority: Suggest a move that helps build towards an L-shape of 5 points
            var strategicMove = FindCorrectMove(currentPlayer, opponent, nbRow, nbCol, 3);
            if (strategicMove.HasValue)
            {
                suggestedMove = strategicMove;
                return strategicMove;
            }
            return null;
        }

    }
}
