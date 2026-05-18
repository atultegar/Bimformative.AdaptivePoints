using AdaptivePoints.Services;
using Autodesk.Revit.DB;
using BIMformative.AdaptivePoints.Extensions;
using BIMformative.AdaptivePoints.Services;
using BIMformative.AdaptivePoints.UI.Views;
using RevitServices.Persistence;
using Point = Autodesk.DesignScript.Geometry.Point;
using Dynamo.Graph.Nodes;
using System.Linq;
using System;
using System.Collections.Generic;
using DynamoServices;

namespace AdaptivePoints
{
    /// <summary>
    /// Dynamo nodes for reading and editing
    /// Adaptive Component placement points.
    /// </summary>
    public static class AdaptivePoint
    {
        [NodeCategory("Query")]
        public static IList<Point> GetPoints(Revit.Elements.AdaptiveComponent adaptiveComponent)
        {
            ValidateInput(adaptiveComponent);

            var doc = DocumentManager.Instance.CurrentDBDocument;

            var service = new AdaptivePointService(doc);

            return service
                .GetAdaptivePoints(new ElementId(adaptiveComponent.Id))
                .Select(p => p.InternalPoint.ToProtoPoint())
                .ToList();
        }

        /// <summary>
        /// Updates the placement points of the specified adaptive component with a new set of points.
        /// </summary>
        /// <remarks>The adaptive component must already exist in the Revit document. Updating points may
        /// affect the geometry or placement of the component within the model.</remarks>
        /// <param name="adaptiveComponent">The adaptive component whose points are to be updated. Cannot be null.</param>
        /// <param name="points">A list of points representing the new locations for the adaptive component's placement points. The number
        /// and order of points must match the requirements of the adaptive component.</param>
        /// <returns>The adaptive component with its points updated to the specified locations.</returns>
        [NodeCategory("Action")]
        public static Revit.Elements.AdaptiveComponent UpdatePoints(Revit.Elements.AdaptiveComponent adaptiveComponent, IList<Point> points)
        {
            ValidateInput(adaptiveComponent);

            var doc = DocumentManager.Instance.CurrentDBDocument;

            var service = new AdaptivePointService(doc);

            var elementId = new ElementId(adaptiveComponent.Id);

            service.UpdatePoints(elementId, points);

            return adaptiveComponent;
        }

        /// <summary>
        /// Opens adaptive point editor UI.
        /// </summary>
        /// <param name="adaptiveComponent"></param>
        /// <returns>List of points</returns>
        [STAThread]
        [NodeCategory("UI")]
        public static IList<Point> EditPointsUI(Revit.Elements.AdaptiveComponent adaptiveComponent)
        {
            ValidateInput(adaptiveComponent);

            var doc = DocumentManager.Instance.CurrentDBDocument;

            var adaptivePointservice = new AdaptivePointService(doc);

            var transformService = new CoordinateTransformService(doc);

            var elementId = new ElementId(adaptiveComponent.Id);

            var points = adaptivePointservice.GetAdaptivePoints(elementId);
                        
            var window = new AdaptivePointWindow(
                elementId,
                points,
                adaptivePointservice,
                transformService);

            var result = window.ShowDialog();

            if (result != true)
            {
                return points.Select(x => x.InternalPoint.ToProtoPoint()).ToList();
            }

            return window.Result
                .Select(x => x.InternalPoint.ToProtoPoint())
                .ToList();
        }

        #region Helpers

        private static void ValidateInput(
            Revit.Elements.AdaptiveComponent adaptiveComponent)
        {
            if (adaptiveComponent.Id <= 0)
            {
                LogWarningMessageEvents.OnLogWarningMessage("Invalid Adaptive Component.");
            }
        }

        #endregion
    }
}
