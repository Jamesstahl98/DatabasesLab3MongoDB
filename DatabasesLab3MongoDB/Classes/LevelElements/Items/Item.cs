[Interactable]
public abstract class Item : LevelElement
{
    public Item(Position pos, char c, ConsoleColor color) : base(pos, c, color) { }

    public abstract void Update();
}