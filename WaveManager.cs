using System;
using System.Collections.Generic;
using Raylib_cs;
namespace ZombieShooter
{
    public enum WaveState
    {
        Intermission,     // esperando a próxima wave começar
        Spawning,         // spawnando zumbis aos poucos
        WaitingForClear   // já spawnou todos, esperando o jogador matar o resto
    }

    // O cérebro do sistema de waves. Funciona como uma máquina de estados:
    //
    //   Intermission -> Spawning -> WaitingForClear -> Intermission -> ...
    //
    // Regras (parecido com COD Zombies):
    // - cada wave tem um número total de zumbis pra spawnar
    // - eles spawnam aos poucos (não tudo de uma vez), num intervalo de tempo
    // - existe um limite de zumbis vivos ao mesmo tempo (MaxConcurrentZombies)
    // - a wave só termina quando TODOS os zumbis foram spawnados E mortos
    public class WaveManager
    {
        
        private readonly List<SpawnPoint> spawnPoints;
        private readonly List<Zombie> activeZombies; // referência à lista do GameManager
        private readonly Random random = new Random();

        public int CurrentWave { get; private set; } = 0;
        public WaveState State { get; private set; } = WaveState.Intermission;

        // Quantos zumbis ainda faltam aparecer no mapa (contando os já vivos)
        public int ZombiesRemainingInWave =>
            (zombiesToSpawnThisWave - zombiesSpawnedThisWave) + activeZombies.Count;

        private int zombiesToSpawnThisWave;
        private int zombiesSpawnedThisWave;

        private float spawnTimer;
        private readonly float spawnInterval = 1.2f;

        private float intermissionTimer = 10f; // tempo antes da wave 1 começar
        private const float IntermissionDuration = 16f;

        private const int MaxConcurrentZombies = 10;
        // Eventos pra você "plugar" som, UI, etc sem acoplar essas coisas aqui dentro
        public event Action<int> OnWaveStart;
        public event Action<int> OnWaveComplete;
        
        
        public WaveManager(List<SpawnPoint> spawnPoints, List<Zombie> activeZombies)
        {
            
            this.spawnPoints = spawnPoints;
            this.activeZombies = activeZombies;
        }

        public void Update(float deltaTime)
        {
            switch (State)
            {
                case WaveState.Intermission:
                    UpdateIntermission(deltaTime);
                        
                    break;
                case WaveState.Spawning:
                    UpdateSpawning(deltaTime);
                    break;
                case WaveState.WaitingForClear:
                    UpdateWaitingForClear();
                    break;
            }
        }

        private void UpdateIntermission(float deltaTime)
        {
            intermissionTimer -= deltaTime;
            if (intermissionTimer <= 0f)
            {
                StartNextWave();
            }
        }

        private void StartNextWave()
        {
            CurrentWave++;
            
            zombiesToSpawnThisWave = CalculateZombiesForWave(CurrentWave);
            zombiesSpawnedThisWave = 0;
            spawnTimer = 0f;
            State = WaveState.Spawning;
            
            
            OnWaveStart?.Invoke(CurrentWave);
            
        }

        private void UpdateSpawning(float deltaTime)
        {
            spawnTimer -= deltaTime;

            bool aindaFaltaSpawnar = zombiesSpawnedThisWave < zombiesToSpawnThisWave;
            bool temEspacoPraSpawnar = activeZombies.Count < MaxConcurrentZombies;

            if (aindaFaltaSpawnar && temEspacoPraSpawnar && spawnTimer <= 0f)
            {
                SpawnZombie();
                spawnTimer = spawnInterval;
            }

            if (!aindaFaltaSpawnar)
            {
                State = WaveState.WaitingForClear;
            }
        }

        private void UpdateWaitingForClear()
        {
            if (activeZombies.Count == 0)
            {
                State = WaveState.Intermission;
                intermissionTimer = IntermissionDuration;
                OnWaveComplete?.Invoke(CurrentWave);
            }
        }

        private void SpawnZombie()
        {
            SpawnPoint escolhido = spawnPoints[random.Next(spawnPoints.Count)];
            Zombie zombie = ZombieFactory.Create(CurrentWave, escolhido.Position);

            activeZombies.Add(zombie);
            zombiesSpawnedThisWave++;
        }

        // Fórmula simples e fácil de ajustar. Troque à vontade pra calibrar a dificuldade.
        private int CalculateZombiesForWave(int wave)
        {
            return Math.Min(6 + 4 * (wave - 1), 30);
        }
    }
}
