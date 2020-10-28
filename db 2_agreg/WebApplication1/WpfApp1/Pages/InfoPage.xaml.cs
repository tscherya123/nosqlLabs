using Core.Models;
using Core.Observers;
using Data.Connections;
using Data.DAO;
using Data.Factories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp1.Configuration;

namespace WpfApp1.Pages
{
    /// <summary>
    /// Interaction logic for InfoPage.xaml
    /// </summary>
    public partial class InfoPage : Page
    {
        private readonly IInfoDAO infoDAO;
        private readonly Observer observer;

        public InfoPage()
        {
            InitializeComponent();

            var connectionFactory = new ConnectionFactory(SqlServerConfiguration.Settings, MongoDbConfiguration.Settings);
            infoDAO = new DAOFactory(connectionFactory.GetConnection(), connectionFactory.GetMongoClient()).GetConnection<IInfoDAO>(ApplicationConfiguration.IsMongo);

            observer = new Observer(GetAll_Click);
            infoDAO.AddObserver(observer);

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

            var creators = infoDAO.Get(id, CreationDate.SelectedDate, text);

            foreach (var creator in creators)
            {
                Creators.Items.Add(creator);
            }
        }

        private void GetAll_Click(object sender, EventArgs e)
        {
            var creators = infoDAO.GetAll();
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
                DateTime dateTime;

                if (CreationDate.SelectedDate == null)
                {
                    return;
                } else
                {
                    dateTime = (DateTime)CreationDate.SelectedDate;
                }

                var info = new Info()
                {
                    CreatonDate = dateTime,
                    Name = Name.Text
                };

                infoDAO.Add(info);
            }
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Name.Text) && !string.IsNullOrWhiteSpace(Id.Text))
            {
                int id;

                if (int.TryParse(Id.Text, out id))
                {
                    DateTime dateTime;

                    if (CreationDate.SelectedDate == null)
                    {
                        return;
                    }
                    else
                    {
                        dateTime = (DateTime)CreationDate.SelectedDate;
                    }

                    var info = new Info()
                    {
                        CreatonDate = dateTime,
                        Name = Name.Text
                    };

                    infoDAO.Update(id, info);
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
                    infoDAO.Delete(id);
                }
            }
        }
    }
}
