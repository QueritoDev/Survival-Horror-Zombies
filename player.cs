using System;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using Raylib_cs;
using RayGUI_cs;
using ZombieShooter;

namespace Players;

public class Player : Sprite
{
    Texture2D texFrontIdle, texFrontWalking, texRunningFront, 
    
    texSide, texBackIdle, texBackWalking, texBackRunning;
    float angleDeg = 0f;
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
    public Pistol Gun { get; private set; }
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
    
    

    public Player(Vector2 initialPosition) : base (initialPosition, SPEED_INIT)
    {
        speed = SPEED_INIT;
        Gun = new Pistol();
        RenderTexture2D playerTexture = Raylib.LoadRenderTexture(64,64);
        /*OLD TEXTURES
        texFrontIdle = Raylib.LoadTexture(Path.Combine("sprites", "player_char", "front", "Bowllingguychibi-Idle.png"));
        texFrontWalking = Raylib.LoadTexture(Path.Combine("sprites", "player_char", "front", "Bowllingguychibi-Walk.png"));
        texRunningFront = Raylib.LoadTexture(Path.Combine("sprites", "player_char", "front", "Bowllingguychibi-Run.png"));
        texSide = Raylib.LoadTexture(Path.Combine("sprites", "player_char", "side", "BowllingguychibiLeft-Idle.png"));
        texBackIdle = Raylib.LoadTexture(Path.Combine("sprites", "player_char", "UP", "Bowllingguychibiback-Idle.png"));
        texBackWalking = Raylib.LoadTexture(Path.Combine("sprites", "player_char", "UP", "Bowllingguychibiback-Walk.png"));
        texBackRunning = Raylib.LoadTexture(Path.Combine("sprites", "player_char", "UP", "Bowllingguychibiback-Run.png"));

        Raylib.SetTextureFilter(texFrontIdle, TextureFilter.Point);
        Raylib.SetTextureFilter(texFrontWalking, TextureFilter.Point);
        Raylib.SetTextureFilter(texRunningFront, TextureFilter.Point);
        Raylib.SetTextureFilter(texBackIdle, TextureFilter.Point);
        Raylib.SetTextureFilter(texBackWalking, TextureFilter.Point);
        Raylib.SetTextureFilter(texBackRunning, TextureFilter.Point);
        */
    }

    /*
    public void Animation(float _dt, ref int _frameWalk)
    {
        frameTimer += _dt;
        if(frameTimer>= frameDuration)
        {
            frameTimer = 0f;
            _frameWalk = (_frameWalk+1) % totalFrames;
        }
    }
    */


    public void Update(InventoryUI inventory, Camera2D camera)
    {
        isAlive = Health > 0f;
        if(!isAlive) return;
        
        if(Raylib.IsKeyPressed(KeyboardKey.F)) // For testing purposes only
            Health -= 2f;
        
        
        const float ACCELERATION = 800f;
        const float DECELERATION = 1200f;
        float deltaTime = Raylib.GetFrameTime();
        
        direction.X = (Raylib.IsKeyDown(KeyboardKey.D) ? 1:0) - (Raylib.IsKeyDown(KeyboardKey.A) ? 1:0); 
        direction.Y = (Raylib.IsKeyDown(KeyboardKey.S) ? 1:0) - (Raylib.IsKeyDown(KeyboardKey.W) ? 1:0);
        
        if(inventory.InvIsShow) {direction=Vector2.Zero;} // The player must be stopped when open inventory XD
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


        /* OLD ANIMATIONS
        if(IsStopped) {Animation(deltaTime, ref idleFrame);}
        if (direction.Y > 0 && IsWalking) {Animation(deltaTime, ref walkFrame);}
        if (direction.Y < 0 && IsWalking) {Animation(deltaTime, ref walkFrame);}
        if (direction.Y > 0 && IsRunning) {Animation(deltaTime, ref runFrames);}
        if (direction.Y < 0 && IsRunning) {Animation(deltaTime, ref runFrames);}
        */

        Move(deltaTime);
        Vector2 screenMouse = Raylib.GetMousePosition();
        Vector2 offsetGunBarrel= new Vector2(10f, -5f);
        Vector2 originFire = GetPosition() + offsetGunBarrel;
        Vector2 worldMouse = Raylib.GetScreenToWorld2D(screenMouse, camera);
      
        if (Raylib.IsMouseButtonPressed(MouseButton.Left))
        {
            Gun.Atirar(originFire, worldMouse);
            
        }

        Gun.Update(deltaTime);
        
    }

    public Vector2 GetPosition()
    {
        return this.pos;
    }

