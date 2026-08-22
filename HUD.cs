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
        DrawRadialGauge(radialGauge_Position, _player, MAX_HEALTH);
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

    public void DrawRadialGauge (Vector2 _center, Player _player, float maxValue)
    {
    if(GameManager.CurrentState!=GameState.Playing) return;
    float currentValue = _player.Health;
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
    

    Raylib.SetTextureFilter(Fonts.Montserrat_SemiBoldItalic.Texture, TextureFilter.Bilinear);
    DrawGunIcon(_center, _player.EquippedWeapon);
    Raylib.DrawRing(_center, innerRadius, outRadius, ringStart, ringEnd+20, 64, Color.DarkGray);
    Raylib.DrawRing(_center, innerRadius, outRadius, -ringStart, -endAngle, 64, gaugeColor);
    }

    void DrawGunIcon(Vector2 _center, Gun _gun)
    {
        if(_gun == null) return;
        
        Vector2 gunPos = new Vector2(
        _center.X - _gun.Icon.Width-44,
        _center.Y - _gun.Icon.Height-20
        );
        
        int offsetShadow = 2;
        float fontSize = 34f;
        float scaleImage = 2.6f;
        Vector2 ammo_TextPos = new Vector2(gunPos.X+78, gunPos.Y+46);
        Vector2 shadow_AmmoText = new Vector2(ammo_TextPos.X, ammo_TextPos.Y+offsetShadow);
        
        if(_gun.Grip==TypeGrip.Pistol) 
        {
            scaleImage = 3.4f;
            gunPos.Y-=10;
            ammo_TextPos.X+=6;
            ammo_TextPos.Y+=6;
            shadow_AmmoText.X+=6;
            shadow_AmmoText.Y+=6;
        }
        else if(_gun.Grip==TypeGrip.LongGun)
        {
            scaleImage = 2.1f;
            gunPos.X+=16;
            gunPos.Y+=2;
            ammo_TextPos.X-=8;
            ammo_TextPos.Y+=4;
            shadow_AmmoText.X = ammo_TextPos.X;
            shadow_AmmoText.Y+=4;
        }
       
        Raylib.DrawCircleV(new Vector2(_center.X+4, _center.Y), 50f, Raylib.Fade(Color.LightGray, 0.5f));
        Raylib.DrawRectangle((int)ammo_TextPos.X, (int)ammo_TextPos.Y+10, 66, 22, Raylib.Fade(Color.LightGray, 0.2f));
        Raylib.DrawTextureEx(_gun.Icon, gunPos, 0, scaleImage, Color.White);
        Raylib.DrawTextEx(Fonts.Montserrat_SemiBoldItalic, $"{_gun.CurrentAmmo}/{_gun.TotalAmmo}", shadow_AmmoText, fontSize+1f, 0f, Color.Black);
        Raylib.DrawTextEx(Fonts.Montserrat_SemiBoldItalic, $"{_gun.CurrentAmmo}/{_gun.TotalAmmo}", ammo_TextPos, fontSize, 0f, Color.White);
    }
    public void Unload(Player _player)
    {
        UnloadAllTextures(_player.EquippedWeapon);   
        Raylib.UnloadFont(RequiemFont);
    }

    public void UnloadAllTextures(Gun _gun)
    {
        Raylib.UnloadTexture(_gun.Icon);
    }
}