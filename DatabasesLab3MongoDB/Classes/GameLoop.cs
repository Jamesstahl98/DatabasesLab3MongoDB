public static class GameLoop
{
    public static int TurnCounter { get; set; }

    public static void Start()
    {
        Console.CursorVisible = false;
        LevelData.Load("Level1.txt");
        LevelData.Player.Name = UserInterface.GetPlayerName();
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