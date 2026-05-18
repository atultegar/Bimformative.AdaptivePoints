using AdaptivePoints.Core.Models;
using AdaptivePoints.Core.Services;
using Autodesk.DesignScript.Geometry;
using Autodesk.Revit.DB;
using BIMformative.AdaptivePoints.Extensions;
using BIMformative.AdaptivePoints.Utils;
using System;

namespace AdaptivePoints.Services
{
    class CoordinateTransformService : ICoordinateTransformService
    {
        private readonly CoordinateSystem _toSharedTransform;

        private readonly CoordinateSystem _toInternalTransform;

        public CoordinateTransformService(Document doc)
        {
            if (doc == null) 
                throw new ArgumentNullException(nameof(doc));

            _toInternalTransform = TransformUtils.DocumentTotalTransform();

            _toSharedTransform = TransformUtils.DocumentTotalTransform().Inverse();
        }

        
        public CoordinatePoint ToInternal(CoordinatePoint sharedPoint)
        {

            var dsPoint = sharedPoint.ToProtoPoint();

            var internalPoint = dsPoint.Transform(_toInternalTransform) as Autodesk.DesignScript.Geometry.Point;

            var output = internalPoint.ToCoordinatePoint();

            dsPoint.Dispose();
            internalPoint.Dispose();

            return output;
        }

        
        public CoordinatePoint ToShared(CoordinatePoint internalPoint)
        {
            var dsPoint = internalPoint.ToProtoPoint();

            var sharedPoint = dsPoint.Transform(_toSharedTransform) as Autodesk.DesignScript.Geometry.Point;

            var output = sharedPoint.ToCoordinatePoint();

            dsPoint.Dispose();
            sharedPoint.Dispose();

            return output;
        }
    }
}
