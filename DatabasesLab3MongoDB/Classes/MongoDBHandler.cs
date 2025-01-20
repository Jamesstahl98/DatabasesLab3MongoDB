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

    public static async Task SaveToMongoDBAsync(string connectionString, string databaseName, string collectionName, string saveFileName)
    {
        var collection = GetSaveFileCollection(connectionString, databaseName, collectionName);

        var saveFile = new SaveFile
        {
            FileName = saveFileName,
            Player = LevelData.Player as Player,
            LastModified = DateTime.Now,
            LevelElements = LevelData.Elements,
            Turn = GameLoop.TurnCounter,
            CombatLog = UserInterface.FullCombatLog,
            LineCount = LevelData.LineCount
        };

        var existingSaveFile = await collection.Find(sf => sf.FileName == saveFileName).FirstOrDefaultAsync();

        if (existingSaveFile != null)
        {
            saveFile.Id = existingSaveFile.Id;

            await collection.ReplaceOneAsync(
                sf => sf.FileName == saveFileName,
                saveFile
            );
        }
        else
        {
            await collection.InsertOneAsync(saveFile);
        }

        UserInterface.PrintMessage("Game saved. Press any key to continue");
        Console.ReadKey();
    }

    public static async Task DeleteSaveFileAsync(string connectionString, string databaseName, string collectionName, string saveFileName)
    {
        var collection = GetSaveFileCollection(connectionString, databaseName, collectionName);
        await collection.DeleteOneAsync(sf => sf.FileName == saveFileName);
    }

    public static async Task<bool> SaveFileExistsAsync(string connectionString, string databaseName, string collectionName, string saveFileName)
    {
        var collection = GetSaveFileCollection(connectionString, databaseName, collectionName);
        return await collection.Find(sf => sf.FileName == saveFileName).AnyAsync();
    }

    public static async Task<List<SaveFile>> GetSaveFilesAsync(string connectionString, string databaseName, string collectionName)
    {
        var collection = GetSaveFileCollection(connectionString, databaseName, collectionName);
        return await collection.Find(_ => true).ToListAsync();
    }

    public static async Task<SaveFile> LoadFromMongoDBAsync(string connectionString, string databaseName, string collectionName, string saveFileName)
    {
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        var collection = database.GetCollection<SaveFile>(collectionName);

        var saveFile = await collection.Find(sf => sf.FileName == saveFileName).FirstOrDefaultAsync();

        return saveFile;
    }
}
