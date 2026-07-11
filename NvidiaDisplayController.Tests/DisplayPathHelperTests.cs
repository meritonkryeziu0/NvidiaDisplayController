using WindowsDisplayAPI;
using Xunit;

namespace NvidiaDisplayController.Tests;

public class DisplayPathHelperTests
{
    [Theory]
    [InlineData("\\\\?\\PCI#VEN_10DE&DEV_1B80&SUBSYS_00000000&REV_A1#4&2FD787A9&0&0000000E#{E6CF7B14-...}", "\\\\.\\PCI#VEN_10DE&DEV_1B80&SUBSYS_00000000&REV_A1#4&2FD787A9&0&0000000E#{E6CF7B14-...}")]
    [InlineData("\\\\?\\DISPLAY#ABC123#{...}", "DISPLAY#ABC123#{...}")]
    [InlineData("\\\\?\\DISPLAY#ABC123#{...}", "\\\\?\\DISPLAY#ABC123#{...}")]
    public void PathsMatch_HandlesDifferentPathRepresentations(string left, string right)
    {
        Assert.True(DisplayPathHelper.PathsMatch(left, right));
    }

    [Fact]
    public void PathsMatch_ReturnsFalseForDifferentPaths()
    {
        Assert.False(DisplayPathHelper.PathsMatch("\\\\?\\DISPLAY#A", "\\\\?\\DISPLAY#B"));
    }
}
