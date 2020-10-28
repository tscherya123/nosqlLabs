using MongoDB.Bson.Serialization.Attributes;

namespace Data.Models
{
    [BsonIgnoreExtraElements]
    public class ItemCreatorMongoModel
    {
        public int Item { get; set; }

        public int Creator { get; set; }
    }
}
