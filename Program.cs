using System.Numerics;
using Players;
using Raylib_cs;
using ImGuiNET;
using rlImGui_cs;


public static class Program
{
    // STAThread is required if you deploy using NativeAOT on Windows
    // See https://github.com/raylib-cs/raylib-cs/issues/301
    public static bool isShowDebug = false;
    [System.STAThread]
    public static void Main()
    {
        
        Raylib.InitWindow(1280, 720, "Survival Horror - Learning");
        Raylib.InitAudioDevice();
        Raylib.SetTargetFPS(100);
        Raylib.SetExitKey(KeyboardKey.Null);
        rlImGui.Setup();
        
        
        HUD _Hud = new HUD();
        Camera2D cam = new Camera2D();
        Player rbz = new Player(new Vector2(500,500));
        Inventory inventory = new Inventory();
        InventoryUI inventoryUI = new InventoryUI(ref inventory);
        cam.Zoom = 1.7f;
        cam.Target = (rbz.pos);
        cam.Offset = new Vector2(640,360);
        cam.Rotation = 0;
        
        while (!Raylib.WindowShouldClose())
        {
            GameManager.Input();
            MainMenu.Input();
            PausedMenu.Input();
            float dt = Raylib.GetFrameTime();
            
            Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.White);
                Raylib.BeginMode2D(cam);
                    rbz.Draw();
                Raylib.DrawText("Hello, world!", 400, 400, 20, Color.Black);
                Raylib.EndMode2D();
                
                _Hud.DrawStaminaBAR(100f, rbz.Stamina);
                _Hud.DrawRadialGauge(new Vector2(1160,630), rbz.Health, 100f);
                inventoryUI.Draw();
            switch(GameManager.CurrentState) 
            {
            case GameState.Playing:
                inventoryUI.Update(dt);
                inventoryUI.Input();
                rbz.Update(inventoryUI);
                cam.Target = (rbz.pos);
                if(Raylib.IsKeyPressed(KeyboardKey.F5))
                    isShowDebug = !isShowDebug;
            break;
            } 
            /* Não é necessário inserir outros estados aqui, pois cada um deles estão configurado em seus próprios arquivios - Nos menus, por exemplo, sempre terá a condição
            "(Se o CurrentState NÃO É IGUAL(!=) ao X_Menu) return;"
            */
            
            PausedMenu.Draw();
            MainMenu.Draw();
            OptionsMenu.Draw();
            
            rlImGui.Begin();
                DebugGame.OpenWindow();
                DebugGame.PlayerSection(rbz.speed, rbz.Health, rbz.Stamina, rbz.IsStopped, rbz.IsWalking, rbz.IsRunning, rbz.pos);
                DebugGame.InputSection(rbz.direction);
                ImGui.Text($"Current GameState: {GameManager.CurrentState}");
                DebugGame.CloseWindow();
            rlImGui.End();
            Raylib.EndDrawing();
        }
        rbz.Unload();
        inventoryUI.Unload();
        _Hud.Unload();
        rlImGui.Shutdown();
        Raylib.CloseWindow();
    }
}