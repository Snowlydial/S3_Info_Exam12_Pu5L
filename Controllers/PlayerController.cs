using System.Text.Json;
using System.IO;
using Exam12_Pu5L.Models;
using System.Windows.Controls;

namespace Exam12_Pu5L.Controllers
{
    public class PlayerController
    {
        private static Player[] players;
        private static Views.GridView gameGrid;
        private readonly static string savesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Saves");

        public PlayerController() { }

        public PlayerController(Player[] _players, Views.GridView _gameGrid)
        {
            players = _players;
            gameGrid = _gameGrid;
        }

        //------FOR JSON MAPPING/BLUEPRINT KINDA
        private class PlayerData
        {
            public string Name { get; set; }
            public string Color { get; set; }
            public int Score { get; set; }
            public int Id { get; set; }
            public List<Models.Point> Points { get; set; }
            public int Limit { get; set; }
            public int OGLimit { get; set; }
        }

        private class GameState
        {
            public List<PlayerData> Players { get; set; }
            public int NumberOfRows { get; set; }
            public int NumberOfColumns { get; set; }
            public bool ContinuousBool {  get; set; }
            public int CurrentTurn { get; set; }
        }


        //------SAVE
        public string GenerateSaveFileName()
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string player1Name = players[0].name.Replace(" ", "_");
            string player2Name = players[1].name.Replace(" ", "_");
            return $"{date}_{player1Name}-{player2Name}.json";
        }

        public void SaveGame()
        {
            var gameState = new GameState
            {
                //Transform each player p into a PlayerData
                Players = players.Select(p => new PlayerData
                {
                    Name = p.name,
                    Color = p.color,
                    Score = p.score,
                    Id = p.id,
                    Points = p.points.Select(tuple => new Point(tuple.X, tuple.Y)).ToList(),
                    Limit = p.limit,
                    OGLimit = p.originalLimit
                }).ToList(),
                NumberOfRows = gameGrid.nbRow,
                NumberOfColumns = gameGrid.nbCol,
                ContinuousBool = gameGrid.continuous,
                CurrentTurn = gameGrid.currentPlayerIndex
            };

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string saveFileName = GenerateSaveFileName();
            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, savesDirectory, saveFileName);
            string jsonString = JsonSerializer.Serialize(gameState, options);
            File.WriteAllText(fullPath, jsonString);
        }

        //------LOAD
        public static List<string> GetAvailableSaves()
        {
            if (!Directory.Exists(savesDirectory))
            {
                Directory.CreateDirectory(savesDirectory);
            }

            return Directory.GetFiles(savesDirectory, "*.json")
                            .Select(Path.GetFileName)
                            .ToList();
        }

        public static ((int Rows, int Columns) GridDimensions, Player[] Players, bool continuousBool, int turn) LoadGame(string saveFileName)
        {
            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, savesDirectory, saveFileName);

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"Save file not found: {saveFileName}");
            }

            string jsonString = File.ReadAllText(fullPath);
            var gameState = JsonSerializer.Deserialize<GameState>(jsonString);
            int nbPlayer = gameState.Players.Count;
            Player[] players = new Player[nbPlayer];

            for (int i = 0; i < nbPlayer; i++)
            {
                var playerData = gameState.Players[i];
                players[i] = new Player(
                    playerData.Name,
                    playerData.Color,
                    playerData.Score,
                    playerData.Id,
                    new HashSet<(int X, int Y)>(playerData.Points.Select(p => (p.X, p.Y))),
                    playerData.Limit,
                    playerData.OGLimit
                );
            }

            return ((gameState.NumberOfRows, gameState.NumberOfColumns), players, gameState.ContinuousBool, gameState.CurrentTurn);
        }


        // CLEARANCE
        public void RepaintPointsOnGrid(int rows, int cols, Player[] _players, Grid _gameGrid, GridController gctrl)
        {
            // Redraw points for both players
            foreach (var player in _players)
            {
                foreach (var point in player.points)
                {
                    gctrl.Place_point(point.X, point.Y, player, _gameGrid, false);
                }
            }
        }

        public void ClearPreviousGrid(Grid _gameGrid) // Clear existing grid
        {
            foreach (var child in _gameGrid.Children)
            {
                if (child is Button button)
                {
                    button.Content = "";
                    button.Background = null;
                }
            }
        }
    }
}
