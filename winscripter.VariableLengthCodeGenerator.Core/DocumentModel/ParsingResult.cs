using winscripter.VariableLengthCodeGenerator.Core.Warnings;

namespace winscripter.VariableLengthCodeGenerator.Core.DocumentModel
{
    public record ParsingResult(List<ParserWarning> Warnings);
}
