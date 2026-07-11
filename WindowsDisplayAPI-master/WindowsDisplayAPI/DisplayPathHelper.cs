using System;

namespace WindowsDisplayAPI;

public static class DisplayPathHelper
{
    public static string NormalizeDevicePath(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return string.Empty;
        }

        var normalized = path.Trim().Replace('/', '\\');

        if (normalized.StartsWith(@"\\?\", StringComparison.OrdinalIgnoreCase))
        {
            normalized = normalized[4..];
        }

        if (normalized.StartsWith(@"\\.\", StringComparison.OrdinalIgnoreCase))
        {
            normalized = normalized[4..];
        }

        normalized = normalized.Replace("#", "\\");

        return normalized.Trim().ToUpperInvariant();
    }

    public static bool PathsMatch(string? left, string? right)
    {
        if (string.IsNullOrWhiteSpace(left) || string.IsNullOrWhiteSpace(right))
        {
            return string.Equals(left, right, StringComparison.OrdinalIgnoreCase);
        }

        return string.Equals(NormalizeDevicePath(left), NormalizeDevicePath(right), StringComparison.OrdinalIgnoreCase);
    }
}
