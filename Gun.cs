using System.Numerics;
using System.Collections.Generic;
using Raylib_cs;

public class Pistol
{
    public List<Projetil> Projeteis { get; private set; }
    
    private float taxaDeDisparo = 0.2f; // Tempo em segundos entre cada tiro
    private float timerRecarga = 0f;
    private float velocidadeBala = 600f;
    Sound pistol_fire = Raylib.LoadSound(Path.Combine("audio", "sfx", "pistol", "pistol_fire.wav"));
    public Pistol()
    {
        Projeteis = new List<Projetil>();
        Raylib.SetSoundVolume(pistol_fire, 0.4f);
    }

    public void Update(float deltaTime)
    {
        // Reduz o tempo de espera para o próximo tiro
        if (timerRecarga > 0) timerRecarga -= deltaTime;

        // Atualiza os tiros e remove os que já saíram da tela
        for (int i = Projeteis.Count - 1; i >= 0; i--)
        {
            Projeteis[i].Update(deltaTime);
            if (!Projeteis[i].Ativo)
            {
                Projeteis.RemoveAt(i);
            }
        }
    }

    public void Atirar(Vector2 origem, Vector2 posicaoAlvo)
    {
        // Só atira se o tempo de recarga (cooldown) tiver zerado
        if (timerRecarga <= 0)
        {
            // Calcula a direção do tiro (do jogador para o mouse)
            Vector2 direcao = Vector2.Normalize(posicaoAlvo - origem);
            
            Projeteis.Add(new Projetil(origem, direcao, velocidadeBala));
            Raylib.PlaySound(pistol_fire);
            // Reseta o tempo de recarga
            timerRecarga = taxaDeDisparo;
        }
    }

    public void Draw()
    {
        foreach (var projetil in Projeteis)
        {
            projetil.Draw();
        }
    }
}