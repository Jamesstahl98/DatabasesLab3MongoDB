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
        PrintGraveyard,
        SaveAndExitGame,
        DeleteSaveFile,
        ExitGame
    }

    private static readonly List<string> CombatLogEntries = new();
    public static readonly List<string> FullCombatLog = new();

    public static async Task ExecuteMenuActionAsync(MenuOptions option)
    {
        switch (option)
        {
            case MenuOptions.Continue:
                break;
            case MenuOptions.SaveGame:
                await GameLoop.SaveGameAsync();
                break;
            case MenuOptions.LoadGame:
                await GameLoop.LoadGameAsync(false);
                break;
            case MenuOptions.PrintCombatLog:
                PrintFullCombatLog(FullCombatLog);
                break;
            case MenuOptions.PrintGraveyard:
                await PrintGraveyardAsync();
                break;
            case MenuOptions.SaveAndExitGame:
                await GameLoop.SaveAndExitGameAsync();
                break;
            case MenuOptions.DeleteSaveFile:
                await GameLoop.DeleteSaveFileAsync();
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

        foreach (var entry in fullCombatLog)
        {
            Console.WriteLine(entry);
        }

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
        LevelData.ReloadElements();
    }

    public static async Task PrintSaveFilesAsync()
    {
        Console.Clear();
        var saveFiles = await MongoDBHandler.GetSaveFilesAsync("mongodb://localhost:27017", "JamesStåhl", "SaveFiles");

        Console.WriteLine("Available Save Files:");
        foreach (var saveFile in saveFiles)
        {
            Console.WriteLine($"- {saveFile.FileName} (Last Modified: {saveFile.LastModified})");
        }
    }

    public static string ChooseSaveFile()
    {
        Console.WriteLine("Enter the name of the save file to load or enter a new name:");
        return Console.ReadLine();
    }

    public static string GetPlayerName()
    {
        Console.SetCursorPosition(0, 0);
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write("Please enter your name: ");
        string? name = Console.ReadLine();

        ClearLog();

        return string.IsNullOrWhiteSpace(name) ? "Player" : name;
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
                    $"dealing {damage} damage.";

        CombatLogEntries.Add(entry);
        FullCombatLog.Add(entry);

        Console.SetCursorPosition(0, LevelData.LineCount + CombatLogEntries.Count);
        Console.ForegroundColor = attacker.Color;

        Console.Write(entry);

        if (defender.HP <= 0)
        {
            Console.Write(" Instantly killing it.");
        }
        else
        {
            Console.Write($" {defender.Name} has {defender.HP} health left.");
        }
    }

    private static async Task PrintGraveyardAsync()
    {
        var graveyard = await MongoDBHandler.GetSaveFilesAsync("mongodb://localhost:27017", "JamesStåhl", "Graveyard");
        ClearMenu();
        Console.SetCursorPosition(0, LevelData.LineCount);
        Console.WriteLine("Graveyard:");
        foreach (var saveFile in graveyard)
        {
            var player = saveFile.Player as Player;
            Console.WriteLine($"- {saveFile.FileName}, Player name: {player?.Name}," +
                $"\nEnemies killed: {player?.EnemiesKilled}, Items picked up: {player?.ItemsPickedUp} " +
                $"\nTurns survived: {saveFile.Turn}");
        }
        PressAnyKeyToContinue();
        LevelData.ReloadElements();
    }

    public static void PrintItemPickup(Item item)
    {
        CombatLogEntries.Add(item.ToString());
        FullCombatLog.Add(item.ToString());
        Console.SetCursorPosition(0, LevelData.LineCount + CombatLogEntries.Count);
        Console.ForegroundColor = item.Color;

        Console.Write(CombatLogEntries[^1]);
    }

    public static void ClearLog()
    {
        Console.SetCursorPosition(0, 0);
        Console.Write(new string(' ', Console.WindowWidth));
        for (int i = 0; i < CombatLogEntries.Count; i++)
        {
            Console.SetCursorPosition(0, LevelData.LineCount + i + 1);
            Console.Write(new string(' ', Console.WindowWidth));
        }
        CombatLogEntries.Clear();
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

    public static async Task OpenMenuAsync()
    {
        PrintMenu();
        await GetMenuInputAsync();
    }

    private static void PrintMenu()
    {
        ClearLog();
        Console.SetCursorPosition(0, LevelData.LineCount);

        foreach (MenuOptions option in Enum.GetValues(typeof(MenuOptions)))
        {
            Console.WriteLine(option);
        }
        Console.CursorVisible = true;
    }

    private static async Task GetMenuInputAsync()
    {
        Console.SetCursorPosition(0, LevelData.LineCount);
        while (true)
        {
            ConsoleKeyInfo cki = Console.ReadKey(true);
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
                if (cursorPos.Top - LevelData.LineCount >= Enum.GetValues(typeof(MenuOptions)).Length - 1)
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
                await ExecuteMenuActionAsync((MenuOptions)(cursorPos.Top - LevelData.LineCount));
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