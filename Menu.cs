using System.Numerics;
using Raylib_cs;

public static class MainMenu
{

    public static Sound confirm_MENU = Raylib.LoadSound(Path.Combine("audio","sfx","Menu_Confirm.wav"));
    public static Sound back_MENU = Raylib.LoadSound(Path.Combine("audio","sfx","Menu_Back.wav"));
    public static void Input()
    {
        if(GameManager.CurrentState!=GameState.MainMenu) return;

        if(Raylib.IsKeyPressed(KeyboardKey.Enter))
        {
            Raylib.PlaySound(confirm_MENU);
            GameManager.Push(GameState.Playing);
        }
    }

    public static void Draw()
    {
        if(GameManager.CurrentState!=GameState.MainMenu) return;
        Raylib.ClearBackground(Color.Black);
        Raylib.DrawText("Press enter to play", 510,650, 30,Color.LightGray);
    }
}

public static class PausedMenu
{
    static int fontSize = 50;
    static string textPause = "PAUSED";
    static Vector2 textSize = Raylib.MeasureTextEx(Fonts.RequiemFont, textPause, fontSize, 0);
    static Vector2 allignedPos = new Vector2(580, 340);
    
    public static void Input()
    {
       
    }
    public static void Draw()
    {
        if(GameManager.CurrentState!=GameState.Paused) return;
        Raylib.DrawRectangle(0,0,1280,720, Raylib.Fade(Color.DarkGray,0.6f));
        Raylib.DrawTextEx(Fonts.RequiemFont, textPause, allignedPos, fontSize, 0, Color.White);
    }
}
