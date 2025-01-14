using MongoDB.Bson.Serialization.Attributes;

public class Wall : LevelElement
{
    public Wall(Position pos, char c, ConsoleColor color) : base(pos, c, color) { }

    public override void ElementContact(Creature element) { }

    public void OutOfRange()
    {
        if(IsDiscovered)
        {
            Console.ForegroundColor = Color + 1;
            Console.SetCursorPosition(Position.X, Position.Y);
            Console.Write(Character);
        }
    }

    public void CheckIfPlayerInRange()
    {
        if(IsPlayerNearby())
        {
            IsDiscovered = true;
            Draw();
        }
        else
        {
            OutOfRange();
        }
    }
}