using System.Numerics;
using Raylib_cs;

public class Projetil
{
    
    public Vector2 Position { get; private set; }
    public Vector2 Direction { get; private set; }
    public float Speed { get; private set; } = 1200f;
    public float Radius { get; private set; } = 4f;
    public bool Active { get; set; } = true;

    private float lifeSpan = 4f;

    public Projetil(Vector2 initialPosition, Vector2 direction)
    {
        Position = initialPosition;
        Direction = direction;
    }

    public void Update(float deltaTime)
    {
        if(!Active) return;

        Position += Direction * Speed * deltaTime;

    }

    public void Draw()
    {
        if (!Active) return;
        Raylib.DrawCircleV(Position, Radius, Color.Yellow);
    }
}