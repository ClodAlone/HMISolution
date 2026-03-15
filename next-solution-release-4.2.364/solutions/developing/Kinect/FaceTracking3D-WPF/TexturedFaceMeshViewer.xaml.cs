// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TexturedFaceMeshViewer.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FaceTracking3D
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Media.Media3D;
    using Microsoft.Kinect;
    using Microsoft.Kinect.Toolkit.FaceTracking;

    using Point = System.Windows.Point;

    /// <summary>
    /// Interaction logic for TexturedFaceMeshViewer.xaml
    /// </summary>
    public partial class TexturedFaceMeshViewer : UserControl, IDisposable
    {
        public static readonly DependencyProperty KinectProperty = DependencyProperty.Register(
            "Kinect", 
            typeof(KinectSensor), 
            typeof(TexturedFaceMeshViewer), 
            new UIPropertyMetadata(
                null, 
                (o, args) =>
                ((TexturedFaceMeshViewer)o).OnKinectChanged((KinectSensor)args.OldValue, (KinectSensor)args.NewValue)));

        private static readonly int Bgr32BytesPerPixel = (PixelFormats.Bgr32.BitsPerPixel + 7) / 8;

        private byte[] colorImage;

        private ColorImageFormat colorImageFormat = ColorImageFormat.Undefined;

        private WriteableBitmap colorImageWritableBitmap;

        private short[] depthImage;

        private DepthImageFormat depthImageFormat = DepthImageFormat.Undefined;

        private FaceTracker faceTracker;

        private Skeleton[] skeletonData;

        private int trackingId = -1;

        private FaceTriangle[] triangleIndices;

        public TexturedFaceMeshViewer()
        {
            this.DataContext = this;
            this.InitializeComponent();
        }

        public KinectSensor Kinect
        {
            get
            {
                return (KinectSensor)this.GetValue(KinectProperty);
            }

            set
            {
                this.SetValue(KinectProperty, value);
            }
        }

        public void Dispose()
        {
            this.DestroyFaceTracker();
        }

        private void AllFramesReady(object sender, AllFramesReadyEventArgs allFramesReadyEventArgs)
        {
            ColorImageFrame colorImageFrame = null;
            DepthImageFrame depthImageFrame = null;
            SkeletonFrame skeletonFrame = null;

            try
            {
                colorImageFrame = allFramesReadyEventArgs.OpenColorImageFrame();
                depthImageFrame = allFramesReadyEventArgs.OpenDepthImageFrame();
                skeletonFrame = allFramesReadyEventArgs.OpenSkeletonFrame();

                if (colorImageFrame == null || depthImageFrame == null || skeletonFrame == null)
                {
                    return;
                }

                // Check for changes in any of the data this function is receiving
                // and reset things appropriately.
                if (this.depthImageFormat != depthImageFrame.Format)
                {
                    this.DestroyFaceTracker();
                    this.depthImage = null;
                    this.depthImageFormat = depthImageFrame.Format;
                }

                if (this.colorImageFormat != colorImageFrame.Format)
                {
                    this.DestroyFaceTracker();
                    this.colorImage = null;
                    this.colorImageFormat = colorImageFrame.Format;
                    this.colorImageWritableBitmap = null;
                    this.ColorImage.Source = null;
                    this.theMaterial.Brush = null;
                }

                if (this.skeletonData != null && this.skeletonData.Length != skeletonFrame.SkeletonArrayLength)
                {
                    this.skeletonData = null;
                }

                // Create any buffers to store copies of the data we work with
                if (this.depthImage == null)
                {
                    this.depthImage = new short[depthImageFrame.PixelDataLength];
                }

                if (this.colorImage == null)
                {
                    this.colorImage = new byte[colorImageFrame.PixelDataLength];
                }

                if (this.colorImageWritableBitmap == null)
                {
                    this.colorImageWritableBitmap = new WriteableBitmap(
                        colorImageFrame.Width, colorImageFrame.Height, 96, 96, PixelFormats.Bgr32, null);
                    this.ColorImage.Source = this.colorImageWritableBitmap;
                    this.theMaterial.Brush = new ImageBrush(this.colorImageWritableBitmap)
                        {
                           ViewportUnits = BrushMappingMode.Absolute 
                        };
                }

                if (this.skeletonData == null)
                {
                    this.skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];
                }

                // Copy data received in this event to our buffers.
                colorImageFrame.CopyPixelDataTo(this.colorImage);
                depthImageFrame.CopyPixelDataTo(this.depthImage);
                skeletonFrame.CopySkeletonDataTo(this.skeletonData);
                this.colorImageWritableBitmap.WritePixels(
                    new Int32Rect(0, 0, colorImageFrame.Width, colorImageFrame.Height), 
                    this.colorImage, 
                    colorImageFrame.Width * Bgr32BytesPerPixel, 
                    0);

                // Find a skeleton to track.
                // First see if our old one is good.
                // When a skeleton is in PositionOnly tracking state, don't pick a new one
                // as it may become fully tracked again.
                Skeleton skeletonOfInterest =
                    this.skeletonData.FirstOrDefault(
                        skeleton =>
                        skeleton.TrackingId == this.trackingId
                        && skeleton.TrackingState != SkeletonTrackingState.NotTracked);

                if (skeletonOfInterest == null)
                {
                    // Old one wasn't around.  Find any skeleton that is being tracked and use it.
                    skeletonOfInterest =
                        this.skeletonData.FirstOrDefault(
                            skeleton => skeleton.TrackingState == SkeletonTrackingState.Tracked);

                    if (skeletonOfInterest != null)
                    {
                        // This may be a different person so reset the tracker which
                        // could have tuned itself to the previous person.
                        if (this.faceTracker != null)
                        {
                            this.faceTracker.ResetTracking();
                        }

                        this.trackingId = skeletonOfInterest.TrackingId;
                    }
                }

                bool displayFaceMesh = false;

                if (skeletonOfInterest != null && skeletonOfInterest.TrackingState == SkeletonTrackingState.Tracked)
                {
                    if (this.faceTracker == null)
                    {
                        try
                        {
                            this.faceTracker = new FaceTracker(this.Kinect);
                        }
                        catch (InvalidOperationException)
                        {
                            // During some shutdown scenarios the FaceTracker
                            // is unable to be instantiated.  Catch that exception
                            // and don't track a face.
                            Debug.WriteLine("AllFramesReady - creating a new FaceTracker threw an InvalidOperationException");
                            this.faceTracker = null;
                        }
                    }

                    if (this.faceTracker != null)
                    {
                        FaceTrackFrame faceTrackFrame = this.faceTracker.Track(
                            this.colorImageFormat,
                            this.colorImage,
                            this.depthImageFormat,
                            this.depthImage,
                            skeletonOfInterest);

                        if (faceTrackFrame.TrackSuccessful)
                        {
                            this.UpdateMesh(faceTrackFrame);

                            // Only display the face mesh if there was a successful track.
                            displayFaceMesh = true;
                        }
                    }
                }
                else
                {
                    this.trackingId = -1;
                }

                this.viewport3d.Visibility = displayFaceMesh ? Visibility.Visible : Visibility.Hidden;
            }
            finally
            {
                if (colorImageFrame != null)
                {
                    colorImageFrame.Dispose();
                }

                if (depthImageFrame != null)
                {
                    depthImageFrame.Dispose();
                }

                if (skeletonFrame != null)
                {
                    skeletonFrame.Dispose();
                }
            }
        }

        private void DestroyFaceTracker()
        {
            if (this.faceTracker != null)
            {
                this.faceTracker.Dispose();
                this.faceTracker = null;
            }
        }

        private void OnKinectChanged(KinectSensor oldSensor, KinectSensor newSensor)
        {
            if (oldSensor != null)
            {
                try
                {
                    oldSensor.AllFramesReady -= this.AllFramesReady;

                    this.DestroyFaceTracker();
                }
                catch (InvalidOperationException)
                {
                    // KinectSensor might enter an invalid state while enabling/disabling streams or stream features.
                    // E.g.: sensor might be abruptly unplugged.
                }
            }

            if (newSensor != null)
            {
                try
                {
                    this.faceTracker = new FaceTracker(this.Kinect);

                    newSensor.AllFramesReady += this.AllFramesReady;
                }
                catch (InvalidOperationException)
                {
                    // KinectSensor might enter an invalid state while enabling/disabling streams or stream features.
                    // E.g.: sensor might be abruptly unplugged.
                }
            }
        }

        private void UpdateMesh(FaceTrackFrame faceTrackingFrame)
        {
            EnumIndexableCollection<FeaturePoint, Vector3DF> shapePoints = faceTrackingFrame.Get3DShape();
            EnumIndexableCollection<FeaturePoint, PointF> projectedShapePoints = faceTrackingFrame.GetProjected3DShape();

            if (this.triangleIndices == null)
            {
                // Update stuff that doesn't change from frame to frame
                this.triangleIndices = faceTrackingFrame.GetTriangles();
                var indices = new Int32Collection(this.triangleIndices.Length * 3);
                foreach (FaceTriangle triangle in this.triangleIndices)
                {
                    indices.Add(triangle.Third);
                    indices.Add(triangle.Second);
                    indices.Add(triangle.First);
                }

                this.theGeometry.TriangleIndices = indices;
                this.theGeometry.Normals = null; // Let WPF3D calculate these.

                this.theGeometry.Positions = new Point3DCollection(shapePoints.Count);
                this.theGeometry.TextureCoordinates = new PointCollection(projectedShapePoints.Count);
                for (int pointIndex = 0; pointIndex < shapePoints.Count; pointIndex++)
                {
                    this.theGeometry.Positions.Add(new Point3D());
                    this.theGeometry.TextureCoordinates.Add(new Point());
                }
            }

            // Update the 3D model's vertices and texture coordinates
            for (int pointIndex = 0; pointIndex < shapePoints.Count; pointIndex++)
            {
                Vector3DF point = shapePoints[pointIndex];
                this.theGeometry.Positions[pointIndex] = new Point3D(point.X, point.Y, -point.Z);

                PointF projected = projectedShapePoints[pointIndex];

                this.theGeometry.TextureCoordinates[pointIndex] =
                    new Point(
                        projected.X / (double)this.colorImageWritableBitmap.PixelWidth, 
                        projected.Y / (double)this.colorImageWritableBitmap.PixelHeight);
            }
        }
    }
}