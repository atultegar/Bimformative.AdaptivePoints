using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptivePoints.Core.Models
{
    public class AdaptivePointData
    {
        /// <summary>
        /// Point order/index in adaptive component
        /// </summary>
        public int Index { get; set; }
        
        /// <summary>
        /// Reference point name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public CoordinateSystemType CoordinateSystemType { get; set; }

        public AdaptivePointData Clone()
        {
            return (AdaptivePointData)MemberwiseClone();
        }
    }
}
