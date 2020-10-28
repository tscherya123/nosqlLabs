using Data.DAO;
using Data.Factories;
using Data.Migrators;
using System;
using System.Windows.Controls;
using WpfApp1.Configuration;

namespace WpfApp1.Pages
{
    /// <summary>
    /// Interaction logic for MigrationPage.xaml
    /// </summary>
    public partial class MigrationPage : Page
    {
        private readonly ConnectionFactory connectionFactory;

        public MigrationPage()
        {
            InitializeComponent();

            connectionFactory = new ConnectionFactory(SqlServerConfiguration.Settings, MongoDbConfiguration.Settings);

            ToSQL.Click += ToSql_Click;
            ToMongo.Click += ToMongo_Click;
        }

        private void ToMongo_Click(object sender, EventArgs e)
        {
            DisplayMigration(false);
        }

        private void ToSql_Click(object sender, EventArgs e)
        {
            DisplayMigration(true);
        }

        private (IInfoDAO, ICreatorDAO, IItemDAO, IInfoDAO, ICreatorDAO, IItemDAO) GetDaos(bool toSql)
        {
            var daoFactory = new DAOFactory(connectionFactory.GetConnection(), connectionFactory.GetMongoClient());

            return (
                daoFactory.GetConnection<IInfoDAO>(toSql),
                daoFactory.GetConnection<ICreatorDAO>(toSql),
                daoFactory.GetConnection<IItemDAO>(toSql),

                daoFactory.GetConnection<IInfoDAO>(!toSql),
                daoFactory.GetConnection<ICreatorDAO>(!toSql),
                daoFactory.GetConnection<IItemDAO>(!toSql)
           );
        }

        private void MigrateData(bool toSql)
        {
            var daos = GetDaos(toSql);

            DbMigrator.Migrate(daos.Item1, daos.Item2, daos.Item3, daos.Item4, daos.Item5, daos.Item6);
        }

        private void DisplayMigration(bool toSql)
        {
            Status.Text = "...";
            ExceptionMessage.Text = string.Empty;

            try
            {
                MigrateData(toSql);
            }
            catch (Exception ex)
            {
                Status.Text = "Failed";
                ExceptionMessage.Text = ex.Message;

                return;
            }

            Status.Text = "Done";
        }
    }
}
