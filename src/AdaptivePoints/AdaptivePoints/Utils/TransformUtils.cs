using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using Revit.GeometryConversion;
using RevitServices.Persistence;
using System.Collections.Generic;
using System.Linq;

namespace BIMformative.AdaptivePoints.Utils
{
    /// <summary>
    /// Provides utility methods for converting coordinates between a document's internal coordinate system and the
    /// shared coordinate system.
    /// </summary>
    /// <remarks>This class offers static methods to transform points and collections of points between
    /// internal and shared coordinate systems for a given document. These utilities are useful when working with models
    /// that require consistent spatial references, such as when linking or federating multiple documents. All methods
    /// require a valid document instance and do not modify the input data.</remarks>
    public static class TransformUtils
    {
        /// <summary>
        /// Gets the transform that converts coordinates from the internal coordinate system to the shared coordinate
        /// system for the specified document.
        /// </summary>
        /// <param name="doc">The document for which to retrieve the internal-to-shared coordinate transform. Cannot be null.</param>
        /// <returns>A Transform representing the conversion from the internal coordinate system to the shared coordinate system
        /// of the document.</returns>
        [IsVisibleInDynamoLibrary(false)]
        public static Transform GetInternalTransform(Document doc)
        {
            return doc.ActiveProjectLocation
                .GetTotalTransform()
                .Inverse;
        }

        /// <summary>
        /// Gets the transformation that maps coordinates from the shared coordinate system to the internal coordinate
        /// system for the specified document.
        /// </summary>
        /// <param name="doc">The document for which to retrieve the shared-to-internal transformation. Cannot be null.</param>
        /// <returns>A Transform object representing the transformation from the shared coordinate system to the internal
        /// coordinate system of the document.</returns>
        [IsVisibleInDynamoLibrary(false)]
        public static Transform GetSharedTransform(Document doc)
        {
            return doc.ActiveProjectLocation
                .GetTotalTransform();
        }

        /// <summary>
        /// Converts a collection of model-space points to their corresponding coordinates in the shared coordinate
        /// system of the specified document.
        /// </summary>
        /// <remarks>The transformation uses the shared coordinate system defined in the specified
        /// document. The order of the returned points matches the input sequence.</remarks>
        /// <param name="doc">The document whose shared coordinate system is used for the transformation.</param>
        /// <param name="points">The collection of points, in model coordinates, to convert to shared coordinates. Cannot be null.</param>
        /// <returns>A list of points representing the input coordinates transformed to the shared coordinate system. The list
        /// will be empty if no points are provided.</returns>
        [IsVisibleInDynamoLibrary(false)]
        public static List<XYZ> ToShared(Document doc, IEnumerable<XYZ> points)
        {
            var transform = GetSharedTransform(doc);

            return points
                .Select(transform.OfPoint)
                .ToList();
        }

        /// <summary>
        /// Converts a collection of points from the shared coordinate system to the internal coordinate system of the
        /// specified document.
        /// </summary>
        /// <remarks>The returned list contains the transformed points in the same order as the input
        /// collection. This method is typically used when working with coordinates that need to be aligned with the
        /// document's internal system.</remarks>
        /// <param name="doc">The document whose internal coordinate system is used for the transformation. Cannot be null.</param>
        /// <param name="points">The collection of points in the shared coordinate system to convert. Cannot be null.</param>
        /// <returns>A list of points transformed to the internal coordinate system of the document.</returns>
        [IsVisibleInDynamoLibrary(false)]
        public static List<XYZ> ToInternal(Document doc, IEnumerable<XYZ> points)
        {
            var transform = GetInternalTransform(doc);

            return points
                .Select(transform.OfPoint)
                .ToList();
        }

        /// <summary>
        /// Converts the specified point from the internal coordinate system of the given document to the shared
        /// coordinate system.
        /// </summary>
        /// <param name="doc">The document whose internal-to-shared coordinate transformation will be used.</param>
        /// <param name="point">The point, in the document's internal coordinate system, to convert.</param>
        /// <returns>A point in the shared coordinate system corresponding to the specified internal point.</returns>
        [IsVisibleInDynamoLibrary(false)]
        public static XYZ ToShared(Document doc, XYZ point)
        {
            return GetSharedTransform(doc)
                .OfPoint(point);
        }

        /// <summary>
        /// Converts a point from shared coordinates to the internal coordinate system of the specified document.
        /// </summary>
        /// <param name="doc">The document whose internal coordinate system is used for the conversion. Cannot be null.</param>
        /// <param name="point">The point in shared coordinates to convert.</param>
        /// <returns>An <see cref="XYZ"/> representing the point in the document's internal coordinate system.</returns>
        [IsVisibleInDynamoLibrary(false)]
        public static XYZ ToInternal(Document doc, XYZ point)
        {
            return GetInternalTransform(doc).OfPoint(point);
        }

        [IsVisibleInDynamoLibrary(false)]
        public static CoordinateSystem DocumentTotalTransform()
        {
            CoordinateSystem cs = null;

            Document doc = DocumentManager.Instance.CurrentDBDocument;

            ProjectLocation location = doc.ActiveProjectLocation;

            if (location != null)
            {
                Transform transform = location.GetTotalTransform();
                cs = CoordinateSystem.ByOriginVectors(
                    transform.Origin.ToPoint(),
                    transform.BasisX.ToVector(),
                    transform.BasisY.ToVector(),
                    transform.BasisZ.ToVector());
            }
            else
            {
                cs = CoordinateSystem.ByOrigin(0.0, 0.0, 0.0);
            }

            return cs;
        }
    }
}
