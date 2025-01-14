using System.Numerics;

public abstract class LevelElement
{
    public Position Position { get; set; }
    protected char Character { get; set; }
    public ConsoleColor Color { get; set; }
    public bool IsDiscovered { get; set; }

    public LevelElement(Position pos, char c, ConsoleColor color)
    {
        Position = pos;
        Character = c;
        Color = color;
    }
    
    public void Draw()
    {
        Console.SetCursorPosition(Position.X, Position.Y);
        Console.ForegroundColor = Color;
        Console.Write(Character);
    }

    public void RemoveElement()
    {
        Console.SetCursorPosition(Position.X, Position.Y);
        Console.Write(" ");
        LevelData.Elements.Remove(this);
    }

    public bool IsPlayerNearby()
    {
        int posXDiff = Math.Abs(Position.X - LevelData.Player.Position.X);
        int posYDiff = Math.Abs(Position.Y - LevelData.Player.Position.Y);
        if ((posXDiff * posXDiff) + (posYDiff * posYDiff) < 25)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public (int, int) GetXAndYDistanceToElement(Position elementPosition)
    {
        return (Math.Abs(Position.X - elementPosition.X), 
            Math.Abs(Position.Y - elementPosition.Y));
    }

    public abstract void ElementContact(Creature element);
}