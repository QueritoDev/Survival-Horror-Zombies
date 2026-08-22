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
    protected Sound switchGun_Sound;
    protected Sound UI_WarningMsg;
    protected bool WarningLowAmmo = false;
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
    public Gun[] Arsenal { get; private set; } = new Gun[3];
    public int CurrentSlot { get; private set; } = 0;
    public Gun EquippedWeapon => Arsenal[CurrentSlot];


    public float Stamina 
    {
        get => _stamina;
        private set => _stamina = Math.Clamp (value, 0f, MAX_STAMINA);
    }
    
    bool HaveStamina = true;
    bool NoStamina = false;
    
    const float SPEED_INIT = 300f;
    const float SPEED_WHILE_SHOOTING = 120f;
    const float MAX_SPEED = 400f;
    const float STOPPED = 0f;
    public float Health { get; private set;} = 100f;
    public bool isAlive = true;
    public bool IsWalking {get; private set;} = false;
    public bool IsStopped {get; private set;} = true;
    public bool IsRunning {get; private set;} = false;
    private Vector2 WeaponStartPos;
    private Vector2 WeaponEndPos;
    

    public Player(Vector2 initialPosition) : base (initialPosition, SPEED_INIT)
    {
        speed = SPEED_INIT;
        Arsenal[0] = new MarksmanRifle();   // Slot Primário (ArmaLonga)
        Arsenal[1] = new Pistol(); // Slot Secundário (Pistola)
        
        UI_WarningMsg = Raylib.LoadSound(Path.Combine("audio","sfx","UI_WarningMsg.wav"));
        switchGun_Sound = Raylib.LoadSound(Path.Combine("audio", "sfx", "SwitchWeapons","wpn_hudon.wav"));
        Raylib.SetSoundVolume(switchGun_Sound, 0.8f);
        
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


    public void Update(InventoryUI inventory, Camera2D camera, float angleRad)
    {
        isAlive = Health > 0f;
        if(!isAlive) return;
        float deltaTime = Raylib.GetFrameTime();
        
        if (Raylib.IsKeyPressed(KeyboardKey.One)) SwitchGun(CurrentSlot==0 ? 1:0);
        if (Raylib.IsKeyPressed(KeyboardKey.Two)) SwitchGun(CurrentSlot==1 ? 1:0);
        if (Raylib.IsKeyPressed(KeyboardKey.Three)) SwitchGun(2);
        if (Raylib.IsKeyPressed(KeyboardKey.R) && EquippedWeapon != null) EquippedWeapon.Reload();
        
        WarningLowAmmo = EquippedWeapon !=null && EquippedWeapon.TotalAmmo<5;
        
        direction.X = (Raylib.IsKeyDown(KeyboardKey.D) ? 1:0) - (Raylib.IsKeyDown(KeyboardKey.A) ? 1:0); 
        direction.Y = (Raylib.IsKeyDown(KeyboardKey.S) ? 1:0) - (Raylib.IsKeyDown(KeyboardKey.W) ? 1:0);
        
        if(inventory.InvIsShow) {direction=Vector2.Zero;} // The player must be stopped when open inventory XD
        HaveStamina = Stamina>0;
        NoStamina = Stamina<=0;
        
        IsRunning = HaveStamina && direction!=Vector2.Zero && Raylib.IsKeyDown(KeyboardKey.LeftShift) && !Raylib.IsMouseButtonDown(MouseButton.Left);
        IsWalking = direction!=Vector2.Zero && !IsRunning;
        IsStopped = direction==Vector2.Zero;

        
        MovementPhysics(deltaTime);
        Move(deltaTime);
        
        /* OLD ANIMATIONS
        if(IsStopped) {Animation(deltaTime, ref idleFrame);}
        if (direction.Y > 0 && IsWalking) {Animation(deltaTime, ref walkFrame);}
        if (direction.Y < 0 && IsWalking) {Animation(deltaTime, ref walkFrame);}
        if (direction.Y > 0 && IsRunning) {Animation(deltaTime, ref runFrames);}
        if (direction.Y < 0 && IsRunning) {Animation(deltaTime, ref runFrames);}
        */

        
        Vector2 screenMouse = Raylib.GetMousePosition();
        Vector2 offsetGunBarrel = WeaponEndPos;
        Vector2 worldMouse = Raylib.GetScreenToWorld2D(screenMouse, camera);
        
        
        if (EquippedWeapon != null)
        {
            EquippedWeapon.Update(deltaTime);
            // Shooting System Using ScreenToWorld2D
            if (Raylib.IsMouseButtonDown(MouseButton.Left))
            {
                Vector2 shootingDirection = new Vector2(MathF.Cos(angleRad), MathF.Sin(angleRad));

                EquippedWeapon.Fire(offsetGunBarrel, shootingDirection);
                speed = SPEED_WHILE_SHOOTING;
            }
            
            if(WarningLowAmmo)
            {
                Raylib.PlaySound(UI_WarningMsg);
                WarningLowAmmo = true;
                if(Raylib.IsKeyPressed(KeyboardKey.J))
                {
                    EquippedWeapon.RestoreMaxAmmo();
                }
            }
        }
    }
    private void SwitchGun(int novoSlot)
    {
        // Verifica se existe uma arma cadastrada neste slot (evita crash do jogo)
        if (Arsenal[novoSlot] != null) 
        {
            CurrentSlot = novoSlot;
            Raylib.PlaySound(switchGun_Sound);
        }
    }

    
    /* TO SWITCH FIRE MODE SOON
    private void SwitchFireMode(FireMode _firemode, int slot)
    {
        // Verifica se existe uma arma cadastrada neste slot
        if (Arsenal[slot] != null) 
        {
            _firemode = FireMode.Semi_Auto;
        } 
    }
    */

    public void MovementPhysics(float deltaTime)
    {
        const float ACCELERATION = 800f;
        const float DECELERATION = 1200f;
        float TargetSpeed = IsRunning ? MAX_SPEED : SPEED_INIT;
        if(IsStopped)
            speed = Math.Max(0, speed - DECELERATION * deltaTime); 
        else
            speed = Math.Min(TargetSpeed, speed + ACCELERATION * deltaTime);

        if(IsRunning)
        {
            Stamina = Math.Max(0, Stamina - timerStamina * deltaTime);
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
    }

    public Vector2 GetPosition()
    {
        return this.pos;
    }

    public void TakeDamage(float amount)
    {
        Health = Math.Max(0, Health - amount);
    }

    /* CRASHING
    public int GetTotalAmmo()
    {
        return Arsenal[0].CurrentAmmo + Arsenal[1].CurrentAmmo + Arsenal[2].CurrentAmmo;
    }

    public void DropMaxAmmo()
    {
        if(GetTotalAmmo()<25)
        {
            for(int i = 0; i <= Arsenal.Length; i++)
            {
                Arsenal[i].RestoreMaxAmmo();
            }
        }
    }
    */

    public void Draw(float angleRad)
    {
        if(!isAlive) return;
        
        SkinPlayer(angleRad);
        EquippedWeapon.Draw();

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
        
        Vector2 rightHandOffset = Vector2.Zero;
        Vector2 leftHandOffset = Vector2.Zero;
        Vector2 weaponStartOffset = Vector2.Zero;
        Vector2 weaponEndOffset = Vector2.Zero;
        // Distance between hands
        TypeGrip CurrentGrip = EquippedWeapon != null ? EquippedWeapon.Grip : TypeGrip.Knife;
        
        switch(CurrentGrip)
        {
            case TypeGrip.Pistol:
                weaponStartOffset = new Vector2(0f, 22f);
                weaponEndOffset = new Vector2(0f, 45f);
                rightHandOffset = new Vector2(-6f, 22f);
                leftHandOffset = new Vector2(-6f, 22f);
            break;

            case TypeGrip.LongGun:
                weaponStartOffset = new Vector2(-5f, 15f);
                weaponEndOffset = new Vector2(-5f, 52f);
                rightHandOffset = new Vector2(-8f, 15f);
                leftHandOffset = new Vector2(-10f, 40f);
            break;

            case TypeGrip.Knife:
            default:
                rightHandOffset = new Vector2(-10f, 19f);
                leftHandOffset = new Vector2(10f, 19f);
            break;
        }
        

        float adjustedAngle = angleRad - (MathF.PI / 2f);

        // Sin and Cos of Angle
        float cos = MathF.Cos(adjustedAngle);
        float sin = MathF.Sin(adjustedAngle);
        
        // Rotate the displacement points around the center (0,0)
        Vector2 RotateOffset(Vector2 offset)
        {
        return new Vector2(
            offset.X * cos - offset.Y * sin,
            offset.X * sin + offset.Y * cos
        );
        }
       
        Vector2 RightHandPos = pos + RotateOffset(rightHandOffset);
        Vector2 LeftHandPos = pos + RotateOffset(leftHandOffset);
        WeaponStartPos = pos + RotateOffset(weaponStartOffset);
        WeaponEndPos = pos + RotateOffset(weaponEndOffset);
        
        DrawGunOnHand();

        // Draw the hands in the newly recalculated positions
        Raylib.DrawRing(RightHandPos, 4f, 7.2f, 0f, 360f, 16, Color.Black); // Shadow Right-Hand
        Raylib.DrawCircleV(RightHandPos, 5f, Color.Beige); // Right Hand

        Raylib.DrawRing(LeftHandPos, 4f, 7.2f, 0f, 360f, 16, Color.Black); // Shadow Left-Hand
        Raylib.DrawCircleV(LeftHandPos, 5f, Color.Beige); // Left Hand
    }   

    
    public void DrawGunOnHand()
    {
        if (EquippedWeapon != null)
        {
        // Linha preta mais grossa por baixo (Cria o efeito de borda/stroke igual ao do boneco)
        Raylib.DrawLineEx(WeaponStartPos, WeaponEndPos, 6f, Color.Black);
        
        // Linha cinza ligeiramente mais fina por cima (Corpo real da arma)
        Raylib.DrawLineEx(WeaponStartPos, WeaponEndPos, 2f, Color.DarkGray);
        }
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

