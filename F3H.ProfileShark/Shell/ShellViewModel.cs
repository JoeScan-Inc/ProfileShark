﻿using Caliburn.Micro;
using F3H.ProfileShark.CrossSection;
using F3H.ProfileShark.Grid;
using F3H.ProfileShark.Models;
using F3H.ProfileShark.ProfileDetail;
using F3H.ProfileShark.RawBoard3D;
using F3H.ProfileShark.Timeline;
using F3H.ProfileShark.Toolbar;
using NLog;

namespace F3H.ProfileShark.Shell;

public class ShellViewModel : Screen
{
    private RawProfile selectedProfile;
    private double encoderPulseInterval;
    public DataManager DataManager { get; }
    public ToolbarViewModel ToolBar { get; }
    public RawProfileGridViewModel DataGridView { get; }
    public CrossSectionViewModel CrossSection { get; }
    public TimelinePlotViewModel TimelinePlot { get; }
    
    public RawBoard3DViewModel RawBoard3D { get; }
    public ProfileDetailViewModel ProfileDetail { get; }
    public ILogger Logger { get; }

   
    public ShellViewModel(DataManager dataManager,
        ToolbarViewModel toolBar,
        RawProfileGridViewModel dataGridView,
        CrossSectionViewModel crossSection,
        TimelinePlotViewModel timelinePlot,
        RawBoard3DViewModel rawBoard3D,
        ProfileDetailViewModel profileDetail,
        ILogger logger)
    {
        DataManager = dataManager;
        ToolBar = toolBar;
        DataGridView = dataGridView;
        CrossSection = crossSection;
        TimelinePlot = timelinePlot;
        RawBoard3D = rawBoard3D;
        ProfileDetail = profileDetail;
        Logger = logger;
    }
}