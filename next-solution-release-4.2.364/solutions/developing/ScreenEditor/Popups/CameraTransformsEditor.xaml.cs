using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using _3DTools;
using ScreenSettings.Entities;

namespace ScreenManager.Popups
{
    /// <summary>
    /// Interaction logic for CameraTransformsEditor.xaml
    /// </summary>
    public partial class CameraTransformsEditor : UserControl
    {
        readonly Trackball trackBall;
        readonly ScreenEntity entity;

        public CameraTransformsEditor(Trackball track, Object ent)
        {
            InitializeComponent();

            trackBall = track;
            entity = ent as ScreenEntity;

            gridControl.ItemsSource = entity.ListCameraTransforms;
        }

        void gridDataControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (gridControl.SelectedItem == null)
                return;
            var item = gridControl.SelectedItem as CameraTranforms;
            trackBall.Animate(item.Scale, item.Rotation, item.TranslateTransform2D, item.AnimationTime, 
                new SineEase() { EasingMode = EasingMode.EaseOut });
        }

        void UpdateGridControl(List<CameraTranforms> items)
        {
            gridControl.ItemsSource = null;
            gridControl.ItemsSource = items;
        }

        private void CanAddCurrentCamera(object sender, CanExecuteRoutedEventArgs e)
        {
            var text = name.EditValue as String;
            e.CanExecute = entity != null && !String.IsNullOrEmpty(text);
        }

        private void OnAddCurrentCamera(object sender, ExecutedRoutedEventArgs e)
        {
            var text = name.EditValue as String;
            if (entity.ListCameraTransforms == null)
                entity.ListCameraTransforms = new List<CameraTranforms>();
            entity.ListCameraTransforms.Add(new CameraTranforms()
            {
                Name = text,
                Scale = trackBall.ScaleTransform.Clone(),
                Rotation = trackBall.RotateTransform.Clone(),
                TranslateTransform2D = trackBall.TranslateTransform2D.Clone()
            });
            UpdateGridControl(entity.ListCameraTransforms);
        }

        private void CanRemoveCurrentCamera(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = entity != null && IsAnyItemSelected();
        }

        private void OnRemoveCurrentCamera(object sender, ExecutedRoutedEventArgs e)
        {
            DeleteCurrent();
        }

        void DeleteCurrent()
        {
            CameraTranforms selectedItem = gridControl.SelectedItem as CameraTranforms;
            if (entity.ListCameraTransforms == null)
                entity.ListCameraTransforms = new List<CameraTranforms>();
            if (entity.ListCameraTransforms.Contains(selectedItem))
                entity.ListCameraTransforms.Remove(selectedItem);
            UpdateGridControl(entity.ListCameraTransforms);
        }

        bool IsAnyItemSelected()
        {
            return gridControl.SelectedItem is CameraTranforms;
        }
    }
}
