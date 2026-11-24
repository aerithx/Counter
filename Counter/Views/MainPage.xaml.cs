using Counter.Models;

namespace Counter.Views
{
    public partial class MainPage : ContentPage
    {
        private CounterItem mainCounter = new CounterItem { Name = "Główny licznik", Value = 0, InitialValue = 0 };
        private List<CounterItem> additionalCounters = new List<CounterItem>();
        private string filePath = Path.Combine(FileSystem.AppDataDirectory, "counters.txt");

        public MainPage()
        {
            InitializeComponent();
            LoadData();
            RefreshUI();
        }

        private void OnMainPlusClicked(object? sender, EventArgs e)
        {
            mainCounter.Value++;
            RefreshUI();
            SaveData();
        }

        private void OnMainMinusClicked(object? sender, EventArgs e)
        {
            mainCounter.Value--;
            RefreshUI();
            SaveData();
        }

        private void OnMainResetClicked(object? sender, EventArgs e)
        {
            mainCounter.Value = mainCounter.InitialValue;
            RefreshUI();
            SaveData();
        }

        private async void OnAddClicked(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameEntry.Text))
            {
                await DisplayAlertAsync("Błąd", "Wpisz nazwę licznika!", "OK");
                return;
            }

            int startValue = 0;

            if (!string.IsNullOrWhiteSpace(StartValueEntry.Text))
            {
                if (!int.TryParse(StartValueEntry.Text, out startValue))
                {
                    await DisplayAlertAsync("Błąd", "Wartość początkowa musi być liczbą całkowitą!", "OK");
                    return;
                }
            }

            CounterItem newCounter = new CounterItem
            {
                Name = NameEntry.Text.Replace(";", ""),
                Value = startValue,
                InitialValue = startValue
            };

            additionalCounters.Add(newCounter);
            NameEntry.Text = "";
            StartValueEntry.Text = "";

            RefreshUI();
            SaveData();
        }

        private void OnPlusClicked(object? sender, EventArgs e)
        {
            Button btn = (Button)sender!;
            CounterItem item = (CounterItem)btn.BindingContext!;

            item.Value++;
            RefreshUI();
            SaveData();
        }

        private void OnMinusClicked(object? sender, EventArgs e)
        {
            Button btn = (Button)sender!;
            CounterItem item = (CounterItem)btn.BindingContext!;

            item.Value--;
            RefreshUI();
            SaveData();
        }

        private void OnResetClicked(object? sender, EventArgs e)
        {
            Button btn = (Button)sender!;
            CounterItem item = (CounterItem)btn.BindingContext!;

            item.Value = item.InitialValue;
            RefreshUI();
            SaveData();
        }

        private void OnDeleteClicked(object? sender, EventArgs e)
        {
            Button btn = (Button)sender!;
            CounterItem item = (CounterItem)btn.BindingContext!;

            additionalCounters.Remove(item);
            RefreshUI();
            SaveData();
        }
        private void RefreshUI()
        {
            MainValueLabel.Text = mainCounter.Value.ToString();

            CountersListView.ItemsSource = null;
            CountersListView.ItemsSource = additionalCounters;
        }

        private void SaveData()
        {
            List<string> lines = new List<string>();

            lines.Add("MAIN;" + mainCounter.Name + ";" + mainCounter.Value + ";" + mainCounter.InitialValue);

            foreach (CounterItem item in additionalCounters)
            {
                lines.Add("ITEM;" + item.Name + ";" + item.Value + ";" + item.InitialValue);
            }

            File.WriteAllLines(filePath, lines);
        }

        private void LoadData()
        {
            if (!File.Exists(filePath))
                return;

            string[] lines = File.ReadAllLines(filePath);
            additionalCounters.Clear();

            foreach (string line in lines)
            {
                string[] parts = line.Split(';');
                if (parts.Length == 4)
                {
                    CounterItem loaded = new CounterItem
                    {
                        Name = parts[1],
                        Value = int.Parse(parts[2]),
                        InitialValue = int.Parse(parts[3])
                    };

                    if (parts[0] == "MAIN")
                    {
                        mainCounter = loaded;
                    }
                    else if (parts[0] == "ITEM")
                    {
                        additionalCounters.Add(loaded);
                    }
                }
            }
        }
    }
}