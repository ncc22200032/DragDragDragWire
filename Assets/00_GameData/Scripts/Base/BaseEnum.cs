/// <summary>
/// —ñ‹“Œ^‚ğŠÇ—‚·‚éƒNƒ‰ƒX
/// </summary>
public static class BaseEnum
{
    public enum Switch
    {
        ON, OFF
    }
    public enum Scheme
    {
        KeyboardMouse, Gamepad
    }
    public enum SceneState
    {
        TITLE,
        STAGE01, STAGE02, STAGE03,
    }
    public enum GameState
    {
        DEFAULT, PAUSE, GAMECLEAR, GAMEOVER
    }

    public static readonly float WIDTH = 1920;
    public static readonly float Height = 1080;
}
