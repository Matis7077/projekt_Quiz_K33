using System;
using System.Collections.Generic;

namespace Quiz_K33
{
    
    interface IAnswer
    {
        string Tresc { get; set; }
        bool CzyPoprawna { get; set; }
    }

    // Interfejs dla pytania z generykiem
    interface IQuestion<T> where T : IAnswer
    {
        string Tresc { get; set; }
        List<T> Odpowiedzi { get; set; }
        void DodajOdpowiedz(T odpowiedz);
        bool SprawdzOdpowiedz(int numer);
        void Wyswietl();
    }

    // Interfejs dla quizu z generykiem
    interface IQuiz<TQuestion, TAnswer>
        where TQuestion : IQuestion<TAnswer>
        where TAnswer : IAnswer
    {
        void DodajPytanie(TQuestion pytanie);
        void Rozpocznij();
        int PobierzPunkty();
        List<TQuestion> PobierzPytania();
    }

    // Implementacja odpowiedzi
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

    // Implementacja pytania z generykiem
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

        public void Wyswietl()
        {
            Console.WriteLine(Tresc);
            Console.WriteLine();

            for (int i = 0; i < Odpowiedzi.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {Odpowiedzi[i].Tresc}");
            }
            Console.WriteLine();
        }
    }

    // Implementacja quizu z generykami
    class Quiz<TQuestion, TAnswer> : IQuiz<TQuestion, TAnswer>
        where TQuestion : IQuestion<TAnswer>
        where TAnswer : IAnswer
    {
        private List<TQuestion> pytania = new List<TQuestion>();
        private int punkty = 0;

        public void DodajPytanie(TQuestion pytanie)
        {
            pytania.Add(pytanie);
        }

        public List<TQuestion> PobierzPytania()
        {
            return pytania;
        }

        public void Rozpocznij()
        {
            Console.Clear();
            Console.WriteLine("=== QUIZ - START ===\n");
            punkty = 0;

            for (int i = 0; i < pytania.Count; i++)
            {
                Console.WriteLine($"Pytanie {i + 1}/{pytania.Count}:");
                pytania[i].Wyswietl();

                int odpowiedz = PobierzOdpowiedz();

                if (pytania[i].SprawdzOdpowiedz(odpowiedz))
                {
                    Console.WriteLine("✓ Poprawna odpowiedź!\n");
                    punkty++;
                }
                else
                {
                    Console.WriteLine("✗ Błędna odpowiedź!\n");
                }

                Console.WriteLine("Naciśnij dowolny klawisz...");
                Console.ReadKey();
                Console.Clear();
            }

            PokazWynik();
        }

        public int PobierzPunkty()
        {
            return punkty;
        }

        private int PobierzOdpowiedz()
        {
            int odpowiedz;
            Console.Write("Twoja odpowiedź (numer): ");

            while (!int.TryParse(Console.ReadLine(), out odpowiedz))
            {
                Console.Write("Podaj prawidłowy numer: ");
            }

            return odpowiedz;
        }

        private void PokazWynik()
        {
            Console.WriteLine("=== KONIEC QUIZU ===\n");
            Console.WriteLine($"Twój wynik: {punkty}/{pytania.Count}");

            double procent = (double)punkty / pytania.Count * 100;
            Console.WriteLine($"Procent poprawnych: {procent:F1}%\n");

            if (procent == 100)
                Console.WriteLine("Doskonale! 🌟");
            else if (procent >= 70)
                Console.WriteLine("Bardzo dobrze! 👍");
            else if (procent >= 50)
                Console.WriteLine("Nieźle, ale można lepiej! 📚");
            else
                Console.WriteLine("Trzeba jeszcze popracować! 💪");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Tworzenie quizu z użyciem generyków
            var quiz = new Quiz<Pytanie<Odpowiedz>, Odpowiedz>();

            
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

          
            quiz.Rozpocznij();

            Console.WriteLine("\nNaciśnij dowolny klawisz, aby zakończyć...");
            Console.ReadKey();
        }
    }
}