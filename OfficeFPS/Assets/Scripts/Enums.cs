using UnityEngine;
public enum MouseSpeed
{
    Setting1,
    Setting2,
    Setting3,
    Setting4,
    Setting5,
    Setting6,
    Setting7,
    Setting8,
    Setting9,
    Setting10
}

public enum RGBSettings
{
    RED,
    GREEN,
    BLUE,
    NONE
}

public static class RGBSettingsExtensions
{
    public static Color ToColor(this RGBSettings setting)
    {
        switch (setting)
        {
            case RGBSettings.RED:   return new Color(1f, 0f, 0f, 1f);
            case RGBSettings.GREEN: return new Color(0f, 1f, 0f, 1f);
            case RGBSettings.BLUE:  return new Color(0f, 0f, 1f, 1f);
            default: return Color.white;
        }
    }
}