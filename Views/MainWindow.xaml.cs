using System.Windows;
using Exam12_Pu5L.Controllers;
using System.Windows.Controls;
using Exam12_Pu5L.Models;
using Exam12_Pu5L.Views;

namespace Exam12_Pu5L
{
    public partial class MainWindow : Window
    {
        private Player[] players;
        private readonly string[] playerColors = { "#5FDEC8", "#FF5B55" };
        private PlayerController plc;
        private int limit;

        public MainWindow()
        {
            InitializeComponent();
            plc = new PlayerController();
            PlayerSection.Width = Parent.Width / 2;
            RowColumnSection.Width = Parent.Width / 2;
        }

        private void StartGameButton_Click(object sender, RoutedEventArgs e)
        {
            if (Check_names() != 0) return;
            bool isContinuous = Continuous.IsChecked ?? false;

            // Verify number of Row and Column
            if (int.TryParse(RowInput.Text, out int rows) && int.TryParse(ColumnInput.Text, out int columns) && int.TryParse(LimitInput.Text, out int limit))
            {
                if (limit <= 5) { 
                    MessageBox.Show("Limit must be greater than 5.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (rows > 5 && columns > 5)
                {
                    InitializeGrid(rows, columns, isContinuous, 0);
                }
                else
                {
                    MessageBox.Show("Rows and Columns must be greater than 5.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

            }
            else
            {
                MessageBox.Show("Please enter valid numbers for Rows and Columns and limit.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // In MainWindow.xaml.cs
        public void ShowMainMenu()
        {
            Parent.Visibility = Visibility.Visible;
            GameGridView.Visibility = Visibility.Collapsed;
        }

        private void HideMainMenu()
        {
            Parent.Visibility = Visibility.Collapsed;
            GameGridView.Visibility = Visibility.Visible;
        }

        private void InitializeGrid(int _rows, int _columns, bool _continuousBool, int _turn)
        {
            HideMainMenu();
            plc.ClearPreviousGrid(GameGridView.GameGrid);
            GameGridView.UpdateLayout();
            GameGridView.SetMainWindow(this);
            GameGridView.CreateGrid(_rows, _columns, players, _continuousBool, _turn);
        }

        private int Check_names()
        {
            // Verify player names
            string player1Name = Player1NameInput.Text.Trim();
            string player2Name = Player2NameInput.Text.Trim();
            int limitMove = Convert.ToInt32(LimitInput.Text.Trim());

            if (string.IsNullOrEmpty(player1Name))
            {
                MessageBox.Show("Player 1 must have a name.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return -1;
            }

            if (string.IsNullOrEmpty(player2Name))
            {
                MessageBox.Show("Player 2 must have a name.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return -1;
            }

            if (player1Name.Equals(player2Name, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Players must have different names.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return -1;
            }

            players =
            [
                new Player(player1Name, playerColors[0], 1, 0, limitMove),
                new Player(player2Name, playerColors[1], 2, 0, limitMove)
            ];
            return 0;
        }

        private void LoadGame_Click(object sender, RoutedEventArgs e)
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

                    plc.ClearPreviousGrid(GameGridView.GameGrid);
                    InitializeGrid(gridDimensions.Rows, gridDimensions.Columns, continuousBool, turn);
                    plc.RepaintPointsOnGrid(gridDimensions.Columns, gridDimensions.Rows, players, GameGridView.GameGrid, GameGridView.gridController);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading game: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

}