using System;
using System.IO;
using System.Xml.Linq;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace Counter
{
    public partial class MainPage : ContentPage
    {
        private DaneAplikacji dane = new DaneAplikacji();

        private string sciezka;

        public MainPage()
        {
            InitializeComponent();

            sciezka = Path.Combine(
                FileSystem.AppDataDirectory,
                "counter_dane.xml"
            );

            Wczytaj();
            Odswiez();
        }

        private void StalyPlus_Clicked(object sender, EventArgs e)
        {
            if (dane.Staly.Wartosc < int.MaxValue)
            {
                dane.Staly.Wartosc = dane.Staly.Wartosc + 1;

                Odswiez();
                Zapisz();
            }
        }

        private void StalyMinus_Clicked(object sender, EventArgs e)
        {
            if (dane.Staly.Wartosc > int.MinValue)
            {
                dane.Staly.Wartosc = dane.Staly.Wartosc - 1;

                Odswiez();
                Zapisz();
            }
        }

        private void StalyReset_Clicked(object sender, EventArgs e)
        {
            dane.Staly.Wartosc = dane.Staly.Poczatkowa;

            Odswiez();
            Zapisz();
        }

        private void StalyKolor_Changed(object sender, EventArgs e)
        {
            if (StalyKolorPicker.SelectedIndex >= 0)
            {
                dane.Staly.Kolor =
                    PobierzKolor(StalyKolorPicker.SelectedIndex);

                Odswiez();
                Zapisz();
            }
        }

        private void Dodaj_Clicked(object sender, EventArgs e)
        {
            string nazwa = NazwaEntry.Text;

            if (nazwa == null)
            {
                KomunikatLabel.Text = "Wpisz nazwę.";
                return;
            }

            nazwa = nazwa.Trim();

            if (nazwa == "")
            {
                KomunikatLabel.Text = "Wpisz nazwę.";
                return;
            }

            int poczatkowa;

            try
            {
                poczatkowa = int.Parse(PoczatkowaEntry.Text);
            }
            catch (Exception)
            {
                KomunikatLabel.Text = "Wpisz liczbę całkowitą.";
                return;
            }

            Licznik nowy = new Licznik();

            nowy.Nazwa = nazwa;
            nowy.Poczatkowa = poczatkowa;
            nowy.Wartosc = poczatkowa;
            nowy.Kolor = PobierzKolor(KolorPicker.SelectedIndex);

            dane.Dodatkowe.Add(nowy);

            Odswiez();
            Zapisz();

            NazwaEntry.Text = "";
            PoczatkowaEntry.Text = "0";
        }

        private void Plus_Clicked(object sender, EventArgs e)
        {
            Button przycisk = sender as Button;

            if (przycisk != null)
            {
                Licznik licznik = przycisk.BindingContext as Licznik;

                if (licznik != null)
                {
                    if (licznik.Wartosc < int.MaxValue)
                    {
                        licznik.Wartosc = licznik.Wartosc + 1;

                        Odswiez();
                        Zapisz();
                    }
                }
            }
        }

        private void Minus_Clicked(object sender, EventArgs e)
        {
            Button przycisk = sender as Button;

            if (przycisk != null)
            {
                Licznik licznik = przycisk.BindingContext as Licznik;

                if (licznik != null)
                {
                    if (licznik.Wartosc > int.MinValue)
                    {
                        licznik.Wartosc = licznik.Wartosc - 1;

                        Odswiez();
                        Zapisz();
                    }
                }
            }
        }

        private void Reset_Clicked(object sender, EventArgs e)
        {
            Button przycisk = sender as Button;

            if (przycisk != null)
            {
                Licznik licznik = przycisk.BindingContext as Licznik;

                if (licznik != null)
                {
                    licznik.Wartosc = licznik.Poczatkowa;

                    Odswiez();
                    Zapisz();
                }
            }
        }

        private void Usun_Clicked(object sender, EventArgs e)
        {
            Button przycisk = sender as Button;

            if (przycisk != null)
            {
                Licznik licznik = przycisk.BindingContext as Licznik;

                if (licznik != null)
                {
                    dane.Dodatkowe.Remove(licznik);

                    Odswiez();
                    Zapisz();
                }
            }
        }

        private string PobierzKolor(int numer)
        {
            if (numer == 1)
            {
                return "#90EE90";
            }
            else if (numer == 2)
            {
                return "#FFFF99";
            }
            else if (numer == 3)
            {
                return "#FFB6C1";
            }
            else
            {
                return "#ADD8E6";
            }
        }

        private void Odswiez()
        {
            StalyWartoscLabel.Text = dane.Staly.Wartosc.ToString();
            StalyBorder.BackgroundColor = dane.Staly.Tlo;

            ListaLicznikow.ItemsSource = null;
            ListaLicznikow.ItemsSource = dane.Dodatkowe;
        }

        private XElement LicznikNaXml(Licznik licznik)
        {
            XElement element = new XElement("Licznik");

            XElement nazwa = new XElement("Nazwa", licznik.Nazwa);

            XElement wartosc = new XElement("Wartosc", licznik.Wartosc);

            XElement poczatkowa =
                new XElement("Poczatkowa", licznik.Poczatkowa);

            XElement kolor = new XElement("Kolor", licznik.Kolor);

            element.Add(nazwa);
            element.Add(wartosc);
            element.Add(poczatkowa);
            element.Add(kolor);

            return element;
        }

        private Licznik LicznikZXml(XElement element)
        {
            Licznik licznik = new Licznik();

            XElement nazwa = element.Element("Nazwa");
            XElement wartosc = element.Element("Wartosc");
            XElement poczatkowa = element.Element("Poczatkowa");
            XElement kolor = element.Element("Kolor");

            licznik.Nazwa = nazwa.Value;
            licznik.Wartosc = int.Parse(wartosc.Value);
            licznik.Poczatkowa = int.Parse(poczatkowa.Value);
            licznik.Kolor = kolor.Value;

            return licznik;
        }

        private void Zapisz_Clicked(object sender, EventArgs e)
        {
            Zapisz();

            System.Diagnostics.Process.Start(
                "explorer.exe",
                FileSystem.AppDataDirectory
            );
        }

        private void Zapisz()
        {
            try
            {
                XDocument dokument = new XDocument();

                XElement counter = new XElement("Counter");

                XElement staly = new XElement("Staly");

                XElement stalyLicznik = LicznikNaXml(dane.Staly);

                staly.Add(stalyLicznik);

                XElement dodatkowe = new XElement("Dodatkowe");

                foreach (Licznik licznik in dane.Dodatkowe)
                {
                    XElement element = LicznikNaXml(licznik);

                    dodatkowe.Add(element);
                }

                counter.Add(staly);
                counter.Add(dodatkowe);

                dokument.Add(counter);

                dokument.Save(sciezka);

                KomunikatLabel.Text = "Zapisano do XML.";
            }
            catch (Exception)
            {
                KomunikatLabel.Text = "Błąd zapisu XML.";
            }
        }

        private void Wczytaj()
        {
            bool istnieje = File.Exists(sciezka);

            if (istnieje == true)
            {
                try
                {
                    XDocument dokument = XDocument.Load(sciezka);

                    XElement counter = dokument.Element("Counter");

                    DaneAplikacji wczytane = new DaneAplikacji();

                    XElement staly = counter.Element("Staly");

                    XElement stalyLicznik = staly.Element("Licznik");

                    wczytane.Staly = LicznikZXml(stalyLicznik);

                    XElement dodatkowe = counter.Element("Dodatkowe");

                    foreach (XElement element in dodatkowe.Elements("Licznik"))
                    {
                        Licznik licznik = LicznikZXml(element);

                        wczytane.Dodatkowe.Add(licznik);
                    }

                    dane = wczytane;

                    KomunikatLabel.Text = "Wczytano z XML.";
                }
                catch (Exception)
                {
                    KomunikatLabel.Text = "Błąd odczytu XML.";
                }
            }

            StalyKolorPicker.SelectedIndexChanged -= StalyKolor_Changed;

            if (dane.Staly.Kolor == "#90EE90")
            {
                StalyKolorPicker.SelectedIndex = 1;
            }
            else if (dane.Staly.Kolor == "#FFFF99")
            {
                StalyKolorPicker.SelectedIndex = 2;
            }
            else if (dane.Staly.Kolor == "#FFB6C1")
            {
                StalyKolorPicker.SelectedIndex = 3;
            }
            else
            {
                StalyKolorPicker.SelectedIndex = 0;
            }

            StalyKolorPicker.SelectedIndexChanged += StalyKolor_Changed;
        }
    }
}