namespace Data.Constants
{
    public class MongoDbConnectionConstants
    {
        public static string ConnectionString = "mongodb://{0}:{1},{2}:{3},{4}:{5}?replicaSet=" + ReplicaSetName;

        public const string DbName = "db";

        public const string CreatorsCollectionName = "creators";

        public const string InfosCollectionName = "infos";

        public const string ItemsCollection = "items";

        public const string ItemCreatorCollection = "itemCreator";

        public const string ReplicaSetName = "myreplica";
    }
}
