namespace AdaptivePoints.Core.Models
{
    public class AdaptivePointEntity
    {
        public int Index { get; set; }
        public string Name { get; set; } = string.Empty;

        public CoordinatePoint InternalPoint { get; set; } = new CoordinatePoint();

        public CoordinatePoint SharedPoint { get; set; } = new CoordinatePoint();

        public AdaptivePointEntity Clone()
        {
            return new AdaptivePointEntity
            {
                Index = Index,
                Name = Name,
                InternalPoint = InternalPoint.Clone(),
                SharedPoint = SharedPoint.Clone()
            };
        }
    }
}
