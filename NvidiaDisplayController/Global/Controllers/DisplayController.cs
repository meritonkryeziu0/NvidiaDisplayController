using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
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
                var possibleModes = display.DisplayScreen.GetPossibleSettings().ToList();
                var targetMode = possibleModes
                    .SingleOrDefault(mode => mode.Resolution == profileSetting.Resolution && mode.Frequency == profileSetting.Frequency);

                if (targetMode != null)
                {
                    _logger.Info($"Found exact target mode: {targetMode}");
                    var newSetting = new DisplaySetting(targetMode, currentSetting.Position);
                    display.DisplayScreen.SetSettings(newSetting, true);
                }
                else
                {
                    _logger.Warn($"Target display mode not found for resolution {profileSetting.Resolution} @ {profileSetting.Frequency}Hz.");
                    _logger.Warn($"Available modes: {string.Join(", ", possibleModes)}");

                    var fallbackMode = possibleModes
                        .FirstOrDefault(mode => mode.Resolution == profileSetting.Resolution)
                        ?? possibleModes.FirstOrDefault();

                    if (fallbackMode != null)
                    {
                        _logger.Warn($"Falling back to nearest available mode: {fallbackMode}");
                        var newSetting = new DisplaySetting(fallbackMode, currentSetting.Position);
                        display.DisplayScreen.SetSettings(newSetting, true);
                    }
                    else
                    {
                        _logger.Warn("No available modes found; skipping resolution change.");
                    }
                }
            }

            display.GammaRamp =
                new DisplayGammaRamp(profileSetting.Brightness, profileSetting.Contrast, profileSetting.Gamma);
            if (nvidiaMonitor is not null)
                nvidiaMonitor.DigitalVibranceControl.NormalizedLevel = profileSetting.DigitalVibrance - .3;
        }
        catch (Exception e)
        {
            var message = $"Failed to update display and color settings.\n\n" +
                          $"Target resolution: {profileSetting.Resolution.Width}x{profileSetting.Resolution.Height} @ {profileSetting.Frequency}Hz\n" +
                          $"Brightness: {profileSetting.Brightness}, Contrast: {profileSetting.Contrast}, Gamma: {profileSetting.Gamma}, DigitalVibrance: {profileSetting.DigitalVibrance}\n\n" +
                          $"Exception: {e.Message}\n\n{e}\n";

            _logger.Error(message);
            _logger.Error(e);
            AppendStartupLog(message);

            _windowManager.ShowMessageBox(message);
        }
    }

    private void AppendStartupLog(string message)
    {
        try
        {
            var baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? Environment.CurrentDirectory;
            var logPath = Path.Combine(baseDir, "startup.log");
            File.AppendAllText(logPath, $"{DateTime.UtcNow:o} - {message}{Environment.NewLine}");
        }
        catch { }
    }
}
