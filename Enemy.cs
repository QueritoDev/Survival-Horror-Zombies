using Raylib_cs;
enum EnemyState {Idle, Chase, Attack, Dead}

class Enemy
{
    public EnemyState State { get; private set; }

    public void Update()
    {
        float dt = Raylib.GetFrameTime();
        switch (State)
        {
            case EnemyState.Idle: UpdateIdle(dt); break;
            case EnemyState.Chase: UpdateChase(dt); break;
            case EnemyState.Attack: UpdateAttack(dt); break;
            case EnemyState.Dead: UpdateDead(dt); break;
        }
    }

    void UpdateIdle(float dt)
    {

    }

    void UpdateChase(float dt)
    {

    }

    void UpdateAttack(float dt)
    {

    }

    void UpdateDead(float dt)
    {

    }
}