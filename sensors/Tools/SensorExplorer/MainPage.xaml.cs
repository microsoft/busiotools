// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#if(SENSORWRAPPERS)
using SensorWrappers;
#endif
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Sensors;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.Storage.Streams;
using Windows.Foundation.Diagnostics;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace SensorExplorer
{
    public sealed partial class MainPage : Page
    {
        public static MainPage Current;
        public FileLoggingSession LoggingSessionTests;
        public FileLoggingSession LoggingSessionView;
        public LoggingChannel LoggingChannelTests;
        public LoggingChannel LoggingChannelView;

        public MainPage()
        {
            InitializeComponent();
            Current = this;

            try
            {
                LoggingSessionTests = new FileLoggingSession("SensorExplorerLogTests");
                LoggingChannelTests = new LoggingChannel("SensorExplorerLogTests", null);
                LoggingSessionTests.AddLoggingChannel(LoggingChannelTests);

                LoggingSessionView = new FileLoggingSession("SensorExplorerLogView");
                LoggingChannelView = new LoggingChannel("SensorExplorerLogView", null);
                LoggingSessionView.AddLoggingChannel(LoggingChannelView);
            }
            catch (Exception e)
            {
                NotifyUser(e.Message, NotifyType.ErrorMessage);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Populate the scenario list from the SampleConfiguration.cs file
            ScenarioControl.ItemsSource = scenarios;
            if (Window.Current.Bounds.Width < 640)
            {
                ScenarioControl.SelectedIndex = -1;
            }
            else
            {
                ScenarioControl.SelectedIndex = 0;
            }
        }

        private void ScenarioControlSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Clear the status block when navigating scenarios.
            NotifyUser(string.Empty, NotifyType.StatusMessage);

            ListBox scenarioListBox = sender as ListBox;
            Scenario s = scenarioListBox.SelectedItem as Scenario;
            if (s != null)
            {
                ScenarioFrame.Navigate(s.ClassType);
                if (Window.Current.Bounds.Width < 640)
                {
                    Splitter.IsPaneOpen = false;
                }
            }
        }

        public List<Scenario> Scenarios
        {
            get { return scenarios; }
        }

        public void NotifyUser(string strMessage, NotifyType type)
        {
            // If called from the UI thread, then update immediately.
            // Otherwise, schedule a task on the UI thread to perform the update.
            if (Dispatcher.HasThreadAccess)
            {
                UpdateStatus(strMessage, type);
            }
            else
            {
                var task = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => UpdateStatus(strMessage, type));
            }
        }

        private void UpdateStatus(string strMessage, NotifyType type)
        {
            switch (type)
            {
                case NotifyType.StatusMessage:
                    StatusBorder.Background = new SolidColorBrush(Windows.UI.Colors.Green);
                    break;
                case NotifyType.ErrorMessage:
                    StatusBorder.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                    break;
            }

            StatusBlock.Text = strMessage;

            // Collapse the StatusBlock if it has no text to conserve real estate.
            StatusBorder.Visibility = (StatusBlock.Text != string.Empty) ? Visibility.Visible : Visibility.Collapsed;
            if (StatusBlock.Text != string.Empty)
            {
                StatusBorder.Visibility = Visibility.Visible;
                StatusPanel.Visibility = Visibility.Visible;
            }
            else
            {
                StatusBorder.Visibility = Visibility.Collapsed;
                StatusPanel.Visibility = Visibility.Collapsed;
            }
        }

        async void FooterClick(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri(((HyperlinkButton)sender).Tag.ToString()));
        }

        private async void SaveAllLogsClick(object sender, RoutedEventArgs e)
        {
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            savePicker.FileTypeChoices.Add("ETL", new List<string>() { ".etl" });
            savePicker.SuggestedFileName = "SensorExplorerLog";
            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                CachedFileManager.DeferUpdates(file);
                StorageFile logFileGenerated = await Current.LoggingSessionView.CloseAndSaveToFileAsync(); //returns NULL if the current log file is empty

                if (logFileGenerated != null)
                {
                    await logFileGenerated.CopyAndReplaceAsync(file);
                    FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                    if (status == FileUpdateStatus.Complete)
                    {
                        Current.NotifyUser("File " + file.Name + " was saved.", NotifyType.StatusMessage);
                    }
                    else if (status == FileUpdateStatus.CompleteAndRenamed)
                    {
                        Current.NotifyUser("File " + file.Name + " was renamed and saved.", NotifyType.StatusMessage);
                    }
                    else
                    {
                        Current.NotifyUser("File " + file.Name + " couldn't be saved.", NotifyType.ErrorMessage);
                    }
                }
                else
                {
                    Current.NotifyUser("The log is empty.", NotifyType.ErrorMessage);
                }
            }
            else
            {
                Current.NotifyUser("Operation cancelled.", NotifyType.ErrorMessage);
            }

            // start a new logging session
            Current.LoggingSessionView = new FileLoggingSession("SensorExplorerLogViewNew");
            Current.LoggingSessionView.AddLoggingChannel(Current.LoggingChannelView);
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            Splitter.IsPaneOpen = !Splitter.IsPaneOpen;
        }

        public void DisableScenarioSelect()
        {
            ScenarioControl.IsEnabled = false;
        }

        public void EnableScenarioSelect()
        {
            ScenarioControl.IsEnabled = true;
        }
    }

    public enum NotifyType
    {
        StatusMessage,
        ErrorMessage
    };

    public class ScenarioBindingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Scenario s = value as Scenario;
            return (MainPage.Current.Scenarios.IndexOf(s) + 1) + ") " + s.Title;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return true;
        }
    }
}