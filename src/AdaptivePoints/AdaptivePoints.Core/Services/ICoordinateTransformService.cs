using AdaptivePoints.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptivePoints.Core.Services
{
    public interface ICoordinateTransformService
    {
        CoordinatePoint ToShared(CoordinatePoint internalPoint);
        CoordinatePoint ToInternal(CoordinatePoint sharedPoint);
    }
}
