using System.Numerics;

namespace ZombieShooter
{
    // Representa um ponto onde zumbis podem nascer no mapa.
    // Simples de propósito: se depois você quiser spawn points que
    // "esfriam" depois de usados, ou que só ativam em certas waves,
    // dá pra estender essa classe sem mexer no resto do sistema.
    public class SpawnPoint
    {
        public Vector2 Position { get; }

        public SpawnPoint(Vector2 position)
        {
            Position = position;
        }
    }
}
