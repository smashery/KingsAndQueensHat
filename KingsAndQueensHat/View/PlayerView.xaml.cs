using KingsAndQueensHat.Persistence;
using KingsAndQueensHat.ViewModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KingsAndQueensHat.View
{
    /// <summary>
    /// Interaction logic for PlayerView.xaml
    /// </summary>
    public partial class PlayerView : UserControl
    {
        public PlayerView()
        {
            InitializeComponent();
        }

        private PlayerViewModel ViewModel { get { return DataContext as PlayerViewModel; } }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            var result = dlg.ShowDialog();
            if (result.HasValue && result.Value)
            {
                try
                {
                    ViewModel.ImportFrom(dlg.FileName);
                }
                catch (InvalidPlayerListException ex)
                {
                    MessageBox.Show(ex.Message, "Error loading", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }

        private void AddPlayerButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.AddPlayer(() =>
                {
                    return MessageBox.Show("Add player to current round?", "New Player", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
                },
                (errorStr) =>
                {
                    MessageBox.Show(errorStr, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
        }
    }
}
