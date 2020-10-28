using Core.Models;
using Core.Momento;
using Core.Observers;
using Data.Connections;
using Data.DAO;
using Data.Factories;
using System;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Configuration;

namespace WpfApp1.Pages
{
    /// <summary>
    /// Interaction logic for CreatorPage.xaml
    /// </summary>
    public partial class CreatorPage : Page
    {
        private readonly ICreatorDAO creatorDAO;
        private readonly Observer observer;

        public CreatorPage()
        {
            InitializeComponent();

            var connectionFactory = new ConnectionFactory(SqlServerConfiguration.Settings, MongoDbConfiguration.Settings);
            creatorDAO = new DAOFactory(connectionFactory.GetConnection(), connectionFactory.GetMongoClient()).GetConnection<ICreatorDAO>(ApplicationConfiguration.IsMongo);

            observer = new Observer(GetAll_Click);
            creatorDAO.AddObserver(observer);

            Get_.Click += Get_Click;
            GetAll.Click += GetAll_Click;
            Add.Click += Add_Click;
            Update.Click += Update_Click;
            Delete.Click += Delete_Click;
        }

        private void Get_Click(object sender, RoutedEventArgs e)
        {
            int? id = null;
            Creators.Items.Clear();

            if (string.IsNullOrWhiteSpace(Id.Text))
            {
                id = null;
            }
            else
            {
                int idValue;
                var isNum = int.TryParse(Id.Text, out idValue);

                if (isNum)
                {
                    id = idValue;
                }
            }

            var text = string.IsNullOrWhiteSpace(Name.Text) ? null : Name.Text;

            var creators = creatorDAO.Get(id, text);

            foreach (var creator in creators)
            {
                Creators.Items.Add(creator);
            }
        }

        private void GetAll_Click(object sender, EventArgs e)
        {
            var creators = creatorDAO.GetAll();
            Creators.Items.Clear();

            foreach (var creator in creators)
            {
                Creators.Items.Add(creator);
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Name.Text))
            {
                var creator = new Creator()
                {
                    Name = Name.Text
                };

                creatorDAO.Add(creator);
            }
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Name.Text) && !string.IsNullOrWhiteSpace(Id.Text))
            {
                int id;

                if (int.TryParse(Id.Text, out id))
                {
                    var creator = new Creator()
                    {
                        Name = Name.Text
                    };

                    creatorDAO.Update(id, creator);
                }
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Id.Text))
            {
                int id;

                if (int.TryParse(Id.Text, out id))
                {
                    creatorDAO.Delete(id);
                }
            }
        }
    }
}
