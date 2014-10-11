using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Threading;
using KingsAndQueensHat.Model;
using KingsAndQueensHat.Persistence;
using KingsAndQueensHat.ViewModel;

namespace KingsAndQueensHat.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public TournamentViewModel ViewModel { get; private set; }

        internal void SetStorageLocator(StorageLocator storageLocator)
        {
            try
            {
                ViewModel = new TournamentViewModel(storageLocator);
            }
            catch (InvalidPlayerListException e)
            {
                MessageBox.Show(string.Format("{0}\r\nSee README.rtf to help diagnose the error", e.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
                return;
            }
            catch (InvalidRoundException e)
            {
                MessageBox.Show(string.Format("{0}\r\nDelete the file or correct it", e.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
                return;
            }
            catch (Exception e)
            {
                // For better diagnostics
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }

            DataContext = ViewModel;
        }
    }
}
