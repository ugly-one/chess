using System;

namespace Chess;

public record struct Vector(int X, int Y)
{
    public static Vector operator +(Vector a, Vector b)
        => new Vector(a.X + b.X, a.Y + b.Y);

    public static Vector operator *(Vector a, int scalar)
        => new Vector(a.X * scalar, a.Y * scalar);

    public static Vector operator -(Vector a, Vector b)
        => new Vector(a.X - b.X, a.Y - b.Y);

    public static Vector Up = new Vector(0, -1);
    public static Vector Down = new Vector(0, 1);
    public static Vector Left = new Vector(-1, 0);
    public static Vector Right = new Vector(1, 0);

    public Vector Abs() 
        => new Vector(Math.Abs(X), Math.Abs(Y));

    public Vector Clamp(Vector min, Vector max) 
        => new Vector(Math.Clamp(X, min.X, max.X), Math.Clamp(Y, min.Y, max.Y));

    public Vector Orthogonal()
        => new Vector(Y, 0 - X);
}
