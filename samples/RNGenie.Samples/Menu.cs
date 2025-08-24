namespace RNGenie.Samples
{
    internal class ConsoleMenu
    {
        public static void Run(
            string header,
            string subtitle,
            IReadOnlyDictionary<string, (string Title, Action Run)> items)
        {
            var keys = items.Keys.ToList();
            int idx = 0;

            Console.Clear();

            var oldFg = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"== {header} ==");
            Console.ForegroundColor = oldFg;

            Console.WriteLine(subtitle);
            Console.WriteLine();

            // Top row of menu list.
            int startTop = Console.CursorTop;

            while (true)
            {
                // Repaint, return to top and overwrite lines.
                Console.SetCursorPosition(0, startTop);

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
                        WritePadded($" {i + 1}. {title} ({key})");
                        Console.ForegroundColor = fg;
                        Console.BackgroundColor = bg;
                    }
                    else
                    {
                        WritePadded($" {i + 1}. {title} ({key})");
                    }
                }

                // Controls area (single instance, repainted every frame).
                Console.WriteLine(); // Spacer (padded to wipe leftovers).
                WritePadded(" [Enter] Run   [↑/↓] Move   [1-9] Jump   [Home/End] Ends   [PgUp/PgDn] ±5   [Q] Quit");

                // One extra blank line to ensure old content gets wiped.
                WritePadded("");

                // Input, one key per frame.
                var k = Console.ReadKey(intercept: true);

                if (k.Key == ConsoleKey.Q)
                    break;

                if (k.Key == ConsoleKey.Enter)
                {
                    // Run the selection on a clean screen, then restore header and startTop.
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

                    // Rebuild the static header and reset startTop.
                    Console.Clear();

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"== {header} ==");
                    Console.ForegroundColor = oldFg;

                    Console.WriteLine(subtitle);
                    Console.WriteLine();

                    startTop = Console.CursorTop;
                    continue;
                }

                switch (k.Key)
                {
                    case ConsoleKey.UpArrow:
                        idx = (idx - 1 + keys.Count) % keys.Count;
                        break;
                    case ConsoleKey.DownArrow:
                        idx = (idx + 1) % keys.Count;
                        break;
                    case ConsoleKey.Home:
                        idx = 0;
                        break;
                    case ConsoleKey.End:
                        idx = keys.Count - 1;
                        break;
                    case ConsoleKey.PageUp:
                        idx = Math.Max(0, idx - 5);
                        break;
                    case ConsoleKey.PageDown:
                        idx = Math.Min(keys.Count - 1, idx + 5);
                        break;
                    default:
                        if (TryDigit(k, out int n) && n >= 1 && n <= keys.Count)
                            idx = n - 1;
                        break;
                }
            }
        }

        // Write a line padded to the window width to fully overwrite previous content.
        private static void WritePadded(string text)
        {
            int w = Math.Max(1, Console.WindowWidth);
            // Ensure at least one space so the background fill looks clean.
            string line = (text ?? string.Empty).PadRight(w);
            Console.Write(line);
            // SetCursorPosition moves automatically when writing. Ensure next line.
            int x = 0;
            int y = Console.CursorTop + 1;
            // If the write wrapped (small console), we still want to go to new line start.
            if (y >= Console.BufferHeight) y = Console.BufferHeight - 1;
            Console.SetCursorPosition(x, y);
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
