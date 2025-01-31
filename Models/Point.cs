

namespace Exam12_Pu5L.Models
{
    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        // Needed for deserialization
        public Point() { }
    }

}
