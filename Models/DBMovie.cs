﻿using Amazon.DynamoDBv2.DataModel;

namespace FirstWebApi
{
    [DynamoDBTable("WatchList")]
    public class DBMovie
    {
        [DynamoDBHashKey]
        public int id { get; set; }
        [DynamoDBProperty]
        public string Login { get; set; }
        [DynamoDBProperty]
        public int TmdbId { get; set; }
    }
}