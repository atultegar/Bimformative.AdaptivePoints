using AdaptivePoints.Core.Models;
using Autodesk.Revit.DB;
using Revit.GeometryConversion;
using RevitServices.Persistence;
using Point = Autodesk.DesignScript.Geometry.Point;

namespace BIMformative.AdaptivePoints.Extensions
{    
    internal static class GeometryExtensions
    {        
        public static XYZ ToXyz(this Point point)
        {
            return point.ToRevitType(true);
        }

        internal static XYZ ToXYZ(this CoordinatePoint point)
        {
            return new XYZ(point.X, point.Y, point.Z);
        }

        
        internal static Point ToProtoPoint(this XYZ xyz)
        {
            return xyz.ToPoint();
        }

        
        internal static Point ToProtoPoint(this AdaptivePointData point)
        {
            return Point.ByCoordinates(point.X, point.Y, point.Z);
        }

        internal static Point ToProtoPoint(this CoordinatePoint point)
        {
            return Point.ByCoordinates(
                point.X, 
                point.Y, 
                point.Z);
        }

        internal static CoordinatePoint ToCoordinatePoint(this XYZ xyz)
        {
            return new CoordinatePoint(
                xyz.X,
                xyz.Y,
                xyz.Z);
        }

        internal static CoordinatePoint ToCoordinatePoint(this Point point)
        {
            return new CoordinatePoint(
                point.X,
                point.Y,
                point.Z);
        }
    }
}
