public class Potion : Item
{
    public int HPRestore { get; set; }
    public Potion(Position pos, char c, ConsoleColor color) : base(pos, c, color)
    {
        HPRestore = 50;
    }

    public override void ElementContact(Creature element)
    {
        element.HP += HPRestore;
        if(element.HP>100)
        {
            element.HP = 100;
        }

        UserInterface.PrintItemPickup(this);

        RemoveElement();
    }

    public override void Update()
    {
        if (IsPlayerNearby())
        {
            Draw();
        }
        else
        {
            Console.SetCursorPosition(Position.X, Position.Y);
            Console.Write(" ");
        }
    }

    public override string ToString()
    {
        return $"Player picked up potion, restoring {HPRestore} health.";
    }
}