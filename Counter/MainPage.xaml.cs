using System;
using System.IO;
using System.Xml.Linq;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace Counter
{
    public partial class MainPage : ContentPage
    {
        private AppData appData = new AppData();

        private string filePath;

        public MainPage()
        {
            InitializeComponent();

            filePath = Path.Combine(
                FileSystem.AppDataDirectory,
                "counter_data.xml"
            );

            LoadData();
            RefreshUI();
        }

        private void MainPlus_Clicked(object sender, EventArgs e)
        {
            if (appData.MainCounter.Value < int.MaxValue)
            {
                appData.MainCounter.Value = appData.MainCounter.Value + 1;

                RefreshUI();
                SaveData();
            }
        }

        private void MainMinus_Clicked(object sender, EventArgs e)
        {
            if (appData.MainCounter.Value > int.MinValue)
            {
                appData.MainCounter.Value = appData.MainCounter.Value - 1;

                RefreshUI();
                SaveData();
            }
        }

        private void MainReset_Clicked(object sender, EventArgs e)
        {
            appData.MainCounter.Value = appData.MainCounter.InitialValue;

            RefreshUI();
            SaveData();
        }

        private void MainColor_Changed(object sender, EventArgs e)
        {
            if (MainColorPicker.SelectedIndex >= 0)
            {
                appData.MainCounter.ColorCode =
                    GetColorCode(MainColorPicker.SelectedIndex);

                RefreshUI();
                SaveData();
            }
        }

        private void Add_Clicked(object sender, EventArgs e)
        {
            string name = NameEntry.Text;

            if (name == null)
            {
                MessageLabel.Text = "Wpisz nazwę:";
                return;
            }

            name = name.Trim();

            if (name == "")
            {
                MessageLabel.Text = "Wpisz nazwę:";
                return;
            }

            int initial;

            try
            {
                initial = int.Parse(InitialValueEntry.Text);
            }
            catch (Exception)
            {
                MessageLabel.Text = "Wpisz liczbę całkowitą:";
                return;
            }

            CounterItem newItem = new CounterItem();

            newItem.Name = name;
            newItem.InitialValue = initial;
            newItem.Value = initial;
            newItem.ColorCode = GetColorCode(ColorPicker.SelectedIndex);

            appData.AdditionalCounters.Add(newItem);

            RefreshUI();
            SaveData();

            NameEntry.Text = "";
            InitialValueEntry.Text = "0";
        }

        private void Plus_Clicked(object sender, EventArgs e)
        {
            Button button = sender as Button;

            if (button != null)
            {
                CounterItem counter = button.BindingContext as CounterItem;

                if (counter != null)
                {
                    if (counter.Value < int.MaxValue)
                    {
                        counter.Value = counter.Value + 1;

                        RefreshUI();
                        SaveData();
                    }
                }
            }
        }

        private void Minus_Clicked(object sender, EventArgs e)
        {
            Button button = sender as Button;

            if (button != null)
            {
                CounterItem counter = button.BindingContext as CounterItem;

                if (counter != null)
                {
                    if (counter.Value > int.MinValue)
                    {
                        counter.Value = counter.Value - 1;

                        RefreshUI();
                        SaveData();
                    }
                }
            }
        }

        private void Reset_Clicked(object sender, EventArgs e)
        {
            Button button = sender as Button;

            if (button != null)
            {
                CounterItem counter = button.BindingContext as CounterItem;

                if (counter != null)
                {
                    counter.Value = counter.InitialValue;

                    RefreshUI();
                    SaveData();
                }
            }
        }

        private void Delete_Clicked(object sender, EventArgs e)
        {
            Button button = sender as Button;

            if (button != null)
            {
                CounterItem counter = button.BindingContext as CounterItem;

                if (counter != null)
                {
                    appData.AdditionalCounters.Remove(counter);

                    RefreshUI();
                    SaveData();
                }
            }
        }

        private string GetColorCode(int index)
        {
            if (index == 1)
            {
                return "#90EE90";
            }
            else if (index == 2)
            {
                return "#FFFF99";
            }
            else if (index == 3)
            {
                return "#FFB6C1";
            }
            else
            {
                return "#ADD8E6";
            }
        }

        private void RefreshUI()
        {
            MainValueLabel.Text = appData.MainCounter.Value.ToString();
            MainBorder.BackgroundColor = appData.MainCounter.Background;

            CountersList.ItemsSource = null;
            CountersList.ItemsSource = appData.AdditionalCounters;
        }

        private XElement CounterToXml(CounterItem counter)
        {
            XElement element = new XElement("CounterItem");

            XElement name = new XElement("Name", counter.Name);

            XElement value = new XElement("Value", counter.Value);

            XElement initial =
                new XElement("InitialValue", counter.InitialValue);

            XElement color = new XElement("ColorCode", counter.ColorCode);

            element.Add(name);
            element.Add(value);
            element.Add(initial);
            element.Add(color);

            return element;
        }

        private CounterItem CounterFromXml(XElement element)
        {
            CounterItem counter = new CounterItem();

            XElement name = element.Element("Name");
            XElement value = element.Element("Value");
            XElement initial = element.Element("InitialValue");
            XElement color = element.Element("ColorCode");

            counter.Name = name.Value;
            counter.Value = int.Parse(value.Value);
            counter.InitialValue = int.Parse(initial.Value);
            counter.ColorCode = color.Value;

            return counter;
        }

        private void Save_Clicked(object sender, EventArgs e)
        {
            SaveData();

            System.Diagnostics.Process.Start(
                "explorer.exe",
                FileSystem.AppDataDirectory
            );
        }

        private void SaveData()
        {
            try
            {
                XDocument document = new XDocument();

                XElement root = new XElement("AppData");

                XElement mainGroup = new XElement("MainCounter");

                XElement mainCounter = CounterToXml(appData.MainCounter);

                mainGroup.Add(mainCounter);

                XElement additionalGroup = new XElement("AdditionalCounters");

                foreach (CounterItem counter in appData.AdditionalCounters)
                {
                    XElement element = CounterToXml(counter);

                    additionalGroup.Add(element);
                }

                root.Add(mainGroup);
                root.Add(additionalGroup);

                document.Add(root);

                document.Save(filePath);

                MessageLabel.Text = "Zapisano do XML.";
            }
            catch (Exception)
            {
                MessageLabel.Text = "Błąd zapisu XML.";
            }
        }

        private void LoadData()
        {
            bool exists = File.Exists(filePath);

            if (exists == true)
            {
                try
                {
                    XDocument document = XDocument.Load(filePath);

                    XElement root = document.Element("AppData");

                    AppData loadedData = new AppData();

                    XElement mainGroup = root.Element("MainCounter");

                    XElement mainCounter = mainGroup.Element("CounterItem");

                    loadedData.MainCounter = CounterFromXml(mainCounter);

                    XElement additionalGroup = root.Element("AdditionalCounters");

                    foreach (XElement element in additionalGroup.Elements("CounterItem"))
                    {
                        CounterItem counter = CounterFromXml(element);

                        loadedData.AdditionalCounters.Add(counter);
                    }

                    appData = loadedData;

                    MessageLabel.Text = "Wczytano z XML.";
                }
                catch (Exception)
                {
                    MessageLabel.Text = "Błąd odczytu XML.";
                }
            }

            MainColorPicker.SelectedIndexChanged -= MainColor_Changed;

            if (appData.MainCounter.ColorCode == "#90EE90")
            {
                MainColorPicker.SelectedIndex = 1;
            }
            else if (appData.MainCounter.ColorCode == "#FFFF99")
            {
                MainColorPicker.SelectedIndex = 2;
            }
            else if (appData.MainCounter.ColorCode == "#FFB6C1")
            {
                MainColorPicker.SelectedIndex = 3;
            }
            else
            {
                MainColorPicker.SelectedIndex = 0;
            }

            MainColorPicker.SelectedIndexChanged += MainColor_Changed;
        }
    }
}