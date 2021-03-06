namespace PickaChoose.Data
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

        public Point()
        {
            X = -1;
            Y = -1;
        }

        public bool HasValue() => X != -1 && Y != -1;

        public bool IsEqual(int x, int y) => X == x && Y == y;

        public bool IsEqual(Point point) => X == point.X && Y == point.Y;
    }
}
