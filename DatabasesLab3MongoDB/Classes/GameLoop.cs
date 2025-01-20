using MongoDB.Driver.Core.Configuration;
using MongoDB.Driver;
using System.Diagnostics;

public static class GameLoop
{
    public static string SelectedSaveFileName;
    public static int TurnCounter { get; set; }

    public static async Task StartAsync()
    {
        Console.CursorVisible = false;

        await LoadGameAsync(true);

        UpdateWalls();

        while (LevelData.Player.HP > 0)
        {
            UserInterface.PrintPlayerHPAndTurn(LevelData.Player.HP, TurnCounter);
            bool shouldUpdateGame = await LevelData.Player.ReadPlayerInputAsync();

            if (shouldUpdateGame)
            {
                UpdateEnemies();
                UpdateWalls();
                TurnCounter++;
            }
        }

        await MongoDBHandler.DeleteSaveFileAsync("mongodb://localhost:27017", "DatabaseName", "SaveFiles", SelectedSaveFileName);
        UserInterface.GameOver();
        await MongoDBHandler.SaveToMongoDBAsync("mongodb://localhost:27017", "DatabaseName", "Graveyard", SelectedSaveFileName);
    }

    private static void UpdateEnemies()
    {
        foreach (LevelElement element in LevelData.Elements.ToList())
        {
            if (element is Enemy enemy)
                enemy.Update();
            else if (element is Item item)
                item.Update();
        }
    }

    private static void UpdateWalls()
    {
        var walls = LevelData.Elements.OfType<Wall>();

        foreach (Wall wall in walls)
        {
            wall.CheckIfPlayerInRange();
        }
    }

    public static async Task SaveAndExitGameAsync()
    {
        await SaveGameAsync();
        Environment.Exit(0);
    }

    public static void ExitGame()
    {
        Environment.Exit(0);
    }

    public static async Task SaveGameAsync()
    {
        await MongoDBHandler.SaveToMongoDBAsync(
            "mongodb://localhost:27017",
            "DatabaseName",
            "SaveFiles",
            SelectedSaveFileName);
    }

    public static async Task LoadGameAsync(bool startNewGameWhenNoMatch)
    {
        await UserInterface.PrintSaveFilesAsync();

        SelectedSaveFileName = UserInterface.ChooseSaveFile();

        SaveFile saveFile = await MongoDBHandler.LoadFromMongoDBAsync(
            "mongodb://localhost:27017",
            "DatabaseName",
            "SaveFiles",
            SelectedSaveFileName);

        if (saveFile != null)
        {
            LevelData.LoadFromSaveFile(saveFile);
        }
        else if (startNewGameWhenNoMatch)
        {
            Console.WriteLine("No save file found. Loading new level...");
            UserInterface.PressAnyKeyToContinue();
            LevelData.LoadNewGame("Level1.txt");
            LevelData.Player.Name = UserInterface.GetPlayerName();
        }
        else
        {
            Console.WriteLine("No save file found.");
            UserInterface.PressAnyKeyToContinue();
            LevelData.ReloadElements();
        }
    }

    public static async Task DeleteSaveFileAsync()
    {
        await UserInterface.PrintSaveFilesAsync();
        Console.WriteLine("Enter the name of the save file or leave it blank to exit:");

        Console.Write("\nEnter save file name: ");
        string selectedFileName = Console.ReadLine();

        bool saveFileExists = await MongoDBHandler.SaveFileExistsAsync(
            "mongodb://localhost:27017",
            "DatabaseName",
            "SaveFiles",
            selectedFileName);

        if (saveFileExists)
        {
            await MongoDBHandler.DeleteSaveFileAsync(
                "mongodb://localhost:27017",
                "DatabaseName",
                "SaveFiles",
                selectedFileName);
            Console.WriteLine("Save file deleted. Press any key to continue...");
        }
        else
        {
            Console.WriteLine("Save file not found.");
        }

        Console.ReadKey();
        Console.Clear();
        LevelData.ReloadElements();
    }
}