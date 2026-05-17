using System.Drawing;

namespace NvidiaDisplayController.Objects.Entities;

public class ProfileSetting
{
    public ProfileSetting()
    {
        Resolution = Size.Empty;
        Frequency = 0;
    }

    public ProfileSetting(double brightness, double contrast, double gamma,
        double digitalVibrance)
        : this(brightness, contrast, gamma, digitalVibrance, Size.Empty, 0)
    {
    }

    public ProfileSetting(double brightness, double contrast, double gamma,
        double digitalVibrance, Size resolution, int frequency)
    {
        Brightness = brightness;
        Contrast = contrast;
        Gamma = gamma;
        DigitalVibrance = digitalVibrance;
        Resolution = resolution;
        Frequency = frequency;
    }

    public double Brightness { get; set; }
    public double Contrast { get; set; }
    public double Gamma { get; set; }
    public double DigitalVibrance { get; set; }
    public Size Resolution { get; set; }
    public int Frequency { get; set; }
}