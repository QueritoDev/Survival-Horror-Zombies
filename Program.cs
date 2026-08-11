using System.Numerics;
using Players;
using Raylib_cs;
using ImGuiNET;
using rlImGui_cs;
using System.Reflection.Metadata;


public static class Program
{
    // STAThread is required if you deploy using NativeAOT on Windows
    // See https://github.com/raylib-cs/raylib-cs/issues/301
    public static bool isShowDebug = false;
    [System.STAThread]
    public static void Main()
    {
        const int screenWidth = 1280;
        const int screenHeight = 720;
        

        Raylib.InitWindow(screenWidth, screenHeight, "Survival Horror - Learning");
        Raylib.InitAudioDevice();
        Raylib.SetTargetFPS(100);
        Raylib.SetExitKey(KeyboardKey.Null);
        rlImGui.Setup();
        
        HUD _Hud = new HUD();
        Player rbz = new Player(new Vector2(500,500));
        Inventory inventory = new Inventory();
        InventoryUI inventoryUI = new InventoryUI(ref inventory);
        EnemyManager enemies = new EnemyManager();
        enemies.Spawn(new Vector2(800, 100));

        Camera2D cam = new Camera2D();
        cam.Offset = new Vector2(screenWidth/2.0f, screenHeight/2.0f);
        cam.Target = (rbz.pos);
        cam.Rotation = 0.0f;
        cam.Zoom = 1.7f;
        
        ShaderEffect lightShader = new ShaderEffect(Path.Combine("shaders", "light.fs"));
        
        RenderTexture2D canvas = Raylib.LoadRenderTexture(1280,720);
        

        while (!Raylib.WindowShouldClose())
        {
           
            GameManager.Input();
            MainMenu.Input();
            PausedMenu.Input();
            float dt = Raylib.GetFrameTime();
            
            switch(GameManager.CurrentState) 
            {
            case GameState.Playing:
                inventoryUI.Update(dt);
                inventoryUI.Input();
                rbz.Update(inventoryUI);
                enemies.Update(dt, rbz.pos);
                cam.Target = (rbz.pos);
                if(Raylib.IsKeyPressed(KeyboardKey.F5))
                    isShowDebug = !isShowDebug;
            break;
            } 
            /* Não é necessário inserir outros estados aqui, pois cada um deles estão configurado em seus próprios arquivios - Nos menus, por exemplo, sempre terá a condição
            "(Se o CurrentState NÃO É IGUAL(!=) ao X_Menu) return;"
            */
            
            bool showWorld = GameManager.CurrentState != GameState.MainMenu;

            if(showWorld)
            {
                Vector2 playerScreenPos = Raylib.GetWorldToScreen2D(rbz.pos, cam);
                playerScreenPos.Y = 720 - playerScreenPos.Y;
                lightShader.SetVector2("playerScreenPos", playerScreenPos);
                lightShader.SetFloat("lightRadius", 620f);
                
                Raylib.BeginTextureMode(canvas);
                    Raylib.ClearBackground(Color.White);
                    Raylib.BeginMode2D(cam);
                        rbz.Draw();
                        enemies.Draw();
                    Raylib.DrawText("Hello, world!", 400, 400, 20, Color.Black);
                    Raylib.EndMode2D();
                Raylib.EndTextureMode();
            }
         
            Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.White);
                    
                if(showWorld)
                {
                    lightShader.Begin();
                        Raylib.DrawTextureRec(canvas.Texture, 
                        new Rectangle(0,0, canvas.Texture.Width, -canvas.Texture.Height), 
                        Vector2.Zero, Color.White);
                    lightShader.End();

                    _Hud.DrawStaminaBAR(100f, rbz.Stamina);
                    _Hud.DrawRadialGauge(new Vector2(1160,630), rbz.Health, 100f);
                    inventoryUI.Draw();
                }
                
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
        enemies.UnloadAll();
        lightShader.Unload();
        Raylib.UnloadRenderTexture(canvas);
        inventoryUI.Unload();
        _Hud.Unload();
        rlImGui.Shutdown();
        Raylib.CloseWindow();
    }
}