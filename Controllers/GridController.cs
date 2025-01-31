using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using Exam12_Pu5L.Models;
using System.Windows;

namespace Exam12_Pu5L.Controllers
{
    public class GridController
    {
        private GameGrid gameGrid;
        private double ellipseSize;
        public List<(int X, int Y)> p1Point { get; private set; }
        public List<(int X, int Y)> p2Point { get; private set; }


        public GridController(int _rows, int _columns)
        {
            gameGrid = new GameGrid(_rows, _columns);
            p1Point = new List<(int X, int Y)>();
            p2Point = new List<(int X, int Y)>();
        }

        public bool Place_point(int _row, int _column, Player _player, Grid _grid, bool _limitActivated)
        {
            try
            {
                if (!gameGrid.Is_cell_available(_row, _column)) return false;
                
                if(_limitActivated)
                {
                    if (_player.limit == 0)
                    {
                        RemoveOldestPoint(_player, _grid);
                    }
                }

                // Mark cell as occupied by this player's ID
                gameGrid.Mark_cell(_row, _column, _player.id);
                _player.Add_point(_column, _row);
                AddPointToPlayer(_row, _column, _player);
                Draw_player_view(_row, _column, _player, _grid);
                return true;

            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw new Exception("Cell coordinates are out of bounds.");
            }
        }

        private void Draw_player_view(int _row, int _column, Player _player, Grid _grid)
        {
            double cellWidth = _grid.ActualWidth / _grid.ColumnDefinitions.Count;
            double cellHeight = _grid.ActualHeight / _grid.RowDefinitions.Count;
            ellipseSize = Math.Min(cellWidth, cellHeight) * 0.4;

            Ellipse playerView = new Ellipse
            {
                Width = ellipseSize,
                Height = ellipseSize,
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(_player.color)),
                Tag = $"{_column},{_row}" // Add a unique tag for identification
            };

            Grid.SetRow(playerView, _row);
            Grid.SetColumn(playerView, _column);

            _grid.Children.Add(playerView);

        }

        public void Highlight_aligned(HashSet<(int X, int Y)> alignedPoints, Grid _grid)
        {
            ClearHighlights(_grid);

            foreach ((int x, int y) in alignedPoints)
            {
                Ellipse highlight = new Ellipse
                {
                    Width = ellipseSize + 2,
                    Height = ellipseSize + 2,
                    Stroke = Brushes.Gold,
                    StrokeThickness = 3,
                    Fill = Brushes.Transparent
                };

                // Add a glow effect
                highlight.Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color = Colors.Gold,
                    BlurRadius = 10,
                    ShadowDepth = 0
                };

                Grid.SetColumn(highlight, x);
                Grid.SetRow(highlight, y);

                highlight.Margin = new Thickness(-2);  // (24-20)/2 = 2 to center

                _grid.Children.Add(highlight);
            }
        }

        public void Highlight_aligned_continuous(List<HashSet<(int X, int Y)>> allAlignedPoints, Grid _grid)
        {
            //ClearHighlights(_grid);

            foreach (var alignedSet in allAlignedPoints)
            {
                foreach ((int x, int y) in alignedSet)
                {
                    Ellipse highlight = new Ellipse
                    {
                        Width = ellipseSize + 2,
                        Height = ellipseSize + 2,
                        Stroke = Brushes.Gold,
                        StrokeThickness = 3,
                        Fill = Brushes.Transparent
                    };

                    highlight.Effect = new System.Windows.Media.Effects.DropShadowEffect
                    {
                        Color = Colors.Gold,
                        BlurRadius = 10,
                        ShadowDepth = 0
                    };

                    Grid.SetColumn(highlight, x);
                    Grid.SetRow(highlight, y);
                    highlight.Margin = new Thickness(-2);

                    _grid.Children.Add(highlight);
                }
            }
        }

        private void ClearHighlights(Grid _grid)
        {
            _grid.Children.Cast<UIElement>()
                .Where(element => element is Ellipse ellipse && ellipse.Fill == Brushes.Transparent)
                .ToList()
                .ForEach(element => _grid.Children.Remove(element));
        }

        //----ALEA
        private void AddPointToPlayer(int _row, int _column, Player _player)
        {
            if (_player.id == 1)
            {
                p1Point.Add((_column, _row));
            }
            else if (_player.id == 2)
            {
                p2Point.Add((_column, _row));
            }
            else
            {
                MessageBox.Show($"Invalid player ID: {_player.id}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public bool RemoveOldestPoint(Player _player, Grid _grid)
        {
            try
            {
                if (_player.limit > 0 || (_player.id == 1 && p1Point.Count == 0) || (_player.id == 2 && p2Point.Count == 0))
                {
                    return false;
                }

                (int X, int Y) pointToRemove;
                if (_player.id == 1)
                {
                    pointToRemove = p1Point.First();
                    p1Point.RemoveAt(0);
                }
                else if (_player.id == 2)
                {
                    pointToRemove = p2Point.First();
                    p2Point.RemoveAt(0);
                }
                else
                {
                    MessageBox.Show($"Invalid player ID: {_player.id}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                // Remove the point from the player's HashSet.
                _player.points.Remove(pointToRemove);

                // Get the row and column of the point.
                int row = pointToRemove.Y;
                int column = pointToRemove.X;

                gameGrid.Unmark_cell(row, column);

                // Remove the visual representation of the point.
                var elementToRemove = _grid.Children
                    .OfType<Ellipse>()
                    .FirstOrDefault(e => (string)e.Tag == $"{column},{row}");

                if (elementToRemove != null)
                {
                    _grid.Children.Remove(elementToRemove);
                }


                if (elementToRemove != null)
                {
                    _grid.Children.Remove(elementToRemove);
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while removing the point: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }




    }
}
