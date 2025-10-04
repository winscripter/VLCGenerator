using winscripter.VariableLengthCodeGenerator.Core.Warnings;

namespace winscripter.VariableLengthCodeGenerator.Core.DocumentModel
{
    public class VlcRazorLikeDocument
    {
        private readonly List<VlcOptionSet> _options;
        private readonly List<VlcCodeDefinition> _codeDefinitions;

        public VlcRazorLikeDocument()
        {
            _options = [];
            _codeDefinitions = [];
        }

        public List<VlcOptionSet> Options => _options;
        public List<VlcCodeDefinition> CodeDefinitions => _codeDefinitions;

        public string? NamespaceConfiguration
        {
            get
            {
                return TryGetOption("namespace");
            }

            set
            {
                TrySetOption("namespace", value);
            }
        }

        public string? ClassNameConfiguration
        {
            get
            {
                return TryGetOption("class");
            }

            set
            {
                TrySetOption("class", value);
            }
        }

        public string? BitReaderType
        {
            get
            {
                return TryGetOption("readerType");
            }

            set
            {
                TrySetOption("readerType", value);
            }
        }

        public string? ReadBitMethod
        {
            get
            {
                return TryGetOption("readBitMethod");
            }

            set
            {
                TrySetOption("readBitMethod", value);
            }
        }

        public string? BitWriterType
        {
            get
            {
                return TryGetOption("writerType");
            }

            set
            {
                TrySetOption("writerType", value);
            }
        }

        public string? WriteBitMethod
        {
            get
            {
                return TryGetOption("writeBitMethod");
            }

            set
            {
                TrySetOption("writeBitMethod", value);
            }
        }

        public string? ClassVisibility
        {
            get
            {
                return TryGetOption("visibility");
            }

            set
            {
                TrySetOption("visibility", value);
            }
        }

        public string? TryGetOption(string name)
        {
            return Options
                .SingleOrDefault(x => x.Name == name)
                ?.Value;
        }

        public void TrySetOption(string name, string? value)
        {
            if (value != null)
            {
                if (Options.Any(x => x.Name == name))
                {
                    foreach (var option in Options)
                    {
                        if (option.Name == name)
                        {
                            Options.Remove(option);
                            var opt = option with
                            {
                                Value = value
                            };
                            Options.Add(opt);
                        }
                    }
                }
                else
                {
                    Options.Add(new VlcOptionSet(name, value));
                }
            }
        }

        public ParsingResult Parse(TextReader reader)
        {
            var warnings = new List<ParserWarning>();
            int lineIndex = 0;
            bool seenCodeDefinition = false;

            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                lineIndex++;

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                if (line.StartsWith("@"))
                {
                    // Razor-like option

                    string afterwards = line[1..];

                    string[] splitted = afterwards.Split(' ');

                    string name = splitted[0];
                    string value = splitted[1];

                    Options.Add(new VlcOptionSet(name, value));

                    if (seenCodeDefinition)
                        warnings.Add(new OptionSetsGoFirstWarning(lineIndex));
                }
                else
                {
                    // Code definition
                    seenCodeDefinition = true;

                    int firstSpaceIndex = line.IndexOf(' ');
                    if (firstSpaceIndex == -1)
                    {
                        // No space between value and bits means invalid line,
                        // so report warning and skip.
                        warnings.Add(new LineHasNoCodeBitsSeparator(lineIndex));
                        continue;
                    }

                    string codeValue = line[..firstSpaceIndex];
                    string bits = line[(firstSpaceIndex + 1)..];

                    // When you copy bits, say, from the ITU-T H.26x specifications, some bits
                    // could contain spaces, so remove them.

                    bits = bits.Replace(" ", "");

                    CodeDefinitions.Add(new VlcCodeDefinition(codeValue, bits));
                }
            }

            return new ParsingResult(warnings);
        }

        public void Write(TextWriter writer)
        {
            // Option sets go first

            foreach (VlcOptionSet optionSet in Options)
                writer.WriteLine($"@{optionSet.Name} {optionSet.Value}");

            // Write blank line for formatting
            writer.WriteLine();

            // Write the code definitions
            foreach (VlcCodeDefinition codeDefinition in CodeDefinitions)
                writer.WriteLine($"{codeDefinition.Code} {codeDefinition.Bits}");
        }
    }
}
