using System.Drawing;
using WindowsDisplayAPI;

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
            : this(brightness, contrast, gamma, digitalVibrance, Size.Empty, 0, ColorDepth.Depth32Bit, false)
        {
        }
    

    public ProfileSetting(double brightness, double contrast, double gamma,
        double digitalVibrance, Size resolution, int frequency, ColorDepth colorDepth = ColorDepth.Depth32Bit, bool isInterlaced = false)
    {
        Brightness = brightness;
        Contrast = contrast;
        Gamma = gamma;
        DigitalVibrance = digitalVibrance;
        Resolution = resolution;
        Frequency = frequency;
        ColorDepth = colorDepth;
        IsInterlaced = isInterlaced;
    }

    public double Brightness { get; set; }
    public double Contrast { get; set; }
    public double Gamma { get; set; }
    public double DigitalVibrance { get; set; }
    public Size Resolution { get; set; }
    public int Frequency { get; set; }
    public ColorDepth ColorDepth { get; set; } = ColorDepth.Depth32Bit;
    public bool IsInterlaced { get; set; }
}