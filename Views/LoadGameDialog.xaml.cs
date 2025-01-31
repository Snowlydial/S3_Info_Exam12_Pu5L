using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Exam12_Pu5L.Views
{
    public partial class LoadGameDialog : Window
    {
        public string SelectedSaveFile { get; private set; }

        public LoadGameDialog(List<string> saveFiles)
        {
            InitializeComponent();
            SaveFilesListBox.ItemsSource = saveFiles;
        }

        private void SaveFilesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Enable the Load button only when an item is selected
            LoadButton.IsEnabled = SaveFilesListBox.SelectedItem != null;
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            // Set the selected file and close the dialog with a positive result
            SelectedSaveFile = SaveFilesListBox.SelectedItem as string;
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Close the dialog with a negative result
            DialogResult = false;
            Close();
        }
    }
}
