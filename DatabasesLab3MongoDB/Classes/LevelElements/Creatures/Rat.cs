using System;
using System.Diagnostics;

public class Rat : Enemy
{
    private static readonly Random rand = new Random();
    private enum Directions { left, up, right, down};

    public Rat(Position pos, char c, ConsoleColor color) 
        : base(pos, c, color)
    {
        Name = "rat";
        HP = 10;
        AttackDice = new Dice(1, 6, 3);
        DefenceDice = new Dice(1, 6, 1);
    }

    public override void Update()
    {
        if (WasAttackedThisRound)
        {
            WasAttackedThisRound = false;
            return;
        }

        Position newPos = GetNewPosition(new Position(Position.X, Position.Y));

        Move(newPos);

        if (IsPlayerNearby() && HP > 0)
        {
            Draw();
        }
    }

    public Position GetNewPosition(Position position)
    {
        Array dirValues = Enum.GetValues(typeof(Directions));
        Directions randomDirection = (Directions)dirValues.GetValue(rand.Next(dirValues.Length));
        switch (randomDirection)
        {
            case Directions.left:
                position.X -= 1;
                break;
            case Directions.up:
                position.Y -= 1;
                break;
            case Directions.right:
                position.X += 1;
                break;
            case Directions.down:
                position.Y += 1;
                break;
        }
        return position;
    }
}