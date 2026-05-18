using AdaptivePoints.Core.Models;
using System;
using System.Collections.Generic;

namespace AdaptivePoints.Core.Services
{    
    public static class PointValidationService
    {
        public static void ValidateCount(int expected, int actual, string message = null)
        {
            if (expected != actual)
                throw new InvalidOperationException(
                    message ??
                    $"Point count mismatch. Expected {expected}, Actual: {actual}");
        }

        public static void ValidateNotNull(
            object value,
            string parameterName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        public static void ValidatePoint(AdaptivePointEntity point)
        {
            ValidateNotNull(point, nameof(point));

            ValidateCoordinate(point.InternalPoint.X, nameof(point.InternalPoint.X));
            ValidateCoordinate(point.InternalPoint.Y, nameof(point.InternalPoint.Y));
            ValidateCoordinate(point.InternalPoint.Z, nameof(point.InternalPoint.Z));

            ValidateCoordinate(point.SharedPoint.X, nameof(point.SharedPoint.X));
            ValidateCoordinate(point.SharedPoint.Y, nameof(point.SharedPoint.Y));
            ValidateCoordinate(point.SharedPoint.Z, nameof(point.SharedPoint.Z));
        }

        public static void ValidateCollection(IEnumerable<AdaptivePointEntity> points)
        {
            ValidateNotNull(points, nameof(points));

            foreach (var point in points)
            {
                ValidatePoint(point);
            }
        }

        private static void ValidateCoordinate(double value, string parameterName)
        {
            if (double.IsNaN(value))
            {
                throw new ArgumentException(
                   $"{parameterName} cannot be NaN.");
            }

            if (double.IsInfinity(value))
            {
                throw new ArgumentException(
                   $"{parameterName} cannot be Infinity.");
            }
        }
    }
}
