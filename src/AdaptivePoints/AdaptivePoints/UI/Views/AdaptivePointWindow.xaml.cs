using AdaptivePoints.Core.Models;
using AdaptivePoints.Services;
using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using BIMformative.AdaptivePoints.Services;
using Dynamo.Wpf.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BIMformative.AdaptivePoints.UI.Views
{
    /// <summary>
    /// Interaction logic for AdaptivePointWindow.xaml
    /// </summary>
    [SupressImportIntoVM]
    public partial class AdaptivePointWindow : Window
    {
        #region Fields

        private readonly ElementId _elementId;

        private readonly AdaptivePointService _adaptivePointservice;

        private readonly CoordinateTransformService _transformService;

        private readonly List<AdaptivePointEntity> _originalState;

        private List<AdaptivePointEntity> _points;

        #endregion

        #region Properties

        [IsVisibleInDynamoLibrary(false)]
        public List<AdaptivePointEntity> Result => _points;

        private AdaptivePointEntity CurrentPoint => _points[lstRP.SelectedIndex];

        #endregion

        #region Constructor
        internal AdaptivePointWindow(
            ElementId elementId,
            List<AdaptivePointEntity> points,
            AdaptivePointService adaptivePointService,
            CoordinateTransformService transformService)
        {            
            InitializeComponent();

            _elementId = elementId;

            _adaptivePointservice = adaptivePointService;

            _transformService = transformService;

            _points = points.Select(p => p.Clone()).ToList();

            _originalState = points.Select(p => p.Clone()).ToList();

            lstRP.ItemsSource = _points;

            lstRP.DisplayMemberPath = "Name";

            lstRP.SelectedIndex = 0;
        }

        #endregion

        #region Selection

        private void lstRP_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadPointToUI();

            DisableEditors();
        }

        #endregion

        #region Internal Coordinates

        private void btnRvtEdit_Click(object sender, RoutedEventArgs e)
        {
            rvtX.IsEnabled = true;
            rvtY.IsEnabled = true;
            rvtZ.IsEnabled = true;

            btnRvtCancel.IsEnabled = true;
            btnRvtApply.IsEnabled = true;
        }

        private void btnRvtCancel_Click(object sender, RoutedEventArgs e)
        {
            LoadPointToUI();

            DisableEditors();
        }

        private void btnRvtApply_Click(object sender, RoutedEventArgs e)
        {
            if (!TryGetInternalPoint(out CoordinatePoint internalPoint))
                return;

            var sharedPoint = _transformService.ToShared(internalPoint);

            CurrentPoint.InternalPoint = internalPoint;

            CurrentPoint.SharedPoint = sharedPoint;

            LoadPointToUI();

            DisableEditors();
        }

        #endregion

        #region Shared Coordinates

        private void btnSharedEdit_Click(object sender, RoutedEventArgs e)
        {
            sharedX.IsEnabled = true;
            sharedY.IsEnabled = true;
            sharedZ.IsEnabled = true;

            btnSharedCancel.IsEnabled = true;
            btnSharedApply.IsEnabled = true;
        }

        private void btnSharedCancel_Click(object sender, RoutedEventArgs e)
        {
            LoadPointToUI();

            DisableEditors();
        }

        private void btnSharedApply_Click(object sender, RoutedEventArgs e)
        {
            if (!TryGetSharedPoint(out CoordinatePoint sharedPoint))
                return;

            var internalPoint = _transformService.ToInternal(sharedPoint);

            CurrentPoint.SharedPoint = sharedPoint;
            CurrentPoint.InternalPoint = internalPoint;

            LoadPointToUI();

            DisableEditors();
        }

        #endregion

        #region Main Buttons

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _adaptivePointservice.UpdateAdaptivePoints(_elementId, _points);

                MessageBoxService.Show(
                    "Adaptive points updated successfully.",
                    "AdaptivePoints",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBoxService.Show(
                    ex.Message,
                    "AdaptivePoints",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _adaptivePointservice.UpdateAdaptivePoints(_elementId, _points);

                DialogResult = true;

                Close();
            }
            catch (Exception ex)
            {
                MessageBoxService.Show(
                    ex.Message,
                    "AdaptivePoints",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            _points = _originalState.Select(x => x.Clone()).ToList();

            lstRP.ItemsSource = null;

            lstRP.ItemsSource = _points;

            lstRP.SelectedIndex = 0;

            LoadPointToUI();

            DisableEditors();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;

            Close();
        }

        #endregion


        #region Helpers

        private void LoadPointToUI()
        {
            var p = CurrentPoint;

            rvtX.Text = p.InternalPoint.X.ToString("F3");
            rvtY.Text = p.InternalPoint.Y.ToString("F3");
            rvtZ.Text = p.InternalPoint.Z.ToString("F3");

            sharedX.Text = p.SharedPoint.X.ToString("F3");
            sharedY.Text = p.SharedPoint.Y.ToString("F3");
            sharedZ.Text = p.SharedPoint.Z.ToString("F3");
        }

        private void DisableEditors()
        {
            rvtX.IsEnabled = false; 
            rvtY.IsEnabled = false; 
            rvtZ.IsEnabled = false;

            sharedX.IsEnabled = false; 
            sharedY.IsEnabled = false; 
            sharedZ.IsEnabled = false;

            btnRvtApply.IsEnabled = false;
            btnRvtCancel.IsEnabled = false;

            btnSharedApply.IsEnabled = false;
            btnSharedCancel.IsEnabled = false;
        }

        private bool TryGetInternalPoint(out CoordinatePoint point)
        {
            point = null;

            if (!TryParse(rvtX.Text, out var x) ||
                !TryParse(rvtY.Text, out var y) ||
                !TryParse(rvtZ.Text, out var z))
            {
                MessageBoxService.Show(
                    "Invalid internal coordinate values.", 
                    "AdaptivePoints", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Warning);

                return false;
            }

            point = new CoordinatePoint(x, y, z);

            return true;
        }

        private bool TryGetSharedPoint(out CoordinatePoint point)
        {
            point = null;

            if (!TryParse(sharedX.Text, out var x) ||
                !TryParse(sharedY.Text, out var y) ||
                !TryParse(sharedZ.Text, out var z))
            {
                MessageBoxService.Show(
                    "Invalid shared coordinate values.",
                    "AdaptivePoints",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }

            point = new CoordinatePoint(x, y, z);

            return true;
        }

        private bool TryParse(string text, out double value)
        {
            text = text.Replace(",", ".");

            return double.TryParse(
                text,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out value);
        }

        #endregion
    }
}
