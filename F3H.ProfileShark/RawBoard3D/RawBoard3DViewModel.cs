using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Caliburn.Micro;
using F3H.ProfileShark.Helpers;
using F3H.ProfileShark.Models;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.Wpf;
using SharpDX;
using HelixToolkit.Wpf.SharpDX;
using Camera = HelixToolkit.Wpf.SharpDX.Camera;
using Color = System.Windows.Media.Color;
using GradientStop = SharpDX.Direct2D1.GradientStop;
using OrthographicCamera = HelixToolkit.Wpf.SharpDX.OrthographicCamera;
using PerspectiveCamera = HelixToolkit.Wpf.SharpDX.PerspectiveCamera;

// ReSharper disable MemberCanBePrivate.Global


namespace F3H.ProfileShark.RawBoard3D;

public class RawBoard3DViewModel : Screen
{
    #region Lifecycle

    public RawBoard3DViewModel(DataManager dataManager,
        DisplaySettingsViewModel displayControls,
        ItemColorService colorService)
    {
        DataManager = dataManager;
        DisplayControls = displayControls;
        this.colorService = colorService;
        DataManager.ProfileDataAdded += (_, _) => UpdateBoardDisplay();
        DataManager.CameraSelectionChanged += (_, _) => UpdateBoardDisplay();
        DataManager.HeadSelectionChanged += (_, _) => UpdateBoardDisplay();
        DisplayControls.PropertyChanged += (_, _) => UpdateBoardDisplay();
        DataManager.PropertyChanged += SelectedProfileChanged;

        
        EffectsManager = new DefaultEffectsManager();
        perspectiveCamera = new PerspectiveCamera()
        {
            Position = new Point3D(23, 21, 8),
            LookDirection = new Vector3D(-27, -25, -28),
            UpDirection = new Vector3D(-0.37, 0.84, -0.4),
            NearPlaneDistance = 0.1,
            FarPlaneDistance = 15000
        };
        orthographicCamera = new OrthographicCamera()
        {
            Position = new Point3D(23, 21, 8),
            LookDirection = new Vector3D(-27, -25, -28),
            UpDirection = new Vector3D(-0.37, 0.84, -0.4),
            NearPlaneDistance = -100,
            FarPlaneDistance = 15000
        };
        cameraString = "Perspective";
        camera = perspectiveCamera;
        AmbientLightColor = Colors.DimGray;
        DirectionalLightColor = Colors.White;
        BackgroundTexture =
            BitmapExtensions.CreateLinearGradientBitmapStream(EffectsManager, 128, 128, Direct2DImageFormat.Bmp,
                new Vector2(0, 0), new Vector2(0, 128), new GradientStop[]
                {
                    new() { Color = Colors.DarkGray.ToColor4(), Position = 0f },
                    new() { Color = Colors.Black.ToColor4(), Position = 1f }
                });
    }

    #endregion

    #region UI Bound Properties

    public DataManager DataManager { get; }
    public DisplaySettingsViewModel DisplayControls { get; }
    public IEffectsManager EffectsManager { get; }
    public Color DirectionalLightColor { get; private set; }
    public Color AmbientLightColor { get; private set; }
    public Stream BackgroundTexture { get; }
    public PointGeometry3D PointCloudModel { get; } = new();
    public PointGeometry3D CurrentProfileModel { get; } = new();
    public Size RawPointSize => new Size(DisplayControls.RawPointSize, DisplayControls.RawPointSize);

    public Camera Camera
    {
        get => camera;
        set
        {
            if (Equals(value, camera))
            {
                return;
            }

            // camera.CopyTo(value);
            camera = value;
            NotifyOfPropertyChange(() => Camera);
        }
    }

    public string CameraString
    {
        get => cameraString;
        set
        {
            if (value == cameraString)
            {
                return;
            }

            cameraString = value;
            if (cameraString == "Perspective")
            {
                Camera = perspectiveCamera;
            }
            else if (cameraString == "Orthographic")
            {
                Camera = orthographicCamera;
            }

            NotifyOfPropertyChange(() => CameraString);
        }
    }

    public bool DrawerOpen
    {
        get => drawerOpen;
        set
        {
            if (value == drawerOpen)
            {
                return;
            }
            drawerOpen = value;
            NotifyOfPropertyChange(() => DrawerOpen);
        }
    }

    
    #endregion

