// MainWindow.xaml.cs
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace DailyScheduler
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<TimeSlot> timeSlots = new ObservableCollection<TimeSlot>();
        private ObservableCollection<ScheduleItem> scheduleItems = new ObservableCollection<ScheduleItem>();
        private DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();
            InitializeSchedule();
            InitializeTimer();
        }

        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, e) => TimeDisplay.Text = DateTime.Now.ToString("h:mm:ss tt");
            timer.Start();
        }

        private void InitializeSchedule()
        {
            // Initialize time slots
            for (int i = 0; i < 48; i++)
            {
                int hour = i / 2;
                int minute = (i % 2) * 30;
                timeSlots.Add(new TimeSlot
                {
                    TimeDisplay = minute == 0 ? DateTime.Today.AddHours(hour).ToString("h:mm tt") : ""
                });
            }

            // Set ItemsSource for time column
            TimeColumn.ItemsSource = timeSlots;
            TimeSlots.ItemsSource = timeSlots;

            // Add schedule items
            AddScheduleItems();

            // Set ItemsSource for schedule items
            ScheduleItems.ItemsSource = scheduleItems;
        }

        private void AddScheduleItems()
        {
            // Define schedule blocks
            var blocks = new[]
            {
                new { Start = new TimeSpan(22, 0, 0), End = new TimeSpan(6, 0, 0), Title = "Sleep", Category = "rest" },
                new { Start = new TimeSpan(6, 0, 0), End = new TimeSpan(7, 30, 0), Title = "Morning Family Routine", Category = "family" },
                new { Start = new TimeSpan(7, 30, 0), End = new TimeSpan(8, 0, 0), Title = "Commute to Work", Category = "transit" },
                new { Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 30, 0), Title = "Work Block", Category = "work" },
                new { Start = new TimeSpan(16, 30, 0), End = new TimeSpan(17, 0, 0), Title = "Evening Commute", Category = "transit" },
                new { Start = new TimeSpan(17, 0, 0), End = new TimeSpan(18, 0, 0), Title = "Family Dinner", Category = "family" },
                new { Start = new TimeSpan(18, 0, 0), End = new TimeSpan(19, 0, 0), Title = "Family Play Time", Category = "family" },
                new { Start = new TimeSpan(19, 0, 0), End = new TimeSpan(20, 0, 0), Title = "Kids Bedtime Routine", Category = "family" },
                new { Start = new TimeSpan(20, 0, 0), End = new TimeSpan(22, 0, 0), Title = "Quality Time with Wife", Category = "personal" }
            };

            foreach (var block in blocks)
            {
                double top = (block.Start.Hours * 60 + block.Start.Minutes) / 30.0 * 30;
                double height = ((block.End.Hours - block.Start.Hours) * 60 + (block.End.Minutes - block.Start.Minutes)) / 30.0 * 30;

                if (block.End < block.Start) // Overnight event
                {
                    height = ((24 * 60) - (block.Start.Hours * 60 + block.Start.Minutes) + (block.End.Hours * 60 + block.End.Minutes)) / 30.0 * 30;
                }

                var background = GetCategoryBackground(block.Category);
                var border = GetCategoryBorder(block.Category);

                scheduleItems.Add(new ScheduleItem
                {
                    Title = block.Title,
                    Time = $"{FormatTime(block.Start)} - {FormatTime(block.End)}",
                    Details = GetDetailsForTask(block.Title),
                    Top = top,
                    Height = height,
                    Width = double.NaN, // Stretch to fill
                    Background = background,
                    BorderBrush = border
                });
            }
        }

        private string GetDetailsForTask(string title)
        {
            return title switch
            {
                "Morning Family Routine" => "Kids, breakfast, school prep",
                "Work Block" => "Core work hours",
                "Family Dinner" => "Dinner preparation and family meal",
                "Family Play Time" => "Active engagement with kids",
                "Kids Bedtime Routine" => "Bath, stories, bedtime",
                "Quality Time with Wife" => "Couple time",
                _ => ""
            };
        }

        private string FormatTime(TimeSpan time)
        {
            return DateTime.Today.Add(time).ToString("h:mm tt");
        }

        public LinearGradientBrush GetCategoryBackground(string category)
        {
            Color startColor = Colors.White;
            Color endColor = category switch
            {
                "rest" => Color.FromRgb(238, 242, 255),
                "family" => Color.FromRgb(254, 242, 242),
                "transit" => Color.FromRgb(243, 244, 246),
                "work" => Color.FromRgb(239, 246, 255),
                "personal" => Color.FromRgb(245, 243, 255),
                _ => Color.FromRgb(243, 244, 246)
            };

            return new LinearGradientBrush(startColor, endColor, 90);
        }

        public SolidColorBrush GetCategoryBorder(string category)
        {
            return category switch
            {
                "rest" => new SolidColorBrush(Color.FromRgb(199, 210, 254)),
                "family" => new SolidColorBrush(Color.FromRgb(252, 165, 165)),
                "transit" => new SolidColorBrush(Color.FromRgb(209, 213, 219)),
                "work" => new SolidColorBrush(Color.FromRgb(147, 197, 253)),
                "personal" => new SolidColorBrush(Color.FromRgb(216, 180, 254)),
                _ => new SolidColorBrush(Color.FromRgb(209, 213, 219))
            };
        }
        private void NewEvent_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new NewEventWindow { Owner = this };
            if (dialog.ShowDialog() == true)
            {
                scheduleItems.Add(dialog.NewEvent);
            }
        }


    }

    public class TimeSlot
    {
        public string TimeDisplay { get; set; }
    }

    public class ScheduleItem
    {
        public string Title { get; set; }
        public string Time { get; set; }
        public string Details { get; set; }
        public double Top { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public Brush Background { get; set; }
        public Brush BorderBrush { get; set; }
    }
}