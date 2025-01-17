using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class MongoDBHandler
{
    private static IMongoCollection<SaveFile> GetSaveFileCollection(string connectionString, string databaseName, string collectionName)
    {
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        return database.GetCollection<SaveFile>(collectionName);
    }

    public static void SaveToMongoDB(string connectionString, string databaseName, string collectionName, string saveFileName)
    {
        var collection = GetSaveFileCollection(connectionString, databaseName, collectionName);

        var saveFile = new SaveFile
        {
            FileName = saveFileName,
            LastModified = DateTime.Now,
            LevelElements = LevelData.Elements,
            Turn = GameLoop.TurnCounter,
            CombatLog = UserInterface.FullCombatLog,
            LineCount = LevelData.LineCount
        };

        var existingSaveFile = collection.Find(sf => sf.FileName == saveFileName).FirstOrDefault();

        if (existingSaveFile != null)
        {
            saveFile.Id = existingSaveFile.Id;

            collection.ReplaceOne(
                sf => sf.FileName == saveFileName,
                saveFile
            );
        }
        else
        {
            collection.InsertOne(saveFile);
        }

        UserInterface.PrintMessage("Game saved. Press any key to continue");
        Console.ReadKey();
    }

    public static void DeleteSaveFile(string connectionString, string databaseName, string collectionName, string saveFileName)
    {
        var collection = GetSaveFileCollection(connectionString, databaseName, collectionName);
        collection.DeleteOne(sf => sf.FileName == saveFileName);
    }

    public static List<SaveFile> GetSaveFiles(string connectionString, string databaseName, string collectionName)
    {
        var collection = GetSaveFileCollection(connectionString, databaseName, collectionName);
        return collection.Find(_ => true).ToList();
    }

    public static SaveFile LoadFromMongoDB(string connectionString, string databaseName, string collectionName, string saveFileName)
    {
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        var collection = database.GetCollection<SaveFile>(collectionName);

        var saveFile = collection.Find(sf => sf.FileName == saveFileName).FirstOrDefault();

        if (saveFile == null)
        {
            return null;
        }

        return saveFile;
    }
}
