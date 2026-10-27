using System.Collections.Generic;
using Microsoft.Maui.Graphics;

namespace Counter
{
    public class Licznik
    {
        public string Nazwa { get; set; }
        public int Wartosc { get; set; }
        public int Poczatkowa { get; set; }
        public string Kolor { get; set; }

        public Licznik()
        {
            Nazwa = "";
            Wartosc = 0;
            Poczatkowa = 0;
            Kolor = "#ADD8E6";
        }

        public Color Tlo
        {
            get
            {
                return Color.FromArgb(Kolor);
            }
        }
    }

    public class DaneAplikacji
    {
        public Licznik Staly { get; set; }

        public List<Licznik> Dodatkowe { get; set; }

        public DaneAplikacji()
        {
            Staly = new Licznik();
            Staly.Nazwa = "Stały licznik";

            Dodatkowe = new List<Licznik>();
        }
    }
}