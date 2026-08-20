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
        Glock = Raylib.LoadTexture(Path.Combine("sprites","Guns", "glock_p80.png"));
    }


    public void DrawHud(Player _player)
    {
        const float MAX_STAMINA = 100f;
        const float MAX_HEALTH = 100f;
        Vector2 radialGauge_Position = new Vector2(1160,630);
        
        DrawStaminaBAR(MAX_STAMINA, _player.Stamina);
        DrawRadialGauge(radialGauge_Position, _player.Health, MAX_HEALTH);
    }

    public void DrawStaminaBAR(float _STAMINA_MAX, float _staminaActual)
    {
    if(GameManager.CurrentState!=GameState.Playing) return;
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
    if(GameManager.CurrentState!=GameState.Playing) return;
    float innerRadius = 45f;
    float outRadius = 55f;
    float ringStart = -90f;
    float ringEnd = 90f;
    float totalArc = ringEnd - ringStart;
    float percentage = Math.Clamp(currentValue / maxValue, 0f, 1f);
    float endAngle = ringStart + (totalArc * percentage);
    
    Color GreenCustom = new Color(0,200,0);
    //The colors of health-ring change according to the current state of life (if you've played Resident Evil, you'll understand this)
    Color gaugeColor;
    if(percentage > 0.5f)
        gaugeColor = GreenCustom; //Health: Healthy
    else if(percentage > 0.25f)
        gaugeColor = Color.Yellow; //Health: IN CAUTION
    else
        gaugeColor = Color.Red; //Health: IN DANGER!
    
    string valueText = $"Stamina:\n  {(int)currentValue}";
    int fontSize = 20;
    
    Vector2 textSize = Raylib.MeasureTextEx(Fonts.RequiemFont, valueText, fontSize, 0);
    Vector2 gunPos = new Vector2(
        _center.X - Glock.Width-39,
        _center.Y - Glock.Height-4
    );

    Raylib.SetTextureFilter(Fonts.Montserrat_SemiBoldItalic.Texture, TextureFilter.Bilinear);
    DrawGunIcon(gunPos, _center);
    Raylib.DrawRing(_center, innerRadius, outRadius, ringStart, ringEnd+20, 64, Color.DarkGray);
    Raylib.DrawRing(_center, innerRadius, outRadius, -ringStart, -endAngle, 64, gaugeColor);
    }

    void DrawGunIcon(Vector2 gunPos, Vector2 _center)
    {
        int offsetShadow = 2;
        Vector2 ammo_TextPos = new Vector2(gunPos.X+75, gunPos.Y+48);
        Vector2 shadow_AmmoText = new Vector2(ammo_TextPos.X, ammo_TextPos.Y+offsetShadow);
        Raylib.DrawCircleV(new Vector2(_center.X+4, _center.Y), 50f, Raylib.Fade(Color.LightGray, 0.5f));
        Raylib.DrawRectangle((int)ammo_TextPos.X, (int)ammo_TextPos.Y+10, 66, 22, Raylib.Fade(Color.LightGray, 0.2f));
        Raylib.DrawTextureEx(Glock, gunPos, 0, 2.6f, Color.White);
        Raylib.DrawTextEx(Fonts.Montserrat_SemiBoldItalic, "8/10", shadow_AmmoText, 39f, 0f, Color.Black);
        Raylib.DrawTextEx(Fonts.Montserrat_SemiBoldItalic, "8/10", ammo_TextPos, 38f, 0f, Color.White);
    }
    public void Unload()
    {
        Raylib.UnloadTexture(Glock);
        Raylib.UnloadFont(RequiemFont);
    }
}