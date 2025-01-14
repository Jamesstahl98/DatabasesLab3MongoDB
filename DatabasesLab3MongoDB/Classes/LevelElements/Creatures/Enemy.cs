using MongoDB.Bson.Serialization.Attributes;
using System.Diagnostics;
[Interactable]
public abstract class Enemy : Creature
{
    public bool WasAttackedThisRound { get; set; }
    public Enemy( Position pos, char c, ConsoleColor color) : base(pos, c, color) { }

    public abstract void Update();

    public override void ElementContact(Creature element)
    {
        WasAttackedThisRound = true;
        if (element != LevelData.Player)
        {
            return;
        }
        else
        {
            base.ElementContact(element);
        }
    }
}