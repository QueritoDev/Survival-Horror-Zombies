using System;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using Raylib_cs;
using RayGUI_cs;

namespace Players;

public class Player : Sprite
{
    Texture2D texFrontIdle, texFrontWalking, texRunningFront, 
    texSide, texBackIdle, texBackWalking, texBackRunning;
    int idleFrame = 0;
    int walkFrame = 0;
    int totalFrames = 8;
    int runFrames = 0;
    float frameTimer = 0f;
    float frameDuration = 0.1f;
    const float MAX_STAMINA = 100f;
    const float STAMINA_INIT = 100f;
    const float timerStamina = 50f;
    private float _stamina = STAMINA_INIT;
    const float STAMINA_REGEN_DELAY = 2.1f;
    float staminaRegenTimer = 0f;
    public float Stamina 
    {
        get => _stamina;
        private set => _stamina = Math.Clamp (value, 0f, MAX_STAMINA);
    }
    
    bool HaveStamina = true;
    bool NoStamina = false;
    
    const float SPEED_INIT = 350f;
    const float MAX_SPEED = 450f;
    const float STOPPED = 0f;
    public float Health { get; private set;} = 100f;
    public bool isAlive = true;
    public bool IsWalking {get; private set;} = false;
    public bool IsStopped {get; private set;} = true;
    public bool IsRunning {get; private set;} = false;
    

    public Player(Vector2 posInit) : base (posInit, SPEED_INIT)
    {
        speed = SPEED_INIT;
        texFrontIdle = Raylib.LoadTexture(Path.Combine("sprites", "player_char", "front", "Bowllingguychibi-Idle.png"));
        texFrontWalking = Raylib.LoadTexture(Path.Combine("sprites", "player_char", "front", "Bowllingguychibi-Walk.png"));
        texRunningFront = Raylib.LoadTexture(Path.Combine("sprites", "player_char", "front", "Bowllingguychibi-Run.png"));
        texSide = Raylib.LoadTexture(Path.Combine("sprites", "player_char", "side", "BowllingguychibiLeft-Idle.png"));
        texBackIdle = Raylib.LoadTexture(Path.Combine("sprites", "player_char", "back", "Bowllingguychibiback-Idle.png"));
        texBackWalking = Raylib.LoadTexture(Path.Combine("sprites", "player_char", "back", "Bowllingguychibiback-Walk.png"));
        texBackRunning = Raylib.LoadTexture(Path.Combine("sprites", "player_char", "back", "Bowllingguychibiback-Run.png"));



        Raylib.SetTextureFilter(texFrontIdle, TextureFilter.Point);
        Raylib.SetTextureFilter(texFrontWalking, TextureFilter.Point);
        Raylib.SetTextureFilter(texRunningFront, TextureFilter.Point);
        Raylib.SetTextureFilter(texBackIdle, TextureFilter.Point);
        Raylib.SetTextureFilter(texBackWalking, TextureFilter.Point);
        Raylib.SetTextureFilter(texBackRunning, TextureFilter.Point);
    
    }


    public void Animation(float _dt, ref int _frameWalk)
    {
        frameTimer += _dt;
        if(frameTimer>= frameDuration)
        {
            frameTimer = 0f;
            _frameWalk = (_frameWalk+1) % totalFrames;
        }
    }

