namespace AdaptivePoints.Core.Models
{
    public class CoordinatePoint
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public CoordinatePoint()
        {
            
        }

        public CoordinatePoint(double x, double y, double z)
        {
            X = x;
            Y = y; 
            Z = z;
        }

        public CoordinatePoint Clone()
        {
            return new CoordinatePoint(X, Y, Z);
        }
    }
}
