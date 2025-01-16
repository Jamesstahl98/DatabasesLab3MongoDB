using MongoDB.Driver.Core.Configuration;
using MongoDB.Driver;
using System.Diagnostics;

public static class GameLoop
{
    private static string selectedSaveFileName;
    public static int TurnCounter { get; set; }

    public static void Start()
    {
        Console.CursorVisible = false;
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

        UpdateWalls();

        while(LevelData.Player.HP > 0)
        {
            UserInterface.PrintPlayerHPAndTurn(LevelData.Player.HP, TurnCounter);
            LevelData.Player.ReadPlayerInput();
            UpdateEnemies();
            UpdateWalls();
            TurnCounter++;
        }
        UserInterface.GameOver();
    }

    public static void SaveAndExitGame()
    {
        if(LevelData.Player.HP <= 0)
        {
            //Delete save file
            //Add obituary to obituary file if there is time
            Environment.Exit(0);
        }
        MongoDBHandler.SaveToMongoDB(
            "mongodb://localhost:27017",
            "JamesStåhl",
            "SaveFiles",
            selectedSaveFileName);

        //Save turn counter
        //Save combat log
        Environment.Exit(0);
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
}