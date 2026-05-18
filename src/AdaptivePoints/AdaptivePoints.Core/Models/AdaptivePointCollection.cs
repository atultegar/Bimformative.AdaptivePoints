using System.Collections.Generic;

namespace AdaptivePoints.Core.Models
{
    public class AdaptivePointCollection
    {
        public List<AdaptivePointEntity> Points { get; set; } = new List<AdaptivePointEntity>();

        public int Count => Points.Count;
    }
}