    public void Update(InventoryUI inventory)
    {
        isAlive = Health > 0f;
        if(!isAlive) return;
        
        const float ACCELERATION = 800f;
        const float DECELERATION = 1200f;
        float deltaTime = Raylib.GetFrameTime();
        
        direction.X = (Raylib.IsKeyDown(KeyboardKey.D) ? 1:0) - (Raylib.IsKeyDown(KeyboardKey.A) ? 1:0); 
        direction.Y = (Raylib.IsKeyDown(KeyboardKey.S) ? 1:0) - (Raylib.IsKeyDown(KeyboardKey.W) ? 1:0);
        
        if(inventory.InvIsShow)
        {direction=Vector2.Zero;}
        HaveStamina = Stamina>0;
        NoStamina = Stamina<=0;

        IsRunning = HaveStamina && direction!=Vector2.Zero && Raylib.IsKeyDown(KeyboardKey.LeftShift);
        IsWalking = direction!=Vector2.Zero && !IsRunning;
        IsStopped = direction==Vector2.Zero;

        float TargetSpeed = IsRunning ? MAX_SPEED : SPEED_INIT;
        
        if(IsStopped)
            speed = Math.Max(0, speed - DECELERATION * deltaTime);
        else
            speed = Math.Min(TargetSpeed, speed + ACCELERATION * deltaTime);

        if(IsRunning)
        {
            Stamina = Math.Max(0, Stamina - timerStamina * deltaTime); // trava em 0, nunca negativo
            staminaRegenTimer = STAMINA_REGEN_DELAY;
        }
        else
        {
            if(NoStamina && staminaRegenTimer > 0)
            {
                staminaRegenTimer -= deltaTime; // aguardando o delay
            }
            else
            {
                float regenRate = IsWalking ? (timerStamina - 30f) : timerStamina;
                Stamina = Math.Min(MAX_STAMINA, Stamina + regenRate * deltaTime);
            }
        }

        if(IsStopped) {Animation(deltaTime, ref idleFrame);}
        if (direction.Y > 0 && IsWalking) {Animation(deltaTime, ref walkFrame);}
        if (direction.Y < 0 && IsWalking) {Animation(deltaTime, ref walkFrame);}
        if (direction.Y > 0 && IsRunning) {Animation(deltaTime, ref runFrames);}
        if (direction.Y < 0 && IsRunning) {Animation(deltaTime, ref runFrames);}
        
        Move(deltaTime);
    }

    public void Draw()
    {
        if(!isAlive) return;
        
        float frameWidthFront = texFrontIdle.Width / (float)totalFrames;
        float frameHeightFront = texFrontIdle.Height;

        float frameWidthBack = texBackIdle.Width / (float)totalFrames;
        float frameHeightBack = texBackIdle.Height;

        // Animations Front POV
        Rectangle frameRecIdleFront = new Rectangle(idleFrame * frameWidthFront, 0, frameWidthFront, frameHeightFront);
        Rectangle frameRecWalkingFront = new Rectangle(walkFrame * frameWidthFront, 0, frameWidthFront, frameHeightFront);
        Rectangle frameRecRunFront = new Rectangle(runFrames * frameWidthFront, 0, frameWidthFront, frameHeightFront);

        // Animations Back POV
        
        Rectangle frameRecWalkingBack = new Rectangle(walkFrame * frameWidthBack, 0, frameWidthBack, frameHeightBack);
        Rectangle frameRecRunBack = new Rectangle(runFrames * frameWidthBack, 0, frameWidthBack, frameHeightBack);
        
        if(IsStopped){Raylib.DrawTextureRec(texFrontIdle, frameRecIdleFront, pos, Color.White);}
        
        if(IsWalking && direction.Y > 0){Raylib.DrawTextureRec(texFrontWalking, frameRecWalkingFront, pos, Color.White);}
        if(IsWalking && direction.Y < 0){Raylib.DrawTextureRec(texBackWalking, frameRecWalkingBack, pos, Color.White);}
        if(IsRunning && direction.Y > 0){Raylib.DrawTextureRec(texRunningFront, frameRecRunFront, pos, Color.White);}
        if(IsRunning && direction.Y < 0){Raylib.DrawTextureRec(texBackRunning, frameRecRunBack, pos, Color.White);}
        
    }


    public void Unload()
    {
        Raylib.UnloadTexture(texFrontIdle);
        Raylib.UnloadTexture(texFrontWalking);
        Raylib.UnloadTexture(texRunningFront);
        Raylib.UnloadTexture(texBackIdle);
        Raylib.UnloadTexture(texBackWalking);
        Raylib.UnloadTexture(texBackRunning);
        Raylib.UnloadTexture(texSide);
    }
}

