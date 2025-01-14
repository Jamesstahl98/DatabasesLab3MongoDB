public class Armor : DiceModifierItem
{
    public Armor(Position pos, char c, ConsoleColor color) : base(pos, c, color)
    {
        Amount = 1;
        DiceType = "defence";
        Name = "armor";
        DiceName = "armor modifier";
    }
}