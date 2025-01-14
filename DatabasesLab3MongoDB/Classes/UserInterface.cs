using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

public static class UserInterface
{
    private static readonly List<string> combatLogEntries = new List<string>();

    public static void PrintSaveFiles()
    {
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

    public static void PrintCombatLog(Creature attacker, Creature defender, int attackRoll, int defenceRoll, int damage)
    {
        combatLogEntries.Add($"{attacker.Name} (ATK: {attacker.AttackDice} => {attackRoll}) " +
            $"attacked {defender.Name} " +
            $"(DEF: {defender.DefenceDice} => {defenceRoll}), " +
            $"dealing {damage} damage. ");

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
}