using System.Numerics;
using Players;
using Raylib_cs;
using ImGuiNET;
using rlImGui_cs;
public class HUD
{   
    Font RequiemFont;
    Texture2D Glock;
    public HUD()
    {
        RequiemFont = Raylib.LoadFont(Path.Combine("fonts", "Requiem_RE9.ttf"));
        Glock = Raylib.LoadTexture(Path.Combine("sprites","Guns", "glock_p80.png"));
    }

    public void DrawStaminaBAR(float _STAMINA_MAX, float _staminaActual)
    {
    float barX = 50.0f;
    float barY = 680.0f;
    float barMaxWidth = 200.0f;
    float barMaxHeight = 30.0f;
    float proportionalWidth = barMaxWidth * (_staminaActual / _STAMINA_MAX);
    Rectangle borderRec =  new Rectangle(barX,barY, barMaxWidth, barMaxHeight);
    Raylib.DrawText($"Stamina: {(int)_staminaActual}", 50, 655, 28, Color.Black);
    Raylib.DrawRectangle((int)barX, (int)barY, (int)barMaxWidth, (int)barMaxHeight, Color.DarkGray);
    Raylib.DrawRectangle((int)barX, (int)barY, (int)proportionalWidth, (int)barMaxHeight, Color.Blue);
    Raylib.DrawRectangleLines((int)barX, (int)barY, (int)barMaxWidth, (int)barMaxHeight, Color.Black);
    Raylib.DrawRectangleLinesEx(borderRec, 3f, Color.DarkBlue);
    }

    public void DrawRadialGauge (Vector2 _center, float currentValue, float maxValue)
    {
    float innerRadius = 45f;
    float outRadius = 55f;
    float ringStart = -90f;
    float ringEnd = 90f;
    float totalArc = ringEnd - ringStart;
    float percentage = Math.Clamp(currentValue / maxValue, 0f, 1f);
    float endAngle = ringStart + (totalArc * percentage);
    Color color_Glock = new Color(255, 255, 255);
    

    Color gaugeColor;
    if(percentage > 0.5f)
        gaugeColor = Color.Green;
    else if(percentage > 0.25f)
        gaugeColor = Color.Yellow;
    else
        gaugeColor = Color.Red;
    
    Vector2 backGroundhud = new Vector2(_center.X+2, _center.Y);
    
    Raylib.DrawCircleV(new Vector2(_center.X+4, _center.Y), 44f, Color.LightGray);
    Raylib.DrawRing(_center, innerRadius, outRadius, ringStart, ringEnd+20, 64, Color.DarkGray);
    Raylib.DrawRing(_center, innerRadius, outRadius, -ringStart, -endAngle, 64, gaugeColor);
    
    string valueText = $"Stamina:\n  {(int)currentValue}";
    int fontSize = 20;
    Vector2 textSize = Raylib.MeasureTextEx(RequiemFont, valueText, fontSize, 0);
    Vector2 allignedPos = new Vector2(
        _center.X - Glock.Width-2,
        _center.Y - Glock.Height+10
    );
    Raylib.DrawTextureEx(Glock, allignedPos, 0, 2.1f, color_Glock);
    
    }

    public void Unload()
    {
        Raylib.UnloadTexture(Glock);
        Raylib.UnloadFont(RequiemFont);
    }
}