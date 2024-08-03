using System;

namespace Chess;

public readonly struct Vector(int x, int y)
{
    public readonly int X = x;

    public readonly int Y = y;

    public override bool Equals(object? obj)
        => obj is Vector other && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(X, Y);

    public bool Equals(Vector otherSquare)
        => X == otherSquare.X && Y == otherSquare.Y;

    public static bool operator ==(Vector a, Vector b)
        => a.Equals(b);

    public static bool operator !=(Vector a, Vector b)
        => !a.Equals(b);

    public static Vector operator +(Vector a, Vector b)
        => new Vector(a.X + b.X, a.Y + b.Y);

    public static Vector operator *(Vector a, int scalar)
        => new Vector(a.X * scalar, a.Y * scalar);

    public static Vector operator -(Vector a, Vector b)
        => new Vector(a.X - b.X, a.Y - b.Y);

    public static Vector Up => new Vector(0, -1);
    public static Vector Down => new Vector(0, 1);
    public static Vector Left => new Vector(-1, 0);
    public static Vector Right => new Vector(1, 0);

    public Vector Abs()
        => new Vector(Math.Abs(X), Math.Abs(Y));

    public Vector Clamp(Vector min, Vector max)
        => new Vector(Math.Clamp(X, min.X, max.X), Math.Clamp(Y, min.Y, max.Y));

    public Vector Orthogonal()
        => new Vector(Y, 0 - X);
}
