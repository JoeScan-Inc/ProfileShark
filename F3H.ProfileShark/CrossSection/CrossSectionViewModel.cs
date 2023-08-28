using System.ComponentModel;
using Caliburn.Micro;
using F3H.ProfileShark.Helpers;
using F3H.ProfileShark.Models;
using NLog;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;

namespace F3H.ProfileShark.CrossSection;

public class CrossSectionViewModel : Screen
{
    #region Private Fields

    private LinearAxis columnAxis;
    private LinearAxis rowAxis;
    private bool showBoundingBox = false;
    private bool showFilters = true;
    private readonly Dictionary<uint, Annotation> filterOutlines = new();

    #endregion

    #region Injecteded Properties

    public DataManager DataManager { get; }
    public PlotColorService PlotColorService { get; }
    public ILogger Logger { get; }

    #endregion

    #region UI Bound Properties

    public PlotModel CrossSectionPlot { get; set; }

    public bool ShowBoundingBox
    {
        get => showBoundingBox;
        set
        {
            if (value == showBoundingBox) return;
            showBoundingBox = value;
            NotifyOfPropertyChange(() => ShowBoundingBox);
            RefreshPlot();
        }
    }
    
    public bool ShowFilters
    {
        get => showFilters;
        set
        {
            if (value == showFilters)
            {
                return;
            }
            showFilters = value;
            NotifyOfPropertyChange(() => ShowFilters);
            RefreshPlot();
        }
    }

    #endregion

    #region Lifecycle

    public CrossSectionViewModel(DataManager dataManager, PlotColorService plotColorService, ILogger logger)
    {
        DataManager = dataManager;
        PlotColorService = plotColorService;
        Logger = logger;
        SetupPlot();
        DataManager.PropertyChanged += DataManagerOnPropertyChanged;
    }

    #endregion

    private void DataManagerOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        
        if (e.PropertyName != nameof(DataManager.SelectedProfile))
        {
            return;
        }
        RefreshPlot();
    }

    private void RefreshPlot()
    {
        var p = DataManager.SelectedProfile;
        CrossSectionPlot.Series.Clear();
        CrossSectionPlot.Annotations.Clear();
        if (p != null)
        {
            var series = new ScatterSeries()
            {
                MarkerFill = ColorDefinitions.OxyColorForCableId(p.ScanHeadId),
                MarkerStroke = ColorDefinitions.OxyColorForCableId(p.ScanHeadId),
                MarkerType = MarkerType.Plus,
                MarkerSize = 1
            };
            CrossSectionPlot.Series.Add(series);
            series.Points.AddRange(p.Data.Select(q => new ScatterPoint(q.X, q.Y)));
            if (ShowBoundingBox)
            {
                // CrossSectionPlot.Annotations.Add(new RectangleAnnotation()
                // {
                //     MinimumX = p.Profile.BoundingBox.Left,
                //     MaximumX = p.Profile.BoundingBox.Right,
                //     MinimumY = p.Profile.BoundingBox.Bottom,
                //     MaximumY = p.Profile.BoundingBox.Top,
                //     Stroke = OxyColor.FromArgb(100, ColorDefinitions.OxyColorForCableId(p.ScanHeadId).R,
                //         ColorDefinitions.OxyColorForCableId(p.ScanHeadId).G, ColorDefinitions.OxyColorForCableId(p.ScanHeadId).B),
                //     StrokeThickness = 1,
                //     Fill = OxyColors.Transparent
                //
                // });
            }

            if (ShowFilters && filterOutlines.TryGetValue(p.ScanHeadId, out var outline))
            {
                CrossSectionPlot.Annotations.Add(outline);
            }
        }
        CrossSectionPlot.InvalidatePlot(true);
    }

    private void SetupPlot()
    {
        CrossSectionPlot = new PlotModel
        {
            PlotType = PlotType.Cartesian,
            Background = PlotColorService.PlotBackgroundColor,
            PlotAreaBorderColor = PlotColorService.PlotAreaBorderColor, // not visible anyway
            PlotAreaBorderThickness = new OxyThickness(1),
            // PlotMargins = new OxyThickness(0)
        };

        CrossSectionPlot.Legends.Add(new Legend
        {
            LegendPosition = LegendPosition.BottomRight,
            LegendTextColor = PlotColorService.LegendTextColor,
            LegendBackground = OxyColors.Transparent,
            IsLegendVisible = true
        });

        columnAxis = new LinearAxis
        {
            Minimum = -20,
            Maximum = 20,
            PositionAtZeroCrossing = true,
            AxislineStyle = LineStyle.Solid,
            AxislineColor = PlotColorService.MajorGridLineColor,
            AxislineThickness = 1.3,
            TickStyle = TickStyle.Inside,
            TicklineColor = PlotColorService.MinorGridLineColor,
            MajorGridlineStyle = LineStyle.Solid,
            MinorGridlineStyle = LineStyle.Solid,
            MajorGridlineColor = PlotColorService.MajorGridLineColor,
            MinorGridlineColor = PlotColorService.MinorGridLineColor,
            IsZoomEnabled = true,
            TextColor = PlotColorService.AxisTextColor,
            Position = AxisPosition.Bottom,
             LabelFormatter = q => $"{q} \""
        };
        CrossSectionPlot.Axes.Add(columnAxis);

        rowAxis = new LinearAxis
        {
            Minimum = -1,
            Maximum = 10,
            PositionAtZeroCrossing = true,
            AxislineStyle = LineStyle.Solid,
            AxislineColor = PlotColorService.MajorGridLineColor,
            AxislineThickness = 1.3,
            TickStyle = TickStyle.Inside,
            TicklineColor = PlotColorService.MinorGridLineColor,
            MajorGridlineStyle = LineStyle.Solid,
            MinorGridlineStyle = LineStyle.Solid,
            MajorGridlineColor = PlotColorService.MajorGridLineColor,
            MinorGridlineColor = PlotColorService.MinorGridLineColor,
            IsZoomEnabled = true,
            TextColor = PlotColorService.AxisTextColor,
            Position = AxisPosition.Left,
            LabelFormatter = q => $"{q} \""
        };
        CrossSectionPlot.Axes.Add(rowAxis);

       
    }
}
