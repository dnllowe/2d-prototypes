[System.Serializable]
public class Position : Component
{
    public float X;
    public float Y;
    public float Z;

    public Position() : base(Constants.NullEntityId) {}

    public Position(uint entityId) : base(entityId)
    {
        PositionRegistry.Components.Register(this, entityId);
    }

    public Position(float x, float y, float z) : base(Constants.NullEntityId)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public void Register(uint entityId)
    {
        PositionRegistry.Components.Register(this, entityId);
    }

    public void MoveBy(Position position)
    {
        X += position.X;
        Y += position.Y;
        Z += position.Z;
    }

    public void MoveBy(float x, float y, float z)
    {
        X += x;
        Y += y;
        Z += z;
    }

    public void Add(Position position)
    {
        X += position.X;
        Y += position.Y;
        Z += position.Z;
    }

    public void Subtract(Position position)
    {
        X -= position.X;
        Y -= position.Y;
        Z -= position.Z;
    }

    public void Multiply(Position position)
    {
        X *= position.X;
        Y *= position.Y;
        Z *= position.Z;
    }

    public void Multiply(float factor)
    {
        X *= factor;
        Y *= factor;
        Z *= factor;
    }

    public void Divide (Position position)
    {
        X /= position.X;
        Y /= position.Y;
        Z /= position.Z;
    }

    public void Divide(float factor)
    {
        X /= factor;
        Y /= factor;
        Z /= factor;
    }

    public static float Distance(Position a, Position b)
    {
        return System.Math.Abs(a.X - b.X);
    }

    public static Position operator +(Position a, Position b)
    {
        return new Position(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    }

    public static Position operator -(Position a, Position b)
    {
        return new Position(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    }

    public static Position operator *(Position a, Position b)
    {
        return new Position(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
    }

    public static Position operator *(Position a, int factor)
    {
        return new Position(a.X * factor, a.Y * factor, a.Z * factor);
    }

    public static Position operator *(Position a, float factor)
    {
        return new Position(a.X * factor, a.Y * factor, a.Z * factor);
    }

    public static Position operator /(Position a, Position b)
    {
        return new Position(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
    }

    public static Position operator /(Position a, int factor)
    {
        return new Position(a.X / factor, a.Y / factor, a.Z / factor);
    }

    public static Position operator /(Position a, float factor)
    {
        return new Position(a.X / factor, a.Y / factor, a.Z / factor);
    }
}