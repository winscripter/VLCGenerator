using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.IO;
using System.Text;
using winscripter.VariableLengthCodeGenerator.Core.DocumentModel;
using winscripter.VariableLengthCodeGenerator.Core.Generator;
using winscripter.VariableLengthCodeGenerator.Core.Warnings;

namespace winscripter.VariableLengthCodeGenerator.Workspace
{
    [Generator(LanguageNames.CSharp)]
    public class VlcDefGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // Find all additional files ending in .vlcdef
            var vlcDefFiles = context.AdditionalTextsProvider
                .Where(file => file.Path.EndsWith(".vlcdef"))
                .Combine(context.AnalyzerConfigOptionsProvider);

            // Parse and generate source
            context.RegisterSourceOutput(vlcDefFiles, (spc, pair) =>
            {
                var (file, optionsProvider) = pair;
                var text = file.GetText()?.ToString();
                if (string.IsNullOrWhiteSpace(text)) return;

                ParsingResult result = null!;
                string generatedCode = null!;
                try
                {
                    var doc = new VlcRazorLikeDocument();
                    using var sr = new StringReader(text);
                    result = doc.Parse(sr);
                    generatedCode = CSharpGenerator.Generate(doc);
                }
                catch
                {
                    spc.ReportDiagnostic(Diagnostic.Create(
                        new DiagnosticDescriptor(
                            id: "VLGEN0003",
                            title: "Unhandled error",
                            messageFormat: "Something has gone wrong when generating a VLC serialization C# file.",
                            category: "VlcDefGenerator",
                            DiagnosticSeverity.Error,
                            isEnabledByDefault: true),
                        Location.None,
                        file.Path));
                    return;
                }

                foreach (ParserWarning warning in result.Warnings)
                {
                    if (warning is LineHasNoCodeBitsSeparator)
                    {
                        spc.ReportDiagnostic(Diagnostic.Create(
                            new DiagnosticDescriptor(
                                id: "VLGEN0001",
                                title: "No separator between code and bits",
                                messageFormat: $"Line {warning.Line} does not include a whitespace separator for the code value and the actual bits to represent that value. Place a space after the code value.",
                                category: "VlcDefGenerator",
                                DiagnosticSeverity.Warning,
                                isEnabledByDefault: true),
                            Location.None,
                            file.Path));
                    }
                    else if (warning is OptionSetsGoFirstWarning)
                    {
                        spc.ReportDiagnostic(Diagnostic.Create(
                            new DiagnosticDescriptor(
                                id: "VLGEN0002",
                                title: "Place options (lines starting with @) before code definitions first",
                                messageFormat: $"It's best to put options (lines starting with @) at the top of the file for readability purposes. Move line {warning.Line} to the top of the file.",
                                category: "VlcDefGenerator",
                                DiagnosticSeverity.Warning,
                                isEnabledByDefault: true),
                            Location.None,
                            file.Path));
                    }
                }

                spc.AddSource($"{Path.GetFileNameWithoutExtension(file.Path)}.g.cs", SourceText.From(generatedCode, Encoding.UTF8));
            });
        }
    }
}
