using MongoDB.Driver.Core.Configuration;
using MongoDB.Driver;
using System.Diagnostics;

public static class GameLoop
{
    public static string selectedSaveFileName;
    public static int TurnCounter { get; set; }

    public static void Start()
    {
        Console.CursorVisible = false;

        LoadGame();

        UpdateWalls();

        while(LevelData.Player.HP > 0)
        {
            UserInterface.PrintPlayerHPAndTurn(LevelData.Player.HP, TurnCounter);
            bool shouldUpdateGame = LevelData.Player.ReadPlayerInput();
            if(shouldUpdateGame)
            {
                UpdateEnemies();
                UpdateWalls();
                TurnCounter++;
            }
        }
        UserInterface.GameOver();
    }

    private static void UpdateEnemies()
    {
        foreach (LevelElement element in LevelData.Elements.ToList())
        {
            (element as Enemy)?.Update();
            (element as Item)?.Update();
        }
    }

    private static void UpdateWalls()
    {
        IEnumerable<Wall> walls = LevelData.Elements.OfType<Wall>();

        foreach (Wall wall in walls)
        {
            wall.CheckIfPlayerInRange();
        }
    }

    public static void SaveAndExitGame()
    {
        MongoDBHandler.SaveToMongoDB(
            "mongodb://localhost:27017",
            "JamesStåhl",
            "SaveFiles",
            selectedSaveFileName);

        Environment.Exit(0);
    }

    public static void ExitGame()
    {
        Environment.Exit(0);
    }

    public static void SaveGame()
    {
        MongoDBHandler.SaveToMongoDB(
            "mongodb://localhost:27017",
            "JamesStåhl",
            "SaveFiles",
            selectedSaveFileName);
    }

    public static void LoadGame()
    {
        UserInterface.PrintSaveFiles();

        selectedSaveFileName = UserInterface.ChooseSaveFile();

        SaveFile saveFile = MongoDBHandler.LoadFromMongoDB(
            "mongodb://localhost:27017",
            "JamesStåhl",
            "SaveFiles",
            selectedSaveFileName);

        if (saveFile != null)
        {
            LevelData.LoadFromSaveFile(saveFile);
        }
        else
        {
            Console.WriteLine("No save file found. Loading new level...");
            UserInterface.PressAnyKeyToContinue();
            LevelData.LoadNewGame("Level1.txt");
            LevelData.Player.Name = UserInterface.GetPlayerName();
        }
    }

    public static void NewGame()
    {
        Debug.WriteLine("NewGame");
    }
}