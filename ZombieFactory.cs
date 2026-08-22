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

            float speed = 170f + (wave -1) * 25;
            health = Math.Min(health, 900f); // teto pra não ficar impossível nas waves altas
            speed = Math.Min(speed, 500f);
            return new Zombie(spawnPosition, health, speed);
        }
    }
}
