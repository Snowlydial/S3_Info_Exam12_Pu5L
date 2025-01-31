using Exam12_Pu5L.Controllers;
using Exam12_Pu5L.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Exam12_Pu5L.Views
{
    public partial class GridView : UserControl
    {
        public int nbRow { get; private set; }
        public int nbCol { get; private set; }
        public bool continuous { get; private set; }

        
        private MainWindow mainWindow;
        public GridController gridController { get; private set; }
        private PointController pointController;
        private PlayerController playerController;
        private Player[] players;
        private Player currentPlayer;
        public int currentPlayerIndex { get; private set; }
        
        //Deactivate alea: No more limit
        private bool limitActivated = false;


        public GridView()
        {
            InitializeComponent();
            pointController = new PointController();
            PlayerInteract.Background = new SolidColorBrush(Colors.White);
        }


        //-----PLAYER LOGIC
        public void UpdatePlayerTurnDisplay()
        {
            PlayerTurn.Text = currentPlayer.name;
            PlayerTurn.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(currentPlayer.color));
        }

        private void InitializePlayers(Player[] _players)
        {
            players = _players;
            currentPlayer = players[currentPlayerIndex];
            UpdatePlayerTurnDisplay();
        }

        private void NextTurn()
        {
            currentPlayerIndex = GetNextPlayer();
            currentPlayer = players[currentPlayerIndex];
            UpdatePlayerTurnDisplay();
        }

        private int GetNextPlayer()
        {
            return (currentPlayerIndex + 1) % players.Length; // Cycle through players
        }

        private void DisplayScores()
        {
            // For Player 1
            P1_Score.Inlines.Clear();
            P1_Score.Inlines.Add(new Run(players[0].name) { Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(players[0].color))});
            P1_Score.Inlines.Add(new Run(" score: "));
            P1_Score.Inlines.Add(new Run(players[0].score.ToString()));

            // For Player 2
            P2_Score.Inlines.Clear();
            P2_Score.Inlines.Add(new Run(players[1].name) { Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(players[1].color))});
            P2_Score.Inlines.Add(new Run(" score: "));
            P2_Score.Inlines.Add(new Run(players[1].score.ToString()));
        }

        // ALEA
        private void DisplayLimit()
        {
            // For Player 1
            P1_Limit.Inlines.Clear();
            P1_Limit.Inlines.Add(new Run(players[0].name) { Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(players[0].color)) });
            P1_Limit.Inlines.Add(new Run(" limit: "));
            // For Player 2
            P2_Limit.Inlines.Clear();
            P2_Limit.Inlines.Add(new Run(players[1].name) { Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(players[1].color)) });
            P2_Limit.Inlines.Add(new Run(" limit: "));

            if(!limitActivated)
            {
                P1_Limit.Inlines.Add(new Run("Infinite"));
                P2_Limit.Inlines.Add(new Run("Infinite"));
            } else
            {
                P1_Limit.Inlines.Add(new Run(players[0].limit.ToString()));
                P2_Limit.Inlines.Add(new Run(players[1].limit.ToString()));
            }
        }

        //-----THE GRID
        public void CreateGrid(int _rows, int _columns, Player[] _players, bool _continuous, int _turn)
        {
            continuous = _continuous;
            nbRow = _rows;
            nbCol = _columns;
            InitializePlayers(_players);
            DisplayScores();
            DisplayLimit();
            currentPlayerIndex = _turn;

            gridController = new GridController(_rows, _columns);
            playerController = new PlayerController(players, this);
            
            GridBackground.Children.Clear();
            GameGrid.RowDefinitions.Clear();
            GameGrid.ColumnDefinitions.Clear();
            GameGrid.Children.Clear();

            // Set fixed size for GridBackground Canvas
            GridBackground.Width = 1000;  // Use a large enough fixed size
            GridBackground.Height = 1000;

            for (int i = 0; i < _rows; i++)
            {
                GameGrid.RowDefinitions.Add(new RowDefinition());
            }

            for (int j = 0; j < _columns; j++)
            {
                GameGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            double cellSize = GridBackground.Width / _columns;
            DrawBackgroundGrid(_rows, _columns, cellSize);

            for (int i = 0; i < _rows; i++)
            {
                for (int j = 0; j < _columns; j++)
                {
                    Button cellButton = new Button
                    {
                        Tag = (i, j),
                        Content = "",
                        Background = Brushes.Transparent,
                        BorderBrush = Brushes.Transparent
                    };
                    cellButton.Click += CellButton_Click;

                    Grid.SetRow(cellButton, i);
                    Grid.SetColumn(cellButton, j);
                    GameGrid.Children.Add(cellButton);
                }
            }
        }

        private void DrawBackgroundGrid(int rows, int columns, double spacing)
        {
            double totalWidth = spacing * columns;
            double totalHeight = spacing * rows;

            // Draw horizontal lines
            for (int i = 0; i <= rows; i++)
            {
                double y = i * spacing;
                Line horizontalLine = new Line
                {
                    X1 = 0,
                    Y1 = y,
                    X2 = totalWidth,
                    Y2 = y,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };
                GridBackground.Children.Add(horizontalLine);
            }

            // Draw vertical lines
            for (int j = 0; j <= columns; j++)
            {
                double x = j * spacing;
                Line verticalLine = new Line
                {
                    X1 = x,
                    Y1 = 0,
                    X2 = x,
                    Y2 = totalHeight,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };
                GridBackground.Children.Add(verticalLine);
            }
        }

        private void CellButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is (int column, int row))
            {
                PlaceThePointAtPosition(column, row, currentPlayer, GameGrid);
            }
        }

        private void PlaceThePointAtPosition(int _x, int _y, Player _whichPlayer, Grid _gameGrid)
        {
            bool isPlaced = gridController.Place_point(_x, _y, _whichPlayer, _gameGrid, limitActivated);
            DisplayLimit();
            CheckWin(isPlaced, _x, _y, _whichPlayer);
        }

        private void Restart_Click(object? sender, RoutedEventArgs? e)
        {
            ResetGame();
            //Suggest.Content = "Get a suggestion";
            //Suggest.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DDDDDD"));
            foreach (var player in players)
            {
                player.Reset_points();   
            }
        }

        private void ResetGame()
        {
            playerController.ClearPreviousGrid(GameGrid);
            currentPlayerIndex = 0;
            UpdateLayout();
            foreach(var player in players)
            {
                player.ResetLimit();
            }
            CreateGrid(nbRow, nbCol, players, continuous, currentPlayerIndex);
            DisplayScores();
            DisplayLimit();
        }


        //-----SUGGEST
        private void Suggest_Click(object sender, RoutedEventArgs e)
        {
            var suggestion = pointController.GetSuggestion(currentPlayer, players[GetNextPlayer()], nbRow, nbCol);

            if (suggestion.HasValue)
            {
                PlaceThePointAtPosition(suggestion.Value.X, suggestion.Value.Y, currentPlayer, GameGrid);
            }
        }

        public void UpdateSuggestionContent()
        {
            if (pointController.FindCorrectMove(currentPlayer, players[GetNextPlayer()], nbRow, nbCol, 4).HasValue )
            {
                Suggest.Content = "CHECKMATE";
                Suggest.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e8d574"));
            }
            else if (pointController.FindCorrectMove(players[GetNextPlayer()], currentPlayer, nbRow, nbCol, 4).HasValue)
            {
                Suggest.Content = "Get defensive move";
                Suggest.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7cadd9"));
            }
            else if (pointController.FindCorrectMove(currentPlayer, players[GetNextPlayer()], nbRow, nbCol, 3).HasValue)
            {
                Suggest.Content = "Get strategic move";
                Suggest.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#bcdb97"));
            }
            else
            {
                Suggest.Content = "Get a suggestion";
                Suggest.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DDDDDD"));
            }
        }

        private void CheckWin(bool _isPlaced, int _x, int _y, Player _player)
        {
            if (_isPlaced)
            {
                //---------------------------------------OLD
                //bool is5Aligned = pointController.CheckForAlignment(_x, _y, _player, nbRow, nbCol, 5);

                //---------------------------------------NEW: exa12
                bool is5Aligned = pointController.CheckForAlignmentL(_x, _y, _player, nbRow, nbCol, 5);

                if (is5Aligned)
                {
                    //---------CONTINUOUS (Can continue to play on the same grid no reset)
                    if(continuous)
                    {
                        HashSet<(int X, int Y)> winningPoints = pointController.alignedPoints;
                        _player.Add_score();
                        DisplayScores();
                        DisplayLimit();

                        if (!pointController.previousAlignments.Contains(winningPoints))
                        {
                            pointController.previousAlignments.Add(winningPoints);
                        }

                        gridController.Highlight_aligned_continuous(pointController.previousAlignments, GameGrid);

                        var notification = new Popup
                        {
                            Child = new Border
                            {
                                Child = new TextBlock
                                {
                                    Text = $"{_player.name} scored! (+1 point)",
                                    Background = Brushes.LightYellow,
                                    Padding = new Thickness(10)
                                },
                                BorderBrush = Brushes.Gold,
                                BorderThickness = new Thickness(1)
                            },
                            IsOpen = true,
                            StaysOpen = false
                        };

                        // Auto-close the notification after 2 seconds
                        var timer = new System.Windows.Threading.DispatcherTimer
                        {
                            Interval = TimeSpan.FromSeconds(2)
                        };
                        timer.Tick += (s, e) => {
                            notification.IsOpen = false;
                            timer.Stop();
                        };
                        timer.Start();

                        
                    } else
                    {
                        //---------NO CONTINUOUS (Stop and pass to next round, grid no reset)
                        HashSet<(int X, int Y)> winningPoints = pointController.alignedPoints;
                        _player.Add_score();
                        gridController.Highlight_aligned(winningPoints, GameGrid);

                        MessageBoxResult result = MessageBox.Show(
                            $"'{_player.name}' was able to align 5 points first: {FormatAlignedPoints(pointController.alignedPoints)}; {_player.name} +1 score. Rematch?",
                            "Game Over",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Question
                        );

                        if (result == MessageBoxResult.Yes)
                        {
                            Restart_Click(null, null);
                        }
                        else
                        {
                            Restart_Click(null, null);
                            SaveButton_Click(null, null);
                            mainWindow.ShowMainMenu();
                        }
                        return;
                    }
                }
                NextTurn();
                UpdateSuggestionContent();
            }
            else
            {
                MessageBox.Show($"Cell ({_x}, {_y}) is already occupied!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private string FormatAlignedPoints(HashSet<(int X, int Y)> alignedPoints)
        {
            // Convert each point in the HashSet to a string representation
            var pointsList = alignedPoints.Select(point => $"({point.X}, {point.Y})").ToList();

            // Join the points into a single string separated by commas
            return string.Join(", ", pointsList);
        }

        //-----OPTION MENU
        private void Option_Click(object sender, RoutedEventArgs e)
        {
            OptionsPanel.Visibility = Visibility.Visible;
        }

        private void SaveButton_Click(object? sender, RoutedEventArgs? e)
        {
            playerController.SaveGame();
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            var availableSaves = PlayerController.GetAvailableSaves();

            if (availableSaves.Count == 0)
            {
                MessageBox.Show("No saved games found!", "Load Game", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var dialog = new LoadGameDialog(availableSaves);
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var (gridDimensions, loadedPlayers, continuousBool, turn) = PlayerController.LoadGame(dialog.SelectedSaveFile);

                    players = loadedPlayers;

                    currentPlayerIndex = turn;
                    nbRow = gridDimensions.Rows;
                    nbCol = gridDimensions.Columns;
                    continuous = continuousBool;
                    ResetGame();
                    playerController.RepaintPointsOnGrid(gridDimensions.Columns, gridDimensions.Rows, players, GameGrid, gridController);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading game: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            OptionsPanel.Visibility = Visibility.Collapsed;
        }

        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.ShowMainMenu();
        }

        public void SetMainWindow(MainWindow window)
        {
            mainWindow = window;
        }

    }
}