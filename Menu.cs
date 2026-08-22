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
        Raylib.DrawText("WASD - Movement", 350,120, 50,Color.LightGray);
        Raylib.DrawText("Shift - Run", 350,170, 50,Color.LightGray);
        Raylib.DrawText("1 - Switch Weapons", 350,210, 50,Color.LightGray);
        Raylib.DrawText("R - Reload Equipped Gun", 350,250, 50,Color.LightGray);
        Raylib.DrawText("Mouse - Aim", 350,290, 50,Color.LightGray);
        Raylib.DrawText("Mouse Button Left - Fire", 350,330, 50,Color.LightGray);
        Raylib.DrawText("Press enter to play", 510,650, 30,Color.LightGray);
    }
}

public static class PausedMenu
{
    static int fontSize = 50;
    static string textPause = "PAUSED";
    
    static Vector2 textSize = Raylib.MeasureTextEx(Fonts.RequiemFont, textPause, fontSize, 0);
    static Vector2 textCentered = new Vector2(Raylib.GetScreenWidth()/2 - fontSize, Raylib.GetScreenHeight()/2 - fontSize/2);
    static Vector2 allignedPos = new Vector2(580, 340);
    
    public static void Input()
    {
    if(GameManager.CurrentState!=GameState.Paused) return;
        if(Raylib.IsKeyPressed(KeyboardKey.M))
        {
            GameManager.Push(GameState.Options);
        }
    }
    public static void Draw()
    {
        if(GameManager.CurrentState!=GameState.Paused) return;
        Raylib.DrawRectangle(0,0,1280,720, Raylib.Fade(Color.DarkGray,0.6f));
        Raylib.DrawTextEx(Fonts.RequiemFont, textPause, textCentered, fontSize, 0, Color.White);
        
        Raylib.DrawTextEx(Fonts.RequiemFont, "M - Menu", new Vector2(580, 645), fontSize, 0, Color.White);
    }
}

public static class OptionsMenu
{
    static int widthButton = 100;
    static int heightButton = 50;
    static int fontSize = 20;
    static float button_X = (Raylib.GetScreenWidth() - widthButton) / 2.0f;
    static float button_Y = (Raylib.GetScreenHeight() - heightButton) / 2.0f;
    static Rectangle button_rect = new Rectangle ((int)button_X, (int)button_Y, widthButton, heightButton);

    public static void Draw()
    {
        if(GameManager.CurrentState!=GameState.Options) return;
        Raylib.DrawRectangle(0,0,1280,720, Raylib.Fade(Color.DarkGray,0.6f));
        Raylib.DrawRectangleRec(button_rect, Color.DarkBrown);
        Raylib.DrawTextEx(Fonts.RequiemFont, "TESTE", new Vector2(button_X, button_Y), fontSize, 0f, Color.White);
    }
}
