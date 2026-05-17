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
                var targetMode = display.DisplayScreen.GetPossibleSettings()
                    .SingleOrDefault(mode => mode.Resolution == profileSetting.Resolution && mode.Frequency == profileSetting.Frequency);

                if (targetMode != null)
                {
                    var newSetting = new DisplaySetting(targetMode,
                        currentSetting.Position,
                        currentSetting.Orientation,
                        currentSetting.OutputScalingMode);

                    display.DisplayScreen.SetSettings(newSetting, true);
                }
                else
                {
                    _logger.Warn($"Target display mode not found for resolution {profileSetting.Resolution} @ {profileSetting.Frequency}Hz. Falling back to manual setting.");

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