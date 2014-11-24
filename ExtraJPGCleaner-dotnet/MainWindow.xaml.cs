﻿using System.IO;
using System.Windows;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Collections.Specialized;

namespace ExtraJPGCleaner
{
    using Models;
    using Helpers;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Interface implementations.

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Data model.

        string location;
        public string Location
        {
            get { return location; }
            set
            {
                location = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Location"));
                    PropertyChanged(this, new PropertyChangedEventArgs("IsLocationValid"));
                }
            }
        }

        string extensions;
        public string Extensions
        {
            get { return extensions; }
            set
            {
                extensions = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Extensions"));
                }
            }
        }

        public ObservableCollection<SearchResult> SelectedFiles { get; set; }

        public bool IsLocationValid
        {
            get
            {
                return Directory.Exists(Location);
            }
        }

        public bool IsFileSelected
        {
            get
            {
                return SelectedFiles.Count > 0;
            }
        }

        SearchResults results;
        public SearchResults Results
        {
            get { return results; }
            private set
            {
                results = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Results"));
                }
            }
        }

        #endregion

        #region Event handling functions

        private void browseLocation()
        {
            var dlg = new System.Windows.Forms.FolderBrowserDialog();
            dlg.ShowNewFolderButton = false;
            dlg.SelectedPath = Location;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Location = dlg.SelectedPath;
            }
        }

        private void search()
        {
            var files = FileOperator.Search(Location, Extensions);
            Results.BasePath = Location;
            Results.ResetItems(files);
        }

        private void delete()
        {
            var remains = new List<SearchResult>();
            foreach (var file in SelectedFiles)
            {
                var result = FileOperator.Delete(file.FullPath);
                if (!result.IsSuccessful)
                {
                    file.IsFailed = true;
                    file.FailReason = result.RawException.Message;
                    remains.Add(file);
                }
            }
            Results.ResetItems(remains);
        }

        private void updateSelectionList(System.Collections.IList files)
        {
            SelectedFiles.Clear();
            foreach (SearchResult f in files)
            {
                SelectedFiles.Add(f);
            }
        }

        #endregion

        #region Event handlers

        private void buttonBrowseLocation_Click(object sender, RoutedEventArgs e)
        {
            browseLocation();
        }

        private void buttonSearch_Click(object sender, RoutedEventArgs e)
        {
            search();
        }

        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            delete();
        }

        private void FileList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            updateSelectionList(((DataGrid)sender).SelectedItems);
        }

        private void resultsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Results"));
            }
        }

        private void selectedFilesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            PropertyChanged(this, new PropertyChangedEventArgs("IsFileSelected"));
        }

        #endregion

        public MainWindow()
        {
            InitializeComponent();
            Location = "D:\\tmp";
            Extensions = "ARW,RW2,DNG";
            DataContext = this;
            Results = new SearchResults();
            SelectedFiles = new ObservableCollection<SearchResult>();
            SelectedFiles.CollectionChanged += selectedFilesChanged;
            Results.CollectionChanged += resultsChanged;
        }
    }
}
