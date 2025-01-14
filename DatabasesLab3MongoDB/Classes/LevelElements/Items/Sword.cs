public class Sword : DiceModifierItem
{
    public Sword(Position pos, char c, ConsoleColor color) : base(pos, c, color)
    {
        Amount = 1;
        DiceType = "attack";
        Name = "sword";
        DiceName = "attack modifier";
    }
}