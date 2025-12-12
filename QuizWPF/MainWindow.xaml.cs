using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace QuizWPF
{
    public partial class MainWindow : Window
    {
        private Quiz<Pytanie<Odpowiedz>, Odpowiedz> quiz;
        private int aktualnyIndeksPytania = 0;
        private int punkty = 0;

        public MainWindow()
        {
            InitializeComponent();
            PrzygotujQuiz();
            WyswietlPytanie();
        }

        private void PrzygotujQuiz()
        {
            quiz = new Quiz<Pytanie<Odpowiedz>, Odpowiedz>();

            var pytanie1 = new Pytanie<Odpowiedz>("Jaka jest stolica Polski?");
            pytanie1.DodajOdpowiedz(new Odpowiedz("Kraków", false));
            pytanie1.DodajOdpowiedz(new Odpowiedz("Warszawa", true));
            pytanie1.DodajOdpowiedz(new Odpowiedz("Gdańsk", false));
            pytanie1.DodajOdpowiedz(new Odpowiedz("Wrocław", false));
            quiz.DodajPytanie(pytanie1);

            var pytanie2 = new Pytanie<Odpowiedz>("Ile wynosi 5 + 7?");
            pytanie2.DodajOdpowiedz(new Odpowiedz("10", false));
            pytanie2.DodajOdpowiedz(new Odpowiedz("11", false));
            pytanie2.DodajOdpowiedz(new Odpowiedz("12", true));
            pytanie2.DodajOdpowiedz(new Odpowiedz("13", false));
            quiz.DodajPytanie(pytanie2);

            var pytanie3 = new Pytanie<Odpowiedz>("Który język programowania jest używany w tym quizie?");
            pytanie3.DodajOdpowiedz(new Odpowiedz("Python", false));
            pytanie3.DodajOdpowiedz(new Odpowiedz("Java", false));
            pytanie3.DodajOdpowiedz(new Odpowiedz("C#", true));
            pytanie3.DodajOdpowiedz(new Odpowiedz("JavaScript", false));
            quiz.DodajPytanie(pytanie3);

            var pytanie4 = new Pytanie<Odpowiedz>("W którym roku rozpoczęła się II Wojna Światowa?");
            pytanie4.DodajOdpowiedz(new Odpowiedz("1914", false));
            pytanie4.DodajOdpowiedz(new Odpowiedz("1939", true));
            pytanie4.DodajOdpowiedz(new Odpowiedz("1945", false));
            pytanie4.DodajOdpowiedz(new Odpowiedz("1918", false));
            quiz.DodajPytanie(pytanie4);
        }

        private void WyswietlPytanie()
        {
            var pytania = quiz.PobierzPytania();

            if (aktualnyIndeksPytania >= pytania.Count)
            {
                PokazWynik();
                return;
            }

            var aktualnePytanie = pytania[aktualnyIndeksPytania];

            lblNumerPytania.Text = $"Pytanie {aktualnyIndeksPytania + 1} z {pytania.Count}";
            lblPytanie.Text = aktualnePytanie.Tresc;

            stackOdpowiedzi.Children.Clear();

            for (int i = 0; i < aktualnePytanie.Odpowiedzi.Count; i++)
            {
                var button = new Button
                {
                    Content = aktualnePytanie.Odpowiedzi[i].Tresc,
                    Height = 50,
                    Margin = new Thickness(0, 5, 0, 5),
                    FontSize = 16,
                    Tag = i + 1,
                    Background = new SolidColorBrush(Color.FromRgb(70, 130, 180)),
                    Foreground = Brushes.White,
                    BorderThickness = new Thickness(0),
                    Cursor = System.Windows.Input.Cursors.Hand
                };

                button.Click += BtnOdpowiedz_Click;
                stackOdpowiedzi.Children.Add(button);
            }

            btnNastepne.Visibility = Visibility.Collapsed;
        }

        private void BtnOdpowiedz_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            int wybranaOdpowiedz = (int)button.Tag;

            var aktualnePytanie = quiz.PobierzPytania()[aktualnyIndeksPytania];
            bool czyPoprawna = aktualnePytanie.SprawdzOdpowiedz(wybranaOdpowiedz);

            foreach (Button btn in stackOdpowiedzi.Children)
            {
                btn.IsEnabled = false;
                int numerOdpowiedzi = (int)btn.Tag;

                if (aktualnePytanie.Odpowiedzi[numerOdpowiedzi - 1].CzyPoprawna)
                {
                    btn.Background = new SolidColorBrush(Color.FromRgb(46, 204, 113));
                }
                else if (btn == button && !czyPoprawna)
                {
                    btn.Background = new SolidColorBrush(Color.FromRgb(231, 76, 60));
                }
            }

            if (czyPoprawna)
            {
                punkty++;
                lblStatus.Text = "✓ Poprawna odpowiedź!";
                lblStatus.Foreground = new SolidColorBrush(Color.FromRgb(46, 204, 113));
            }
            else
            {
                lblStatus.Text = "✗ Błędna odpowiedź!";
                lblStatus.Foreground = new SolidColorBrush(Color.FromRgb(231, 76, 60));
            }

            btnNastepne.Visibility = Visibility.Visible;
        }

        private void BtnNastepne_Click(object sender, RoutedEventArgs e)
        {
            aktualnyIndeksPytania++;
            lblStatus.Text = "";
            WyswietlPytanie();
        }

        private void PokazWynik()
        {
            lblNumerPytania.Text = "Quiz zakończony!";
            lblPytanie.Text = "";
            stackOdpowiedzi.Children.Clear();
            btnNastepne.Visibility = Visibility.Collapsed;

            int liczbaPytan = quiz.PobierzPytania().Count;
            double procent = (double)punkty / liczbaPytan * 100;

            var wynikPanel = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            var lblWynik = new TextBlock
            {
                Text = $"Twój wynik: {punkty}/{liczbaPytan}",
                FontSize = 32,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 20, 0, 10)
            };

            var lblProcent = new TextBlock
            {
                Text = $"{procent:F1}%",
                FontSize = 48,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                Foreground = procent >= 70 ? new SolidColorBrush(Color.FromRgb(46, 204, 113)) :
                            new SolidColorBrush(Color.FromRgb(241, 196, 15)),
                Margin = new Thickness(0, 0, 0, 10)
            };

            var lblKomentarz = new TextBlock
            {
                FontSize = 20,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 10, 0, 20)
            };

            if (procent == 100)
            {
                lblKomentarz.Text = "Doskonale! 🌟";
                lblKomentarz.Foreground = new SolidColorBrush(Color.FromRgb(46, 204, 113));
            }
            else if (procent >= 70)
            {
                lblKomentarz.Text = "Bardzo dobrze! 👍";
                lblKomentarz.Foreground = new SolidColorBrush(Color.FromRgb(52, 152, 219));
            }
            else if (procent >= 50)
            {
                lblKomentarz.Text = "Nieźle, ale można lepiej! 📚";
                lblKomentarz.Foreground = new SolidColorBrush(Color.FromRgb(241, 196, 15));
            }
            else
            {
                lblKomentarz.Text = "Trzeba jeszcze popracować! 💪";
                lblKomentarz.Foreground = new SolidColorBrush(Color.FromRgb(231, 76, 60));
            }

            var lblSeparator = new TextBlock
            {
                Height = 20
            };

            var btnRestart = new Button
            {
                Content = "Rozpocznij ponownie",
                Width = 200,
                Height = 40,
                FontSize = 16,
                Background = new SolidColorBrush(Color.FromRgb(70, 130, 180)),
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                Cursor = System.Windows.Input.Cursors.Hand
            };
            btnRestart.Click += BtnRestart_Click;

            wynikPanel.Children.Add(lblWynik);
            wynikPanel.Children.Add(lblProcent);
            wynikPanel.Children.Add(lblKomentarz);
            wynikPanel.Children.Add(lblSeparator);
            wynikPanel.Children.Add(btnRestart);

            stackOdpowiedzi.Children.Add(wynikPanel);
        }

        private void BtnRestart_Click(object sender, RoutedEventArgs e)
        {
            aktualnyIndeksPytania = 0;
            punkty = 0;
            lblStatus.Text = "";
            WyswietlPytanie();
        }
    }

    // Interfejsy i klasy z poprzedniego kodu
    interface IAnswer
    {
        string Tresc { get; set; }
        bool CzyPoprawna { get; set; }
    }

    interface IQuestion<T> where T : IAnswer
    {
        string Tresc { get; set; }
        List<T> Odpowiedzi { get; set; }
        void DodajOdpowiedz(T odpowiedz);
        bool SprawdzOdpowiedz(int numer);
    }

    interface IQuiz<TQuestion, TAnswer>
        where TQuestion : IQuestion<TAnswer>
        where TAnswer : IAnswer
    {
        void DodajPytanie(TQuestion pytanie);
        List<TQuestion> PobierzPytania();
    }

    class Odpowiedz : IAnswer
    {
        public string Tresc { get; set; }
        public bool CzyPoprawna { get; set; }

        public Odpowiedz(string tresc, bool czyPoprawna)
        {
            Tresc = tresc;
            CzyPoprawna = czyPoprawna;
        }
    }

    class Pytanie<T> : IQuestion<T> where T : IAnswer
    {
        public string Tresc { get; set; }
        public List<T> Odpowiedzi { get; set; }

        public Pytanie(string tresc)
        {
            Tresc = tresc;
            Odpowiedzi = new List<T>();
        }

        public void DodajOdpowiedz(T odpowiedz)
        {
            Odpowiedzi.Add(odpowiedz);
        }

        public bool SprawdzOdpowiedz(int numer)
        {
            if (numer < 1 || numer > Odpowiedzi.Count)
                return false;

            return Odpowiedzi[numer - 1].CzyPoprawna;
        }
    }

    class Quiz<TQuestion, TAnswer> : IQuiz<TQuestion, TAnswer>
        where TQuestion : IQuestion<TAnswer>
        where TAnswer : IAnswer
    {
        private List<TQuestion> pytania = new List<TQuestion>();

        public void DodajPytanie(TQuestion pytanie)
        {
            pytania.Add(pytanie);
        }

        public List<TQuestion> PobierzPytania()
        {
            return pytania;
        }
    }
}