using System;
using System.Drawing;
using NLog;
using NvidiaDisplayController.Objects;
using NvidiaDisplayController.Objects.Entities;
using WindowsDisplayAPI;

namespace NvidiaDisplayController.Global.Controllers;

public class DisplayController
{
    private readonly ILogger _logger;
    private readonly NvidiaDisplayWindowManager _windowManager;

    public DisplayController(ILogger logger, NvidiaDisplayWindowManager windowManager)
    {
        _logger = logger;
        _windowManager = windowManager;
    }

    public void UpdateColorSettings(Display display, ProfileSetting profileSetting,
        NvAPIWrapper.Display.Display? nvidiaMonitor)
    {
        try
        {
            var currentSetting = display.DisplayScreen.CurrentSetting;

            if (profileSetting.Resolution != Size.Empty && profileSetting.Frequency > 0 &&
                (profileSetting.Resolution != currentSetting.Resolution || profileSetting.Frequency != currentSetting.Frequency))
            {
                var newSetting = new DisplaySetting(
                    profileSetting.Resolution,
                    currentSetting.Position,
                    currentSetting.ColorDepth,
                    profileSetting.Frequency,
                    currentSetting.IsInterlaced,
                    currentSetting.Orientation,
                    currentSetting.OutputScalingMode);

                display.DisplayScreen.SetSettings(newSetting, true);
            }

            display.GammaRamp =
                new DisplayGammaRamp(profileSetting.Brightness, profileSetting.Contrast, profileSetting.Gamma);
            if (nvidiaMonitor is not null)
                nvidiaMonitor.DigitalVibranceControl.NormalizedLevel = profileSetting.DigitalVibrance - .3;
        }
        catch (Exception e)
        {
            var message = "Failed to update display and color settings.";

            _logger.Error(message);
            _logger.Error(e);

            _windowManager.ShowMessageBox(message);
        }
    }
}