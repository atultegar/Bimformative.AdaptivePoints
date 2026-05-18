using AdaptivePoints.Core.Models;
using AdaptivePoints.Core.Services;
using Autodesk.Revit.DB;
using BIMformative.AdaptivePoints.Utils;
using Dynamo.Wpf.Utilities;
using Revit.GeometryConversion;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BIMformative.AdaptivePoints.Services
{
    /// <summary>
    /// Service for reading and updating
    /// Adaptive Component placement points.
    /// </summary>
    internal class AdaptivePointService
    {
        private readonly Document _doc;

        private readonly ForgeTypeId _lengthTypeId;

        private readonly int _accuracy;
                
        internal AdaptivePointService(Document doc)
        {
            PointValidationService.ValidateNotNull(doc, nameof(doc));

            _doc = doc;


            var units = doc.GetUnits();

            FormatOptions option = units.GetFormatOptions(SpecTypeId.Length);
            _accuracy =(int)(-Math.Log10(option.Accuracy));

            _lengthTypeId = option.GetUnitTypeId();
        }


        #region Public Methods

        internal List<AdaptivePointEntity> GetAdaptivePoints(ElementId elementId)
        {
            var referencePoints = GetReferencePoints(elementId);

            var sharedTransform = TransformUtils.GetInternalTransform(_doc);

            var entities = new List<AdaptivePointEntity>();

            for (int i = 0; i < referencePoints.Count; i++)
            {
                var referencePoint = referencePoints[i];

                var internalPoint = referencePoint.Position;
                
                var sharedPoint = sharedTransform.OfPoint(internalPoint);

                var name = i.ToString();

                if (!String.IsNullOrWhiteSpace(referencePoint.Name))
                {
                    name = referencePoint.Name;
                }

                entities.Add(
                    new AdaptivePointEntity
                    {
                        Index = i,
                        Name = name,

                        InternalPoint = new CoordinatePoint(
                            ConvertFromInternalUnits(internalPoint.X),
                            ConvertFromInternalUnits(internalPoint.Y),
                            ConvertFromInternalUnits(internalPoint.Z)),

                        SharedPoint = new CoordinatePoint(
                            ConvertFromInternalUnits(sharedPoint.X),
                            ConvertFromInternalUnits(sharedPoint.Y),
                            ConvertFromInternalUnits(sharedPoint.Z))
                    });
            }

            return entities;
        }

        internal void UpdateAdaptivePoints(ElementId elementId, IList<AdaptivePointEntity> points)
        {
            PointValidationService.ValidateNotNull(points, nameof(points));

            PointValidationService.ValidateCollection(points);

            var referencePoints = GetReferencePoints(elementId);

            PointValidationService.ValidateCount(referencePoints.Count, points.Count);

            RevitServices.Transactions.TransactionManager.Instance.EnsureInTransaction(_doc);

            for (int i = 0; i < referencePoints.Count; i++)
            {
                using (SubTransaction subTr = new SubTransaction(_doc))
                {
                    subTr.Start();

                    try
                    {
                        var entity = points[i];

                        referencePoints[i].SetPointElementReference(null);

                        var xyzPoint = new XYZ(
                            ConvertToInternalUnits(entity.InternalPoint.X),
                            ConvertToInternalUnits(entity.InternalPoint.Y),
                            ConvertToInternalUnits(entity.InternalPoint.Z));

                        referencePoints[i].Position = xyzPoint;

                        subTr.Commit();
                    }
                    catch (Exception ex)
                    {
                        MessageBoxService.Show(ex.Message, "AdaptivePoints", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                        continue;
                    }
                }
                
            }
        }

        internal void UpdatePoints(ElementId elementId, IList<Autodesk.DesignScript.Geometry.Point> points)
        {
            PointValidationService.ValidateNotNull(points, nameof(points));

            var referencePoints = GetReferencePoints(elementId);

            PointValidationService.ValidateCount(referencePoints.Count, points.Count);

            RevitServices.Transactions.TransactionManager.Instance.EnsureInTransaction(_doc);

            for (int i = 0; i < referencePoints.Count; i++)
            {
                using (SubTransaction subTr = new SubTransaction(_doc))
                {
                    subTr.Start();

                    try
                    {
                        var dsPoint = points[i];

                        referencePoints[i].SetPointElementReference(null);

                        referencePoints[i].Position = dsPoint.ToRevitType(true);

                        subTr.Commit();
                    }
                    catch (Exception ex)
                    {
                        MessageBoxService.Show(ex.Message, "AdaptivePoints", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                        continue;
                    }
                }
                    
            }                    
        }

        #endregion

        #region Private Methods
        private List<ReferencePoint> GetReferencePoints(ElementId elementId)
        {
            var familyInstance = _doc.GetElement(elementId) as FamilyInstance;

            if (familyInstance == null)
            {
                throw new InvalidOperationException("Element is not a FamilyInstance");
            }

            if (!AdaptiveComponentInstanceUtils.IsAdaptiveComponentInstance(familyInstance))
            {
                throw new InvalidOperationException("FamilyInstance is not an adaptive component");
            }

            var ids = AdaptiveComponentInstanceUtils.GetInstancePlacementPointElementRefIds(familyInstance);

            return ids
                .Select(id => _doc.GetElement(id) as ReferencePoint)
                .Where(r => r != null)
                .Cast<ReferencePoint>()
                .ToList();
        }


        private double ConvertFromInternalUnits(double value)
        {
            return  Math.Round(UnitUtils.ConvertFromInternalUnits(value, _lengthTypeId), _accuracy);
        }

        private double ConvertToInternalUnits(double value)
        {
            return UnitUtils.ConvertToInternalUnits(value, _lengthTypeId);
        }
        #endregion
    }
}
