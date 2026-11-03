using System.Collections.Generic;
using Microsoft.Maui.Graphics;

namespace Counter
{
    public class CounterItem
    {
        public string Name { get; set; }
        public int Value { get; set; }
        public int InitialValue { get; set; }
        public string ColorCode { get; set; }

        public CounterItem()
        {
            Name = "";
            Value = 0;
            InitialValue = 0;
            ColorCode = "#ADD8E6";
        }

        public Color Background
        {
            get
            {
                return Color.FromArgb(ColorCode);
            }
        }
    }

    public class AppData
    {
        public CounterItem MainCounter { get; set; }

        public List<CounterItem> AdditionalCounters { get; set; }

        public AppData()
        {
            MainCounter = new CounterItem();
            MainCounter.Name = "Stały licznik";

            AdditionalCounters = new List<CounterItem>();
        }
    }
}