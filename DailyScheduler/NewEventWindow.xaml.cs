// NewEventWindow.xaml.cs
using System;
using System.Windows;
using System.Windows.Controls;

namespace DailyScheduler
{
    public partial class NewEventWindow : Window
    {
        public ScheduleItem NewEvent { get; private set; }

        public NewEventWindow()
        {
            InitializeComponent();
            PopulateTimeComboBoxes();
            CategoryComboBox.SelectedIndex = 0;
            PriorityComboBox.SelectedIndex = 0;
            EnergyComboBox.SelectedIndex = 0;
        }

        private void PopulateTimeComboBoxes()
        {
            // Populate hours (12-hour format)
            for (int i = 1; i <= 12; i++)
            {
                StartHourComboBox.Items.Add($"{i:D2}:00 AM");
                StartHourComboBox.Items.Add($"{i:D2}:00 PM");
                EndHourComboBox.Items.Add($"{i:D2}:00 AM");
                EndHourComboBox.Items.Add($"{i:D2}:00 PM");
            }

            // Populate minutes
            string[] minutes = { "00", "30" };
            foreach (string minute in minutes)
            {
                StartMinuteComboBox.Items.Add(minute);
                EndMinuteComboBox.Items.Add(minute);
            }

            // Set default selections
            StartHourComboBox.SelectedIndex = 0;
            StartMinuteComboBox.SelectedIndex = 0;
            EndHourComboBox.SelectedIndex = 1; // Select next hour by default
            EndMinuteComboBox.SelectedIndex = 0;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
            {
                MessageBox.Show("Please enter an event title.", "Validation Error");
                return;
            }

            try
            {
                // Parse selected times
                string startHourStr = StartHourComboBox.SelectedItem?.ToString();
                string startMinuteStr = StartMinuteComboBox.SelectedItem?.ToString();
                string endHourStr = EndHourComboBox.SelectedItem?.ToString();
                string endMinuteStr = EndMinuteComboBox.SelectedItem?.ToString();

                // Default times if not provided
                DateTime startTime = DateTime.Now;
                DateTime endTime = DateTime.Now.AddHours(1);

                if (!string.IsNullOrEmpty(startHourStr) && !string.IsNullOrEmpty(startMinuteStr))
                {
                    startTime = DateTime.Parse(startHourStr.Replace(":00", ""));
                    startTime = startTime.AddMinutes(int.Parse(startMinuteStr));
                }

                if (!string.IsNullOrEmpty(endHourStr) && !string.IsNullOrEmpty(endMinuteStr))
                {
                    endTime = DateTime.Parse(endHourStr.Replace(":00", ""));
                    endTime = endTime.AddMinutes(int.Parse(endMinuteStr));
                }

                string category = ((ComboBoxItem)CategoryComboBox.SelectedItem).Content.ToString().ToLower();
                string priority = ((ComboBoxItem)PriorityComboBox.SelectedItem).Content.ToString();
                string energyLevel = ((ComboBoxItem)EnergyComboBox.SelectedItem).Content.ToString();

                NewEvent = new ScheduleItem
                {
                    Title = TitleTextBox.Text,
                    Time = $"{startTime.ToString("h:mm tt")} - {endTime.ToString("h:mm tt")}",
                    Details = DetailsTextBox.Text,
                    Top = 0,
                    Height = 0,
                    Width = double.NaN,
                    Background = ((MainWindow)Owner).GetCategoryBackground(category),
                    BorderBrush = ((MainWindow)Owner).GetCategoryBorder(category),
                    Category = category,
                    Priority = priority,
                    EnergyLevel = energyLevel
                };

                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating event: {ex.Message}", "Error");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}