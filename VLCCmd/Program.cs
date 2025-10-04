using winscripter.VariableLengthCodeGenerator.Core.DocumentModel;
using winscripter.VariableLengthCodeGenerator.Core.Generator;

internal class Program
{
    private static void Main(string[] args)
    {
        string contents = File.ReadAllText(args[0]);

        var vlc = new VlcRazorLikeDocument();
        vlc.Parse(new StringReader(contents));

        string result = CSharpGenerator.Generate(vlc);

        File.WriteAllText(args[1], result);
    }
}
