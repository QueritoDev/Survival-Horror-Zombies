using System.Numerics;
using Raylib_cs;

public interface IDamageable
{
    void TakeDamage(float amount);
}

public class Enemy : Sprite, IDamageable
{
    Texture2D texIdle_Down, texIdle_Up, texIdle_Side, 
    texWalk_Down, texWalk_Up, texWalk_Side;
    public float health = 100f;
    public bool IsAlive = true;
    const float SPEED_INIT = 100f;
    int idleFrame = 0;
    int walkFrame = 0;
    int totalFrames = 6;
    float frameTimer = 0f;
    float frameDuration = 0.1f;
    public bool IsStopped {get; private set;} = true;
    public bool IsWalking {get; private set;} = false;

    float retargetTimer = 0f;
     const float RETARGET_INTERVAL = 0.5f;

    public Enemy(Vector2 InitialPosition) : base (InitialPosition, SPEED_INIT)
    {
        texIdle_Down = Raylib.LoadTexture(Path.Combine("sprites","Enemies","Zombie_Small", "Idle","Zombie_Small_Down_Idle-Sheet6.png"));
        texIdle_Up = Raylib.LoadTexture(Path.Combine("sprites","Enemies","Zombie_Small", "Idle", "Zombie_Small_Up_Idle-Sheet6.png"));
        texIdle_Side = Raylib.LoadTexture(Path.Combine("sprites","Enemies","Zombie_Small", "Idle", "Zombie_Small_Side_Idle-Sheet6.png"));

        texWalk_Down = Raylib.LoadTexture(Path.Combine("sprites","Enemies","Zombie_Small", "Walk","Zombie_Small_Down_walk-Sheet6.png"));
        texWalk_Up = Raylib.LoadTexture(Path.Combine("sprites","Enemies","Zombie_Small", "Walk","Zombie_Small_Up_Walk-Sheet6.png")); 
        texWalk_Side = Raylib.LoadTexture(Path.Combine("sprites","Enemies","Zombie_Small", "Walk","Zombie_Small_Side_Walk-Sheet6.png")); 
    }

    public void Activate (Vector2 position)
    {
        pos = position;
        health = 100f;
        IsAlive = true;
    }

    public void Deactivate ()
    {
        IsAlive = false;
        pos = new Vector2(-999, -999);
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

    public void Update(float dt, Vector2 playerPosition)
    {
        if(!IsAlive) return;

        retargetTimer-=dt;

        if(retargetTimer<=0f)
        {
            RetargetTowards(playerPosition);
            retargetTimer = RETARGET_INTERVAL;
        }

        IsWalking = direction!=Vector2.Zero;
        IsStopped = direction==Vector2.Zero;

        if(IsStopped) {Animation(dt, ref idleFrame);}
        if (direction.Y > 0 && IsWalking) {Animation(dt, ref walkFrame);}
        if (direction.Y < 0 && IsWalking) {Animation(dt, ref walkFrame);}

        Move(dt);
    }

    void RetargetTowards(Vector2 target)
    {
        Vector2 diff = target - pos;
        if(diff != Vector2.Zero)
            direction = Vector2.Normalize(diff);
    }

    public void TakeDamage(float amount)
    {
        health -=amount;
        if(health<=0)
            Deactivate();
    }

    public Rectangle GetRec()
    {
        return new Rectangle(pos.X, pos.Y, texIdle_Down.Width, texIdle_Down.Height);
    }

    public void Draw()
    {
        if(!IsAlive) return;
        
        float frameWidthDown = texIdle_Down.Width / (float)totalFrames;
        float frameHeightDown = texIdle_Down.Height;

        float frameWidthUp = texIdle_Up.Width / (float)totalFrames;
        float frameHeightUp = texIdle_Up.Height;

        float frameWidthSide = texIdle_Side.Width / (float)totalFrames;
        float frameHeightSide = texIdle_Side.Height;

        Rectangle frameRecIdleDown = new Rectangle(idleFrame * frameWidthDown, 0, frameWidthDown, frameHeightDown);
        Rectangle frameRecIdleUp = new Rectangle(idleFrame * frameWidthUp, 0, frameWidthUp, frameHeightUp);
        Rectangle frameRecIdleSide = new Rectangle(idleFrame * frameWidthSide, 0, frameWidthSide, frameHeightSide);

        Rectangle frameRecWalkingDown = new Rectangle(walkFrame * frameWidthDown, 0, frameWidthDown, frameHeightDown);
        Rectangle frameRecWalkingUp = new Rectangle(walkFrame * frameWidthUp, 0, frameWidthUp, frameHeightUp);
        Rectangle frameRecWalkingSide = new Rectangle(walkFrame * frameWidthSide, 0, frameWidthSide, frameHeightSide);

        if(IsStopped){Raylib.DrawTextureRec(texIdle_Down, frameRecIdleDown, pos, Color.White);}
        
        if(IsWalking && direction.X > 0){Raylib.DrawTextureRec(texWalk_Side, frameRecWalkingSide, pos, Color.White);}
        if(IsWalking && direction.X < 0){Raylib.DrawTextureRec(texWalk_Side, frameRecWalkingSide, pos, Color.White);}
        if(IsWalking && direction.Y > 0){Raylib.DrawTextureRec(texWalk_Down, frameRecWalkingDown, pos, Color.White);}
        if(IsWalking && direction.Y < 0){Raylib.DrawTextureRec(texWalk_Up, frameRecWalkingUp, pos, Color.White);}
      
    }

    public void Unload()
    {
        Raylib.UnloadTexture(texIdle_Down);
        Raylib.UnloadTexture(texIdle_Side);
        Raylib.UnloadTexture(texIdle_Up);

        Raylib.UnloadTexture(texWalk_Down);
        Raylib.UnloadTexture(texWalk_Side);
        Raylib.UnloadTexture(texWalk_Up);
    }
}