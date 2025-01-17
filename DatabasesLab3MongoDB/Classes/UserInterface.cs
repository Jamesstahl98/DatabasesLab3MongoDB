using MongoDB.Driver;
using SharpCompress.Common;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

public static class UserInterface
{
    public enum MenuOptions
    {
        Continue,
        SaveGame,
        LoadGame,
        PrintCombatLog,
        SaveAndExitGame,
        DeleteSaveFile,
        ExitGame
    }

    private static readonly List<string> combatLogEntries = new List<string>();
    public static List<string> FullCombatLog = new List<string>();

    public static void ExecuteMenuAction(MenuOptions option)
    {
        switch (option)
        {
            case MenuOptions.Continue:
                break;
            case MenuOptions.SaveGame:
                GameLoop.SaveGame();
                break;
            case MenuOptions.LoadGame:
                GameLoop.LoadGame(false);
                break;
            case MenuOptions.PrintCombatLog:
                PrintFullCombatLog(FullCombatLog);
                break;
            case MenuOptions.SaveAndExitGame:
                GameLoop.SaveAndExitGame();
                break;
            case MenuOptions.DeleteSaveFile:
                GameLoop.DeleteSaveFile();
                break;
            case MenuOptions.ExitGame:
                GameLoop.ExitGame();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private static void PrintFullCombatLog(List<string> fullCombatLog)
    {
        Console.Clear();
        Console.SetCursorPosition(0, 0);

        Console.Title = "Combat Log";

        for (int i = 0; i < fullCombatLog.Count; i++)
        {
            Console.WriteLine(fullCombatLog[i]);
        }
        
        Console.WriteLine("Press any key to exit...");

        Console.ReadKey();
        LevelData.ReloadElements();
    }

    public static void PrintSaveFiles()
    {
        Console.Clear();
        var saveFiles = MongoDBHandler.GetSaveFiles("mongodb://localhost:27017", "JamesStåhl", "SaveFiles");

        Console.WriteLine("Available Save Files:");
        foreach (var saveFile in saveFiles)
        {
            Console.WriteLine($"- {saveFile.FileName} (Last Modified: {saveFile.LastModified})");
        }
    }

    public static string ChooseSaveFile()
    {
        Console.WriteLine("Enter the name of the save file to load or enter a new name:");
        string selectedFileName = Console.ReadLine();

        return selectedFileName;
    }

    public static string GetPlayerName()
    {
        Console.SetCursorPosition(0, 0);
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write("Please enter your name: ");
        string ?name = Console.ReadLine();

        ClearLog();

        if(string.IsNullOrWhiteSpace(name))
        {
            return "Player";
        }
        return name;
    }

    public static void PrintPlayerHPAndTurn(int health, int turn)
    {
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.SetCursorPosition(0, 0);
        Console.Write($"Turn: {turn}, Player Health: {health}");
    }

    public static void PrintMessage(string message)
    {
        ClearMenu();
        Console.SetCursorPosition(0, LevelData.LineCount);
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine(message);
    }

    public static void PrintCombatLog(Creature attacker, Creature defender, int attackRoll, int defenceRoll, int damage)
    {
        var entry = $"{attacker.Name} (ATK: {attacker.AttackDice} => {attackRoll}) " +
            $"attacked {defender.Name} " +
            $"(DEF: {defender.DefenceDice} => {defenceRoll}), " +
            $"dealing {damage} damage. ";
        combatLogEntries.Add(entry);
        FullCombatLog.Add(entry);

        Console.SetCursorPosition(0, LevelData.LineCount+combatLogEntries.Count);
        Console.ForegroundColor = attacker.Color;

        Console.Write(combatLogEntries[^1]);

        if (defender.HP <= 0)
        {
            Console.Write("Instantly killing it");
        }
        else
        {
            Console.Write($"{defender} has {defender.HP} health left.");
        }
    }

    public static void PrintItemPickup(Item item)
    {
        combatLogEntries.Add(item.ToString());
        FullCombatLog.Add(item.ToString());
        Console.SetCursorPosition(0, LevelData.LineCount + combatLogEntries.Count);
        Console.ForegroundColor = item.Color;

        Console.Write(combatLogEntries[^1]);
    }

    public static void ClearLog()
    {
        Console.SetCursorPosition(0, 0);
        Console.Write(new string(' ', Console.WindowWidth));
        for (int i = 0; i < combatLogEntries.Count; i++)
        {
            Console.SetCursorPosition(0, LevelData.LineCount + i+1);
            Console.Write(new string(' ', Console.WindowWidth));
        }
        combatLogEntries.Clear();
    }

    public static void GameOver()
    {
        ClearLog();
        Console.SetCursorPosition(0, 0);
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write("You died :(");
        Console.ReadKey();
    }

    public static void PressAnyKeyToContinue()
    {
        Console.WriteLine("Press any key to continue.");
        Console.ReadKey();
        Console.Clear();
    }

    public static void OpenMenu()
    {
        PrintMenu();
        GetMenuInput();
    }

    private static void PrintMenu()
    {
        ClearLog();
        Console.SetCursorPosition(0, LevelData.LineCount);
        
        foreach(MenuOptions option in Enum.GetValues(typeof(MenuOptions)))
        {
            Console.WriteLine(option);
        }
        Console.CursorVisible = true;
    }

    private static void GetMenuInput()
    {
        Console.SetCursorPosition(0, LevelData.LineCount);
        while (true)
        {
            ConsoleKeyInfo cki;
            cki = Console.ReadKey(true);
            var cursorPos = Console.GetCursorPosition();
            if (cki.Key == ConsoleKey.UpArrow)
            {
                if (cursorPos.Top - LevelData.LineCount - 1 >= 0)
                {
                    Console.SetCursorPosition(cursorPos.Left, cursorPos.Top - 1);
                }
                else
                {
                    Console.SetCursorPosition(cursorPos.Left, LevelData.LineCount + Enum.GetValues(typeof(MenuOptions)).Length - 1);
                }
            }
            else if (cki.Key == ConsoleKey.DownArrow)
            {
                if(cursorPos.Top - LevelData.LineCount >= Enum.GetValues(typeof(MenuOptions)).Length - 1)
                {
                    Console.SetCursorPosition(cursorPos.Left, LevelData.LineCount);
                }
                else
                {
                    Console.SetCursorPosition(cursorPos.Left, cursorPos.Top + 1);
                }
            }
            else if (cki.Key == ConsoleKey.Enter)
            {
                ExecuteMenuAction((MenuOptions)cursorPos.Top - LevelData.LineCount);
                break;
            }
        }
        Console.CursorVisible = false;
        ClearMenu();
    }

    private static void ClearMenu()
    {
        Console.SetCursorPosition(0, 0);
        Console.Write(new string(' ', Console.WindowWidth));
        for (int i = 0; i < Enum.GetValues(typeof(MenuOptions)).Length; i++)
        {
            Console.SetCursorPosition(0, LevelData.LineCount + i);
            Console.Write(new string(' ', Console.WindowWidth));
        }
    }
}