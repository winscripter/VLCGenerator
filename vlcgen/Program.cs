using System.Diagnostics;
using winscripter.VariableLengthCodeGenerator.Core.DocumentModel;
using winscripter.VariableLengthCodeGenerator.Core.Generator;

internal class Program
{
    private static void Main(string[] args)
    {
        if (args.Length != 2)
        {
            DisplayHelp();
            return;
        }

        Console.WriteLine("VLC to C# Generator");
        Console.WriteLine("Copyright (c) 2023-2025, winscripter");
        Console.WriteLine();

        string sourceFile = args[0];
        string targetFile = args[1];

        if (!File.Exists(sourceFile))
        {
            WriteRed($"File {sourceFile} does not exist");
            return;
        }

        if (File.Exists(targetFile))
        {
            WriteRed($"File {targetFile} does not exist");
            return;
        }

        var sw = Stopwatch.StartNew();
        string contents = File.ReadAllText(sourceFile);

        var vlc = new VlcRazorLikeDocument();
        vlc.Parse(new StringReader(contents));

        string result = CSharpGenerator.Generate(vlc);

        File.WriteAllText(targetFile, result);

        sw.Stop();
        Console.WriteLine($"Completed in {sw.Elapsed.TotalSeconds}");
    }

    private static void DisplayHelp()
    {
        Console.WriteLine("Usage: vlcgen <source.vlcdef> <target.g.cs>");
    }

    private static void WriteRed(string text)
    {
        ConsoleColor cc = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(text);
        Console.ForegroundColor = cc;
    }
}
