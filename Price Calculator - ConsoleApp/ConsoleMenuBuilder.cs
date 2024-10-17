namespace QR.QRMenu
{
    class QRMenu
    {
        private static readonly double ErrorValue = -1;
        public static int OptionMenu(string Title, string Subtitle, string[] Options, bool errorOutput = true)
        {
            int number;

            do
            {
                Clear();

                if (!string.IsNullOrEmpty(Title))
                {
                    WriteLine(Title);
                    NewLine();
                }

                if (!string.IsNullOrEmpty(Subtitle))
                {
                    WriteLine(Subtitle);
                    NewLine();
                }

                for (int i = 1; i <= Options.Length; i++)
                {
                    WriteLine($"{i}) {Options[i - 1]}");
                }
                WriteLine($"{0}) Salir");

                number = AskIntegerMenu("", (0, Options.Length + 1));

            } while (number == ErrorValue || !errorOutput);

            Clear();

            return number;
        }

        public static bool ConfirmationMenu(string question)
        {
            WriteLine(question + " (Y/S para confirmar, cualquier otra tecla para cancelar)");
            NewLine();

            string input = ReadLine().ToLower();

            return input is "y" or "s";
        }

        public static void Exit()
        {
            Console.WriteLine("Presiona cualquier tecla para continuar...");
            Console.ReadKey();
            Environment.Exit(0);
        }

        public static string AskMenu(string question)
        {
            if (!string.IsNullOrEmpty(question))
                WriteLine(question);

            return ReadLine();
        }

        public static void ErrorMenu(string Title = "", string Subtitle = "", int errorCode = 0)
        {
            Clear();
            if (string.IsNullOrEmpty(Title) && string.IsNullOrEmpty(Subtitle) && errorCode == 0)
            {
                Title = "ERROR NO DEFINIDO";
                Subtitle = "#ERRNDF418";
                errorCode = 418;
            }

            if (!string.IsNullOrEmpty(Title))
            {
                WriteLine(Title);
                NewLine();
            }

            if (!string.IsNullOrEmpty(Subtitle))
            {
                WriteLine(Subtitle);
                NewLine();
            }

            if (errorCode != 0)
            {
                WriteLine($"https://http.cat/{errorCode}");
                NewLine();
            }

            Console.WriteLine("Presiona cualquier tecla para continuar...");
            Console.ReadKey();
            Clear();
        }

        public static int AskIntegerMenu(string question, bool errorOutput = true)
        {
            return AskIntegerMenu(question, (double.NegativeInfinity, double.PositiveInfinity), errorOutput);
        }

        public static int AskIntegerMenu(string question, (double minor, double major) boundaries, bool errorOutput = true)
        {
            return (int)AskNumberMenu(question, (boundaries.minor, boundaries.major), errorOutput);
        }

        public static double AskNumberMenu(string question, bool errorOutput = true)
        {
            return AskNumberMenu(question, (double.NegativeInfinity, double.PositiveInfinity), errorOutput);
        }
        public static double AskNumberMenu(string question, (double minor, double major) boundaries, bool errorOutput = true)
        {
            string input;
            bool failed;
            double output;

            if (boundaries.major < boundaries.minor)
                (boundaries.major, boundaries.minor) = (boundaries.minor, boundaries.major);

            do
            {
                input = AskMenu(question);
                failed = !double.TryParse(input, out output) || output < boundaries.minor || output > boundaries.major;

                bool haveBoundaries = !double.IsInfinity(boundaries.minor) || !double.IsInfinity(boundaries.major);

                (string minor, string major) strBoundaries = (double.IsNegativeInfinity(boundaries.minor) ? "Not Def." : boundaries.minor.ToString(), double.IsPositiveInfinity(boundaries.major) ? "Not Def." : boundaries.major.ToString());

                if (failed)
                    ErrorMenu("Error", $"Recuerda que debe ser un número{(haveBoundaries ? $" y debe estar dentro del rango válido [mínimo: {strBoundaries.minor}, máximo:{strBoundaries.major}]" : "")}");

            } while (failed && !errorOutput);

            if (failed)
                return ErrorValue;

            return output;
        }

        public static void Clear()
        {
            Console.Clear();
        }

        public static void Write(object obj)
        {
            Console.Write(obj);
        }
        public static void WriteLine(object obj)
        {
            Console.WriteLine(obj);
        }
        public static string ReadLine()
        {
            return Console.ReadLine() ?? string.Empty;
        }
        public static void NewLine(int numberOfLines = 1)
        {
            for (int i = 0; i < numberOfLines; i++)
                Console.WriteLine();
        }
    }
}