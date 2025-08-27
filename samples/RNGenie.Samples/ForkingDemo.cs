using RNGenie.Core.Sources;

namespace RNGenie.Samples
{
    /// <summary>
    /// Demonstration of Pcg32 RNG forking and streaming functionality.
    /// </summary>
    public static class ForkingDemo
    {
        /// <summary>
        /// Run the forking demo.
        /// </summary>
        public static void Run()
        {
            var rng = new Pcg32Source(100);

            Console.WriteLine("Main RNG (advance 1 draw to move state):");
            Console.WriteLine($"  main: {rng.NextInt(0, 100)}");

            // Same-stream fork from current state (branch the timeline).
            var sameFork = rng.Fork();

            // Different-stream fork from current state (independent immediately).
            var diffFork = rng.Fork(streamId: 10);

            // Independent stream from original seed (call-order stable).
            var seedStream = rng.NewStreamFromSeed(streamId: 20);

            Console.WriteLine("\nNext three draws from each source:");
            Console.WriteLine("  PCG32 uses the OLD state for output -> all forks match on the first draw.");
            Console.WriteLine("  sameFork stays equal if advanced in lockstep. diffFork diverges from draw #2.");
            Console.WriteLine("  seedStream is independent immediately (from the original seed).\n");

            Console.WriteLine("Draw #1:");
            var main1 = rng.NextInt(0, 100);
            var same1 = sameFork.NextInt(0, 100);
            var diff1 = diffFork.NextInt(0, 100);
            var seed1 = seedStream.NextInt(0, 100);

            Console.WriteLine($"  main: {main1}");
            Console.WriteLine($"  same: {same1} (matches main by design)");
            Console.WriteLine($"  diff: {diff1} (matches main on first draw, stream differs)");
            Console.WriteLine($"  seed: {seed1} (independent)");

            Console.WriteLine("\nDraw #2:");
            var main2 = rng.NextInt(0, 100);
            var same2 = sameFork.NextInt(0, 100);
            var diff2 = diffFork.NextInt(0, 100);
            var seed2 = seedStream.NextInt(0, 100);

            Console.WriteLine($"  main: {main2}");
            Console.WriteLine($"  same: {same2} (= main)");
            Console.WriteLine($"  diff: {diff2} (diverged)");
            Console.WriteLine($"  seed: {seed2} (independent)");

            Console.WriteLine("\nDraw #3:");
            var main3 = rng.NextInt(0, 100);
            var same3 = sameFork.NextInt(0, 100);
            var diff3 = diffFork.NextInt(0, 100);
            var seed3 = seedStream.NextInt(0, 100);

            Console.WriteLine($"  main: {main3}");
            Console.WriteLine($"  same: {same3} (= main)");
            Console.WriteLine($"  diff: {diff3}");
            Console.WriteLine($"  seed: {seed3}");

            // Robust proof (reduces chance coincidences in tiny ranges).
            Console.WriteLine("\nSanity check (large range, divergence within first 5 draws?):");
            bool diffDiverged = false, seedDiverged = false;
            for (int i = 0; i < 5; i++)
            {
                if (rng.NextInt(0, 1_000_000) != diffFork.NextInt(0, 1_000_000)) diffDiverged = true;
                if (rng.NextInt(0, 1_000_000) != seedStream.NextInt(0, 1_000_000)) seedDiverged = true;
            }

            Console.WriteLine($" diffFork diverged: {diffDiverged}");
            Console.WriteLine($" seedStream diverged: {seedDiverged}");

            Console.WriteLine("\nNotes:");
            Console.WriteLine("  • sameFork branches the timeline: first draw equals main’s next, if both advance equally, they remain equal.");
            Console.WriteLine("  • diffFork uses a different stream id: first draw matches (old state) then diverges.");
            Console.WriteLine("  • seedStream starts from the original seed: stable per stream id regardless of main’s progress.");
        }
    }
}
