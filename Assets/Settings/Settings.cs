public static class Settings
{
    public static Quality qualitySettings = new Quality();
}

public struct Quality
{
    public int resolutionWidth;
    public int resolutionHeight;
    public bool fullscreen;
    public int antiAliasing;
}
