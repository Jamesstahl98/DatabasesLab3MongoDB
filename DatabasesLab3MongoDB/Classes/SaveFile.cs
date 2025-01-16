﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SaveFile
{
    [BsonId]
    public ObjectId Id { get; set; }
    public string FileName { get; set; }
    public List<LevelElement> LevelElements { get; set; } = new List<LevelElement>();
    public Player Player { get; set; }
    public DateTime LastModified { get; set; }
    public int Turn { get; set; }
}
