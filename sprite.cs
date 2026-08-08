using System;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using Raylib_cs;

public class Sprite
{
    public Vector2 pos;
    public Vector2 direction;
    public float speed;

    public Sprite (Vector2 initialPosition, float speedInit)
    {
        pos = initialPosition;
        speed = speedInit;
        direction = new Vector2(0,0);

    }

    public void Move(float _deltaTime)
    {
        if(direction!=Vector2.Zero)
        {
            direction = Vector2.Normalize(direction);

            pos.X += direction.X * speed * _deltaTime;
            pos.Y += direction.Y * speed * _deltaTime;
        }
    }
}