    #region Public Methods

    public void ResetView()
    {
        Camera.Position = new Point3D(23, 21, 8);
        Camera.LookDirection = new Vector3D(-27, -25, -28);
        Camera.UpDirection = new Vector3D(-0.37, 0.84, -0.4);
        Camera.LookAt(PointCloudModel.BoundingSphere.Center, PointCloudModel.BoundingSphere.Radius * 2);
        FitView();
    }

    public void FitView()
    {
        Camera.LookAt(PointCloudModel.BoundingSphere.Center, PointCloudModel.BoundingSphere.Radius * 2);
        Camera.ZoomExtents(viewport, 1.0);
    }

    #endregion

    #region Private Methods


    private static byte BinByBrightness(double b)
    {
        return (byte)(b); // clamp?
    }

    

    private void UpdateBoardDisplay()
    {
        Camera = DisplayControls.OrthographicCamera ? orthographicCamera : perspectiveCamera;
        
        if (DataManager.Profiles.Count == 0 || !DisplayControls.ShowRawPoints)
        {
            PointCloudModel.Positions.Clear();
            PointCloudModel.Colors.Clear();
            PointCloudModel.UpdateVertices();
            
            return;
        }

        var first = DataManager.Profiles.First();
        var encoderPulseInterval = DisplayControls.EncoderPulseInterval;
        var pts = DataManager.Profiles.Where(q => DataManager.ShowCamera(q.Camera)).SelectMany(p => p.Data.Select(r
            =>
        {
           
            return new Vector3(r.X, r.Y,
                (float)((p.EncoderValue - first.EncoderValue) * encoderPulseInterval));
        }));
        PointCloudModel.Positions = new Vector3Collection(pts);
        PointCloudModel.Colors = new Color4Collection(PointCloudModel.Positions.Count);
        foreach (var pr in DataManager.Profiles.Where(q => DataManager.ShowCamera(q.Camera)))
        {
            foreach (var point2D in pr.Data)
            {
                if (DisplayControls.DisplayMode == DisplayMode.ByCamera)
                {
                    PointCloudModel.Colors.Add(pr.Camera == JoeScan.Pinchot.Camera.CameraA
                        ? DisplayControls.CameraAColor.ToColor4()
                        : DisplayControls.CameraBColor.ToColor4());
                }
                else
                {
                    PointCloudModel.Colors.Add(colorService.LogColorValues[BinByBrightness(point2D.Brightness)].ToColor4());    
                }
            }
        }
        PointCloudModel.UpdateBounds();
        PointCloudModel.UpdateVertices();
        PointCloudModel.UpdateOctree();
    }
    private void SelectedProfileChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(DataManager.SelectedProfile))
        {
            return;
        }

        var selectedProfile = DataManager.SelectedProfile;
        if (selectedProfile == null)
        {
            CurrentProfileModel.Positions.Clear();
            CurrentProfileModel.Colors.Clear();
            CurrentProfileModel.UpdateVertices();
            return;
        }
        var first = DataManager.Profiles.First();
        var encoderPulseInterval = DisplayControls.EncoderPulseInterval;
        CurrentProfileModel.Positions = new Vector3Collection(selectedProfile.Data.Select(r =>
        {
            
            return new Vector3(r.X, r.Y,
                (float)((selectedProfile.EncoderValue - first.EncoderValue) *
                        encoderPulseInterval));
        }));
        CurrentProfileModel.Colors = new Color4Collection(Enumerable.Repeat(Colors.Red.ToColor4(), selectedProfile.Data.Length));
        CurrentProfileModel.UpdateBounds();
        CurrentProfileModel.UpdateVertices();
        CurrentProfileModel.UpdateOctree();
    }
    #endregion


    #region Private Properties

    private readonly ItemColorService colorService;
    private readonly OrthographicCamera orthographicCamera;
    private readonly PerspectiveCamera perspectiveCamera;
    private Camera camera;
    private string cameraString;
    private bool drawerOpen;
    private Viewport3DX viewport;

    #endregion

    #region IViewAware

    protected override void OnViewAttached(object view, object context)
    {
        if (view is RawBoard3DView rawBoard3DView)
        {
            viewport = rawBoard3DView.Viewport;
        }
    }

    #endregion
}