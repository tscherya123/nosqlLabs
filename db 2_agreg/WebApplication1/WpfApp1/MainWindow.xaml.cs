using System;
using System.Windows;
using WpfApp1.Configuration;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Loaded += Window_Loaded;
            Items.Click += Items_Click;
            Infos.Click += Infos_Click;
            Creators.Click += Creators_Click;
            Migration.Click += Migration_Click;

        }

        private void Window_Loaded(object sender, EventArgs args)
        {
            Switch.Click += Switch_Click;
            UpdateSwithText();
            Items_Click(sender, args);
        }

        private void Switch_Click(object sender, EventArgs args)
        {
            ApplicationConfiguration.IsMongo = !ApplicationConfiguration.IsMongo;
            UpdateSwithText();
            MainFrame.Refresh();
        }

        private void UpdateSwithText()
        {
            Switch.Content = ApplicationConfiguration.IsMongo ? "Mongo" : "SQL";
        }

        private void Infos_Click(object sedner, EventArgs args)
        {
            MainFrame.Navigate(new Uri("Pages/InfoPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void Items_Click(object sedner, EventArgs args)
        {
            MainFrame.Navigate(new Uri("Pages/ItemPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void Creators_Click(object sedner, EventArgs args)
        {
            MainFrame.Navigate(new Uri("Pages/CreatorPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void Migration_Click(object sender, EventArgs args)
        {
            MainFrame.Navigate(new Uri("Pages/MigrationPage.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}
