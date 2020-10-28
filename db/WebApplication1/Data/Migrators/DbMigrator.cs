using Data.DAO;

namespace Data.Migrators
{
    public static class DbMigrator
    {
        public static void Migrate(
            IInfoDAO fromDao1, ICreatorDAO fromDao2, IItemDAO fromDao3,
            IInfoDAO toDao1, ICreatorDAO toDao2, IItemDAO toDao3)
        {
            fromDao1.MigrateTo(toDao1);
            fromDao2.MigrateTo(toDao2);
            fromDao3.MigrateTo(toDao3);
        }
    }
}
