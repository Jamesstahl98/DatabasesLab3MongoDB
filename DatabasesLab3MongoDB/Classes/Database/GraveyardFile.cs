using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class GraveyardFile
{
    [BsonId]
    public ObjectId Id { get; set; }
    public string FileName { get; set; }
    public LevelElement Player { get; set; }
    public int Turn { get; set; }
}
