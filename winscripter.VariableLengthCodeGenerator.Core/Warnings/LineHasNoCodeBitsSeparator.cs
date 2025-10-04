namespace winscripter.VariableLengthCodeGenerator.Core.Warnings
{
    public class LineHasNoCodeBitsSeparator : ParserWarning
    {
        public LineHasNoCodeBitsSeparator(int line)
        {
            Line = line;
        }
    }
}
