using System.Numerics;
using Raylib_cs;

public class Projetil
{
    
    public Vector2 Posicao { get; private set; }
    public Vector2 Velocidade { get; private set; }
    public float Raio { get; private set; } = 4f;
    public bool Ativo { get; set; } = true;

    public Projetil(Vector2 posicaoInicial, Vector2 direcao, float velocidade)
    {
        Posicao = posicaoInicial;
        // Multiplica o vetor direção (normalizado) pela velocidade
        Velocidade = direcao * velocidade;
    }

    public void Update(float deltaTime)
    {
        Posicao += Velocidade * deltaTime;
    }

    public void Draw()
    {
        if (Ativo)
        {
            Raylib.DrawCircleV(Posicao, Raio, Color.Yellow);
        }
    }
}