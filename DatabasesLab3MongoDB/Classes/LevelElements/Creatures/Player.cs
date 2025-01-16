﻿using MongoDB.Bson.Serialization.Attributes;
using System.Diagnostics;

public class Player : Creature
{
    public Player(Position pos, char c, ConsoleColor color) :base(pos, c, color)
    {
        Name = "Player";
        HP = 100;
        AttackDice = new Dice(2, 6, 2);
        DefenceDice = new Dice(2, 6, 0);
    }

    public bool ReadPlayerInput()
    {
        ConsoleKeyInfo cki = Console.ReadKey(true);

        if (cki.Key == ConsoleKey.Escape)
        {
            UserInterface.OpenMenu();
            return false;
        }

        switch (cki.Key)
        {
            case ConsoleKey.LeftArrow:
                Update(new Position(Position.X - 1, Position.Y));
                break;
            case ConsoleKey.RightArrow:
                Update(new Position(Position.X + 1, Position.Y));
                break;
            case ConsoleKey.UpArrow:
                Update(new Position(Position.X, Position.Y - 1));
                break;
            case ConsoleKey.DownArrow:
                Update(new Position(Position.X, Position.Y + 1));
                break;
            case ConsoleKey.Spacebar:
                break;
            default:
                return false;
        }
        return true;
    }

    public void Update(Position newPos)
    {
        UserInterface.ClearLog();

        Move(newPos);

        Draw();
    }
}