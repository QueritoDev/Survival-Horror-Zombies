using System;
using System.Collections.Generic;
using Players;
using System.Numerics;
using Raylib_cs;

namespace ZombieShooter
{
    // Junta as peças: mantém a lista de zumbis vivos, atualiza o WaveManager
    // e desenha tudo na tela. Esta é a única classe que "conhece" todas as outras.
    public class ManagerGame
    {
        private readonly List<Zombie> activeZombies = new List<Zombie>();
        private readonly List<SpawnPoint> spawnPoints;
        private readonly WaveManager waveManager;

        // TODO: troque isso pela posição real do seu jogador
        private Vector2 playerPosition;

        public ManagerGame()
        {
            spawnPoints = new List<SpawnPoint>
            {
                new SpawnPoint(new Vector2(200, 200)),
                new SpawnPoint(new Vector2(300, 50)),
                new SpawnPoint(new Vector2(50, 670)),
                new SpawnPoint(new Vector2(1230, 670)),
            };

            waveManager = new WaveManager(spawnPoints, activeZombies);
            waveManager.OnWaveStart += wave => Console.WriteLine($"Wave {wave} iniciada!");
            waveManager.OnWaveComplete += wave => Console.WriteLine($"Wave {wave} completa!");
        }

        public void Update(float deltaTime, Player player)
        {
            playerPosition = player.GetPosition();
            waveManager.Update(deltaTime);

            
            foreach (Zombie zombie in activeZombies)
            {
                zombie.Update(deltaTime, playerPosition);
            }
            ResolveZombieCollisions();
            // remove zumbis mortos da lista (é essa lista que o WaveManager observa)
            activeZombies.RemoveAll(z => !z.IsAlive);
        }

            private void ResolveZombieCollisions()
            {
            const float minDistance = Zombie.Radius * 2f;
 
            for (int i = 0; i < activeZombies.Count; i++)
            {
                for (int j = i + 1; j < activeZombies.Count; j++)
                {
                    Zombie a = activeZombies[i];
                    Zombie b = activeZombies[j];
 
                    Vector2 delta = b.Position - a.Position;
                    float distance = delta.Length();
 
                    if (distance >= minDistance) continue;
 
                    // se estiverem exatamente na mesma posição, empurra numa direção fixa
                    Vector2 pushDirection = distance > 0.0001f
                        ? delta / distance
                        : new Vector2(1, 0);
 
                    float overlap = minDistance - distance;
 
                    a.Push(-pushDirection * (overlap * 0.5f));
                    b.Push(pushDirection * (overlap * 0.5f));
                }
            }
        }


        public void Draw()
        {
            foreach (Zombie zombie in activeZombies)
            {
                zombie.Draw();
            }
        }

        public void DrawHUD()
        {
            Raylib.DrawText($"Wave: {waveManager.CurrentWave}", 10, 10, 30, Color.White);
            Raylib.DrawText($"Zombies: {waveManager.ZombiesRemainingInWave}", 10, 35, 30, Color.White);
            Raylib.DrawText($"State: {waveManager.State}", 10, 60, 30, Color.White);
        }
    }
}
