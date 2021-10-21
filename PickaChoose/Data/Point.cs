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

        public static Point operator + (Point a, Point b)
        {
            return new Point(a.X + b.X, a.Y + b.Y);
        }

        public bool HasValue()
        {
            return X != -1 && Y != -1;
        }
    }
}
