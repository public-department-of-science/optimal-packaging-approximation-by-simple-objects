namespace TestProblemIpOpt.Model
{
    public class Point
    {
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }

        public Point(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Point()
        {
            X = 0;
            Y = 0;
            Z = 0;
        }
    }
}
