using System.Collections.Generic;
using System.Linq;
using FluentResults;
using NvidiaDisplayController.Objects.Entities;
using WindowsDisplayAPI;
using WindowsDisplayAPI.DisplayConfig;

namespace NvidiaDisplayController.Objects.Factories;

public class ComputerFactory
{
    private readonly MonitorFactory _monitorFactory;
    private readonly IEnumerable<Display> _displays;
    private readonly PathDisplayTarget[] _pathDisplayTargets;

    public ComputerFactory(MonitorFactory monitorFactory)
    {
        _monitorFactory = monitorFactory;

        _displays = Display.GetDisplays();
        _pathDisplayTargets = PathDisplayTarget.GetDisplayTargets();
    }

    public Result<Computer> Create()
    {
        var computer = new Computer();
        var monitors = new List<Monitor>();

        foreach (var display in _displays)
        {
            var resolution = display.DisplayScreen.CurrentSetting.Resolution;
            var frequency = display.DisplayScreen.CurrentSetting.Frequency;
            var displaySource = ResolveDisplayTarget(display);
            var displayName = displaySource?.FriendlyName ?? display.DisplayName ?? display.DeviceName;

            if (string.IsNullOrWhiteSpace(displayName))
            {
                displayName = display.DeviceName;
            }

            var monitor = _monitorFactory
                .CreateDefault(display.DevicePath, displayName, resolution, frequency);

            monitors.Add(monitor);
        }

        computer.Monitors.AddRange(monitors);

        return computer;
    }

    private PathDisplayTarget? ResolveDisplayTarget(Display display)
    {
        var exactMatch = _pathDisplayTargets.FirstOrDefault(target =>
            DisplayPathHelper.PathsMatch(target.DevicePath, display.DevicePath));

        if (exactMatch is not null)
        {
            return exactMatch;
        }

        var fallbackMatch = _pathDisplayTargets.FirstOrDefault(target =>
            !string.IsNullOrWhiteSpace(target.FriendlyName) &&
            !string.IsNullOrWhiteSpace(display.DisplayName) &&
            target.FriendlyName.Contains(display.DisplayName, System.StringComparison.OrdinalIgnoreCase));

        if (fallbackMatch is not null)
        {
            return fallbackMatch;
        }

        return _pathDisplayTargets.FirstOrDefault(target =>
            DisplayPathHelper.PathsMatch(target.DevicePath, display.ScreenName));
    }
}