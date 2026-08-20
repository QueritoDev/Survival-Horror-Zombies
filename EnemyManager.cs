using System.Numerics;
using Raylib_cs;

public class EnemyManager
{
    List<Enemy> enemies = new List<Enemy>();
    private const int MAX_ENEMIES = 15;
    private const int MAX_RENDERED_ENEMIES = 8;
    

   public void Spawn(Vector2 pos)
    {
        foreach(var enemy in enemies)
        {
            if(!enemy.IsAlive)
            {
                enemy.Activate(pos);
                return;
            }
        }

        if(enemies.Count>= MAX_ENEMIES)
            return;
        
        Enemy newEnemy = new Enemy(pos);
        enemies.Add(newEnemy);
    }

    public void Update(float dt, Vector2 playerPos)
    {
        foreach(var enemy in enemies)
            enemy.Update(dt, playerPos);
    }

    public void Draw()
    {
        foreach(var enemy in enemies)
            enemy.Draw();
    }

    bool TryDamage(IDamageable target, Rectangle targetRec, Rectangle attackRec, float amount)
    {
        if(Raylib.CheckCollisionRecs(attackRec, targetRec))
        {
            target.TakeDamage(amount);
            return true;
        }
        return false;
    }

    public void UnloadAll()
    {
        foreach(var enemy in enemies)
            enemy.Unload();
    }
}
