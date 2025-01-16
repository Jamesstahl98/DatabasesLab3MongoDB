using System.Globalization;
using System.IO;
using System.Security;

public static class LevelData
{
    private static List<LevelElement> _elements = new List<LevelElement>();

    public static List<LevelElement> Elements { get { return _elements; } }

    public static Player Player { get; set; }
    
    public static int LineCount { get; set; }

    public static void LoadFromSaveFile(SaveFile saveFile)
    {
        UserInterface.PressAnyKeyToContinue();
        _elements.Clear();

        _elements.AddRange(saveFile.LevelElements);
        Player = saveFile.Player;
        Player.Update(new Position(Player.Position.X, Player.Position.Y));
        GameLoop.TurnCounter = saveFile.Turn;
    }

    public static void LoadNewGame(string fileName)
    {
        using (StreamReader reader = new StreamReader(fileName))
        {
            while(!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                for (int i = 0; i < line.Length; i++)
                {
                    int charUnicode = line[i];
                    switch (charUnicode)
                    {
                        case 33:
                            Potion potion = new Potion(new Position(i, LineCount + 1), '!', ConsoleColor.DarkGreen);
                            _elements.Add(potion);
                            break;
                        case 35:
                            Wall wall = new Wall(new Position(i, LineCount + 1), '#', ConsoleColor.Gray);
                            _elements.Add(wall);
                            break;
                        case 64:
                            Player = new Player(new Position(i, LineCount + 1), '@', ConsoleColor.Yellow);
                            _elements.Add(Player);
                            break;
                        case 97:
                            Armor armor = new Armor(new Position(i, LineCount + 1), 'a', ConsoleColor.DarkYellow);
                            _elements.Add(armor);
                            break;
                        case 108:
                            Sword sword = new Sword(new Position(i, LineCount + 1), 'l', ConsoleColor.DarkYellow);
                            _elements.Add(sword);
                            break;
                        case 114:
                            Rat rat = new Rat(new Position(i, LineCount + 1), 'r', ConsoleColor.Red);
                            _elements.Add(rat);
                            break;
                        case 115:
                            Snake snake = new Snake(new Position(i, LineCount + 1), 's', ConsoleColor.Green);
                            _elements.Add(snake);
                            break;
                        case 116:
                            Troll troll = new Troll(new Position(i, LineCount + 1), 't', ConsoleColor.DarkCyan);
                            _elements.Add(troll);
                            break;
                    }
                }
                LineCount++;
            }
        }

        Player.Update(new Position(Player.Position.X, Player.Position.Y));
    }
}