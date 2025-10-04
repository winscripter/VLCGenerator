using winscripter.VariableLengthCodeGenerator.Core.DocumentModel;

namespace winscripter.VariableLengthCodeGenerator.Core.Internal
{
    internal static class NodeComputation
    {
        public static Node CreateNodeUsingVlcDocument(VlcRazorLikeDocument razorLikeDocument)
        {
            var root = new Node();

            foreach (var definition in razorLikeDocument.CodeDefinitions)
            {
                var current = root;
                foreach (char bit in definition.Bits)
                {
                    current = bit == '0'
                        ? current.Zero ??= new Node()
                        : current.One ??= new Node();
                }

                current.Value = definition.Code;
            }

            return root;
        }
    }
}
