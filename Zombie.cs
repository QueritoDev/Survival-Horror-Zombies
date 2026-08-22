using System;
using System.Numerics;
using Players;
using Raylib_cs;

namespace ZombieShooter
{
    // Entidade zumbi: só sabe se mover em direção a um alvo e levar dano.
    // Não sabe nada sobre waves ou spawn - isso é responsabilidade de outras classes.
    public class Zombie
    {
        public const float Radius = 20f;

        public Vector2 Position { get; private set; }
        public float Speed { get; }
        public float Health { get; private set; }
        public float Damage { get; private set; }
        public bool IsAlive => Health > 0;

        public Zombie(Vector2 spawnPosition, float health, float speed)
        {
            Position = spawnPosition;
            Health = health;
            Speed = speed;
            Damage = 35;
        }

        public void Update(float deltaTime, Vector2 targetPosition)
        {
            if (!IsAlive) return;

            Vector2 toTarget = targetPosition - Position;
            float distance = toTarget.Length();

            if (distance > 0.001f)
            {
                Vector2 direction = toTarget / distance;
                Position += direction * Speed * deltaTime;
            }
        }

        public void TakeDamage(int amount)
        {
            Health = Math.Max(0, Health - amount);
        }

        public float Attack(float dt)
        {
            return Damage * dt;
        }

        public void Push(Vector2 offset)
        {
            Position += offset;
        }

        public void Draw()
        {
            if (!IsAlive) return;
            Raylib.DrawCircleV(Position, Radius, Color.Green);
        }
    }
}
