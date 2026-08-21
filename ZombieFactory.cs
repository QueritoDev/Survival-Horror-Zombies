using System;
using System.Numerics;

namespace ZombieShooter
{
    // Centraliza a criação de zumbis e o balanceamento por wave.
    // Vantagem de isolar isso aqui: se um dia você quiser zumbis especiais
    // (corredor, tanque, cuspidor), basta trocar essa lógica sem tocar
    // no WaveManager nem na classe Zombie.
    public static class ZombieFactory
    {
        public static Zombie Create(int wave, Vector2 spawnPosition)
        {
            float health = 100 + (wave - 1) * 25;

            float speed = 120f;
            health = Math.Min(health, 900f); // teto pra não ficar impossível nas waves altas

            return new Zombie(spawnPosition, health, speed);
        }
    }
}
