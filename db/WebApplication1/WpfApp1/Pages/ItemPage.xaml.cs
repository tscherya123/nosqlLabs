using Core.Models;
using Data.Connections;
using Data.DAO;
using Data.Factories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Configuration;
using WpfApp1.ViewModels;
using Core.Observers;

namespace WpfApp1.Pages
{
    /// <summary>
    /// Interaction logic for ItemPage.xaml
    /// </summary>
    public partial class ItemPage : Page
    {
        private readonly IItemDAO itemDAO;
        private readonly ConnectionFactory connectionFactory;
        private readonly Observer observer;

        public ItemPage()
        {
            InitializeComponent();

            connectionFactory = new ConnectionFactory(SqlServerConfiguration.Settings, MongoDbConfiguration.Settings);
            itemDAO = new DAOFactory(connectionFactory.GetConnection(), connectionFactory.GetMongoClient()).GetConnection<IItemDAO>(ApplicationConfiguration.IsMongo);

            observer = new Observer(GetAll_Click);
            itemDAO.AddObserver(observer);

            Loaded += Page_Load;
            Get_.Click += Get_Click;
            GetAll.Click += GetAll_Click;
            Add.Click += Add_Click;
            Update.Click += Update_Click;
            Delete.Click += Delete_Click;
        }

        private void Page_Load(object sender, RoutedEventArgs e)
        {
            var creators = (new DAOFactory(connectionFactory.GetConnection(), connectionFactory.GetMongoClient()).GetConnection<ICreatorDAO>(ApplicationConfiguration.IsMongo)).GetAll();
            var infos = (new DAOFactory(connectionFactory.GetConnection(), connectionFactory.GetMongoClient()).GetConnection<IInfoDAO>(ApplicationConfiguration.IsMongo)).GetAll();

            foreach (var creator in creators)
            {
                Creators.Items.Add(creator);
            }

            foreach (var info in infos)
            {
                Infos.Items.Add(info);
            }
        }

        private void Get_Click(object sender, RoutedEventArgs e)
        {
            int? id = null;

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

            var items = itemDAO.Get(id, Infos.SelectedItem as Info, Creators.SelectedItem as Creator);

            Items.Items.Clear();

            foreach (var item in items)
            {
                var creators = new StringBuilder();

                foreach (var creator in item.Creators)
                {
                    creators.Append($"{creator.Name}, ");
                }

                Items.Items.Add(new ItemViewModel()
                {
                    Id = item.Id,
                    Info = item.Info,
                    Creators = creators.ToString()
                });
            }
        }

        private void GetAll_Click(object sender, EventArgs e)
        {
            var items = itemDAO.GetAll();
            Items.Items.Clear();

            foreach (var item in items)
            {
                var creators = new StringBuilder();

                foreach (var creator in item.Creators)
                {
                    creators.Append($"{creator.Name}, ");
                }

                Items.Items.Add(new ItemViewModel()
                {
                    Id = item.Id,
                    Info = item.Info,
                    Creators = creators.ToString()
                });
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var item = new Item()
            {
                Info = Infos.SelectedItem as Info,
                Creators = new List<Creator>()
            };
            var creatorsList = Creators.SelectedItems;

            foreach (var creator in creatorsList)
            {
                item.Creators.Add(creator as Creator);
            }

            itemDAO.Add(item);

            GetAll_Click(sender, e);
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Id.Text))
            {
                int id;

                if (int.TryParse(Id.Text, out id))
                {
                    var creators = new List<Creator>();

                    foreach (var creator in Creators.SelectedItems)
                    {
                        creators.Add(creator as Creator);
                    }

                    var item = new Item()
                    {
                        Info = Infos.SelectedItem as Info,
                        Creators = creators
                    };

                    itemDAO.Update(id, item);

                    GetAll_Click(sender, e);
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
                    itemDAO.Delete(id);

                    GetAll_Click(sender, e);
                }
            }
        }
    }
}
