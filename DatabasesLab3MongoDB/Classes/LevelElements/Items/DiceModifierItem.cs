using System.Diagnostics;

public abstract class DiceModifierItem : Item
{
    public int Amount { get; set; }
    public string DiceType { get; set; }
    public string DiceName { get; set; }
    public string Name { get; set; }

    public DiceModifierItem(Position pos, char c, ConsoleColor color) : base(pos, c, color)
    {
        Name = "default name";
        DiceName = "default dice name";
        DiceType = "default type";
    }

    public override void ElementContact(Creature element)
    {
        if(DiceType == "defence")
        {
            element.DefenceDice.Modifier += Amount;
        }
        else
        {
            element.AttackDice.Modifier += Amount;
        }

        UserInterface.PrintItemPickup(this);

        RemoveElement();

        (element as Player).ItemsPickedUp++;
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
        return $"Player picked up {Name}, " +
                $"increasing {DiceName} " +
                $"by {Amount}.";

    }
}