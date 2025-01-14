using System.Diagnostics;

public class Player : Creature
{
    public int Turn { get; set; }
    public Player(Position pos, char c, ConsoleColor color) :base(pos, c, color)
    {
        Turn = 0;
        Name = "Player";
        HP = 100;
        AttackDice = new Dice(2, 6, 2);
        DefenceDice = new Dice(2, 6, 0);
    }

    public void ReadPlayerInput()
    {
        ConsoleKeyInfo cki;
        cki = Console.ReadKey(true);
        if (cki.Key == ConsoleKey.LeftArrow)
        {
            Update(new Position(Position.X - 1, Position.Y));
        }
        else if (cki.Key == ConsoleKey.RightArrow)
        {
            Update(new Position(Position.X + 1, Position.Y));
        }
        else if (cki.Key == ConsoleKey.UpArrow)
        {
            Update(new Position(Position.X, Position.Y - 1));
        }
        else if (cki.Key == ConsoleKey.DownArrow)
        {
            Update(new Position(Position.X, Position.Y + 1));
        }
        else if (cki.Key == ConsoleKey.Escape)
        {
            Environment.Exit(0);
        }
    }

    public void Update(Position newPos)
    {
        UserInterface.ClearLog();

        Move(newPos);

        Draw();
    }
}