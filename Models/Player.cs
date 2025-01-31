using System.Diagnostics;
using System.Windows.Media;

namespace Exam12_Pu5L.Models
{
    public class Player
    {
        public string name { get; private set; }
        public String color { get; private set; }
        public int score { get; private set; }
        public int id { get; private set; }
        public HashSet<(int X, int Y)> points { get; private set; }

        public int limit { get; private set; }
        public int originalLimit { get; private set; }
        public List<(int X, int Y)> pPoint { get; private set; }


        public Player() { }

        public Player(string _name, String _playerColor, int _id, int _score, int _limit)
        {
            name = _name;
            color = _playerColor;
            score = _score;
            id = _id;
            points = new HashSet<(int X, int Y)>();
            limit = _limit;
            originalLimit = _limit;
        }

        //-------LOGIC POINT
        public void Add_point(int x, int y)
        {
            //Debug.WriteLine($"X={x}; Y={y}");
            points.Add((x, y));
            ReduceLimit();
        }

        public void Remove_point(int x, int y)
        {
            points.Remove((x, y));
        }

        public bool Has_point(int x, int y)
        {
            return points.Contains((x, y));
        }

        public void Reset_points()
        {
            points.Clear();
        }

        public void Add_score() { 
            score++; 
        }

        //------LOADING SAVE
        public Player(string name, string color, int score, int id, HashSet<(int X, int Y)> points, int _limit, int OGlim)
        {
            this.name = name;
            this.color = color;
            this.score = score;
            this.id = id;
            this.points = points;
            limit = _limit;
            originalLimit = OGlim;
        }

        //-------ALEA
        public void ReduceLimit()
        {
            if(limit <= 0) {
                limit = 0;
            } else
            {
                limit--;
            }
        }

        public void ResetLimit()
        {
            limit = originalLimit;
        }


    }
}
