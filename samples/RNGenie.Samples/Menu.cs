namespace RNGenie.Samples
{
    internal class ConsoleMenu
    {
        public static void Run(string header, string subtitle, IReadOnlyDictionary<string, (string Title, Action Run)> items)
        {
            var keys = items.Keys.ToList();
            int idx = 0;

            while (true)
            {
                Console.Clear();

                var oldFg = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"== {header} ==");
                Console.ForegroundColor = oldFg;

                Console.WriteLine(subtitle);
                Console.WriteLine();

                for (int i = 0; i < keys.Count; i++)
                {
                    var key = keys[i];
                    var (title, _) = items[key];

                    if (i == idx)
                    {
                        var fg = Console.ForegroundColor;
                        var bg = Console.BackgroundColor;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.DarkCyan;
                        Console.WriteLine($" {i + 1}. {title} ({key})");
                        Console.ForegroundColor = fg;
                        Console.BackgroundColor = bg;
                    }
                    else
                    {
                        Console.WriteLine($" {i + 1}. {title} ({key})");
                    }
                }

                Console.WriteLine();
                Console.WriteLine(" [Enter] Run   [↑/↓] Move   [1-9] Jump   [Q] Quit");

                var k = Console.ReadKey(intercept: true);

                if (k.Key == ConsoleKey.Q) break;

                if (k.Key == ConsoleKey.Enter)
                {
                    Console.Clear();

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"== {header} ==");
                    Console.ForegroundColor = oldFg;

                    var (title, run) = items[keys[idx]];
                    Console.WriteLine(title);
                    Console.WriteLine();

                    try { run(); }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"\nError: {ex.Message}");
                        Console.ForegroundColor = oldFg;
                    }

                    Console.WriteLine("\n\nPress any key to return to the menu...");
                    Console.ReadKey(true);
                    continue;
                }

                if (k.Key == ConsoleKey.UpArrow) 
                    idx = (idx - 1 + keys.Count) % keys.Count;
                else if (k.Key == ConsoleKey.DownArrow)
                    idx = (idx + 1) % keys.Count;
                else if (TryDigit(k, out int n) && n >= 1 && n <= keys.Count)
                    idx = n - 1;
            }
        }

        private static bool TryDigit(ConsoleKeyInfo k, out int n)
        {
            n = -1;
            if (char.IsDigit(k.KeyChar)) 
            { 
                n = k.KeyChar - '0';
                return true;
            }
            if (k.Key is >= ConsoleKey.D0 and <= ConsoleKey.D9)
            {
                n = (int)k.Key - (int)ConsoleKey.D0;
                return true;
            }
            if (k.Key is >= ConsoleKey.NumPad0 and <= ConsoleKey.NumPad9)
            { 
                n = (int)k.Key - (int)ConsoleKey.NumPad0;
                return true;
            }
            return false;
        }
    }
}
