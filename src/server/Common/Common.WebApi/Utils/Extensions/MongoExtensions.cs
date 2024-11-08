using MongoDB.Bson;
using MongoDB.Driver;

namespace Common.WebApi.Utils.Extensions;

public static class MongoExtensions
{
    public static BsonDocument RenderToBsonDocument<T>(this FilterDefinition<T> filter) =>
        filter.RenderToBsonDocument();
}