    public void Draw(float angleRad)
    {
        if(!isAlive) return;
        
        SkinPlayer(angleRad);
        Gun.Draw();
        /* OLD ANIMATIONS (sprites)
        float frameWidthFront = texFrontIdle.Width / (float)totalFrames;
        float frameHeightFront = texFrontIdle.Height;

        float frameWidthBack = texBackIdle.Width / (float)totalFrames;
        float frameHeightBack = texBackIdle.Height;
        
        Animations Moving Down POV
        Rectangle frameRecIdleFront = new Rectangle(idleFrame * frameWidthFront, 0, frameWidthFront, frameHeightFront);
        Rectangle frameRecWalkingFront = new Rectangle(walkFrame * frameWidthFront, 0, frameWidthFront, frameHeightFront);
        Rectangle frameRecRunFront = new Rectangle(runFrames * frameWidthFront, 0, frameWidthFront, frameHeightFront);

        Animations Moving UP POV
        #FIXME (When the player switches to the backward-facing texture, the texture shifts slightly to the bottom-right corner.
        It's just a visual problem, so if you have a better texture to player, switch it :)
        
        Rectangle frameRecWalkingBack = new Rectangle(walkFrame * frameWidthBack, 0, frameWidthBack, frameHeightBack);
        Rectangle frameRecRunBack = new Rectangle(runFrames * frameWidthBack, 0, frameWidthBack, frameHeightBack);
        
        if(IsStopped){Raylib.DrawTextureRec(texFrontIdle, frameRecIdleFront, pos, Color.White);}
        
        if(IsWalking && direction.Y > 0){Raylib.DrawTextureRec(texFrontWalking, frameRecWalkingFront, pos, Color.White);}
        if(IsWalking && direction.Y < 0){Raylib.DrawTextureRec(texBackWalking, frameRecWalkingBack, pos, Color.White);}
        if(IsRunning && direction.Y > 0){Raylib.DrawTextureRec(texRunningFront, frameRecRunFront, pos, Color.White);}
        if(IsRunning && direction.Y < 0){Raylib.DrawTextureRec(texBackRunning, frameRecRunBack, pos, Color.White);}
        */
       
    }

 
    public void SkinPlayer(float angleRad)
    {
        
        //Body player
        Raylib.DrawCircleV(pos, 26f, Raylib.Fade(Color.Black, 0.4f)); //Shadow player
        Raylib.DrawCircleV(pos, 24f, Color.Black); // Stroke effect
        Raylib.DrawCircleV(pos, 22f, Color.Beige); // Skin player

        // Distance between hands
        Vector2 rightOffset = new Vector2(-10f, 19f);
        Vector2 leftOffset = new Vector2(10f, 19f);

        float adjustedAngle = angleRad - (MathF.PI / 2f);

        // Sin and Cos of Angle
        float cos = MathF.Cos(adjustedAngle);
        float sin = MathF.Sin(adjustedAngle);

        // Rotate the displacement points around the center (0,0)
        Vector2 rotatedRight = new Vector2(
            rightOffset.X * cos - rightOffset.Y * sin,
            rightOffset.X * sin + rightOffset.Y * cos
        );

        Vector2 rotatedLeft = new Vector2(
            leftOffset.X * cos - leftOffset.Y * sin,
            leftOffset.X * sin + leftOffset.Y * cos
        );

        // 5. Adds the player's current position to obtain the actual world coordinates.
        Vector2 RightHandPos = pos + rotatedRight;
        Vector2 LeftHandPos = pos + rotatedLeft;

        // Draw the hands in the newly recalculated positions
        
        Raylib.DrawRing(RightHandPos, 5f, 8f, 0f, 360f, 16, Color.Black); // Shadow Right-Hand
        Raylib.DrawCircleV(RightHandPos, 6f, Color.Beige); // Right Hand

        Raylib.DrawRing(LeftHandPos, 5f, 8f, 0f, 360f, 16, Color.Black); // Shadow Left-Hand
        Raylib.DrawCircleV(LeftHandPos, 6f, Color.Beige); // Left Hand
    }   


    public void UnloadEverything()
    {
        UnloadTextures();
    }

    void UnloadTextures()
    {
        /* OLD ANIMATIONS
        Raylib.UnloadTexture(texFrontIdle);
        Raylib.UnloadTexture(texFrontWalking);
        Raylib.UnloadTexture(texRunningFront);
        Raylib.UnloadTexture(texBackIdle);
        Raylib.UnloadTexture(texBackWalking);
        Raylib.UnloadTexture(texBackRunning);
        Raylib.UnloadTexture(texSide);
        */
    }

    void UnloadSound()
    {
        
    }
}

