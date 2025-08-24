namespace RNGenie.Samples
{
    internal class ConsoleMenu
    {
        /// <summary>
        /// Run the console demo selection menu.
        /// </summary>
        /// <param name="header"></param>
        /// <param name="subtitle"></param>
        /// <param name="items"></param>
        public static void Run(string header, string subtitle, IReadOnlyDictionary<string, (string Title, Action Run)> items)
        {
            var keys = items.Keys.ToList();
            int idx = 0;

            while (true)
            {
                Console.Clear();
                WriteHeader(header, subtitle);

                for (int i = 0; i < keys.Count; i++)
                {
                    var key = keys[i];
                    var (demoTitle, _) = items[key];

                    if (i == idx)
                    {
                        Invert(() => Console.WriteLine($" {i + 1}. {demoTitle} ({key})"));
                    }
                    else
                    {
                        Console.WriteLine($" {i + 1}. {demoTitle} ({key})");
                    }

                    Console.WriteLine();
                    Console.Write(" [Enter] Run   [↑/↓] Move   [1-9] Jump   [Q] Quit");

                    var keyInfo = Console.ReadKey(intercept: true);

                    if (keyInfo.Key is ConsoleKey.Q) break;

                    if (keyInfo.Key is ConsoleKey.Enter)
                    {
                        Console.Clear();
                        var selectedKey = keys[idx];
                        var (title, run) = items[selectedKey];

                        WriteHeader(header, title);
                        
                        try { run(); }
                        catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"\nError: {ex.Message}");
                            Console.ResetColor();
                        }

                        Console.WriteLine("\n\nPress any key to return to the menu...");
                        Console.ReadKey(true);
                        continue;
                    }

                    if (keyInfo.Key == ConsoleKey.UpArrow)
                    {
                        idx = (idx - 1 + keys.Count) % keys.Count;
                    }
                    else if (keyInfo.Key == ConsoleKey.DownArrow)
                    {
                        idx = (idx + 1) % keys.Count;
                    }
                    else if (isDigit(keyInfo.KeyChar, out int n) && n >= 1 && n <= keys.Count)
                    {
                        idx = n - 1;
                    }
                }
            }
        }

        private static void WriteHeader(string header, string subtitle)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"== {header} ==");
            Console.ResetColor();
            Console.WriteLine(subtitle);
            Console.WriteLine();
        }

        private static void Invert(Action write)
        {
            var fg = Console.ForegroundColor;
            var bg = Console.BackgroundColor;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.DarkCyan;
            write();
            Console.ForegroundColor = fg;
            Console.BackgroundColor = bg;
        }

        private static bool isDigit(char c, out int n)
        {
            n = c - '0';
            return n is >= 0 and <= 9;
        }
    }
}
