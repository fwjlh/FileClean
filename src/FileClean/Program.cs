using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace FileClean
{
    internal static class Program
    {
        private static readonly Regex WeightRegex = new Regex(
            @"(\d+),(\d{3})\s*kg\b",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        private static int Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: FileClean.exe <input-folder> <output-folder>");
                return 1;
            }

            string inputFolder = Path.GetFullPath(args[0]);
            string outputFolder = Path.GetFullPath(args[1]);

            if (!Directory.Exists(inputFolder))
            {
                Console.Error.WriteLine("Input folder does not exist: " + inputFolder);
                return 1;
            }

            if (string.Equals(inputFolder.TrimEnd(Path.DirectorySeparatorChar),
                              outputFolder.TrimEnd(Path.DirectorySeparatorChar),
                              StringComparison.OrdinalIgnoreCase))
            {
                Console.Error.WriteLine("Input and output folders must be different.");
                return 1;
            }

            try
            {
                Directory.CreateDirectory(outputFolder);
                return ProcessFiles(inputFolder, outputFolder);
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine(exception.Message);
                return 2;
            }
        }

        private static int ProcessFiles(string inputFolder, string outputFolder)
        {
            string[] files = Directory.GetFiles(inputFolder, "*", SearchOption.TopDirectoryOnly);
            var timer = Stopwatch.StartNew();
            int processed = 0;
            int changed = 0;
            int failed = 0;

            foreach (string inputPath in files)
            {
                string outputPath = Path.Combine(outputFolder, Path.GetFileName(inputPath));

                try
                {
                    if (ProcessFile(inputPath, outputPath))
                        changed++;

                    processed++;
                }
                catch (IOException exception)
                {
                    failed++;
                    Console.Error.WriteLine("Skipped " + Path.GetFileName(inputPath) + ": " + exception.Message);
                }
                catch (UnauthorizedAccessException exception)
                {
                    failed++;
                    Console.Error.WriteLine("Skipped " + Path.GetFileName(inputPath) + ": " + exception.Message);
                }
            }

            timer.Stop();
            Console.WriteLine("Files copied: " + processed);
            Console.WriteLine("Files with converted weights: " + changed);
            Console.WriteLine("Files skipped: " + failed);
            Console.WriteLine("Elapsed: " + timer.Elapsed.TotalMilliseconds.ToString("0.0") + " ms");
            return failed == 0 ? 0 : 2;
        }

        private static bool ProcessFile(string inputPath, string outputPath)
        {
            bool changed = false;

            using (var reader = new StreamReader(inputPath, Encoding.UTF8, true, 65536))
            using (var writer = new StreamWriter(outputPath, false, new UTF8Encoding(false), 65536))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string converted = WeightRegex.Replace(line, "$1$2");
                    if (!changed && !string.Equals(line, converted, StringComparison.Ordinal))
                        changed = true;

                    writer.WriteLine(converted);
                }
            }

            return changed;
        }
    }
}
