using Microsoft.CSharp;
using System.CodeDom.Compiler;
using winscripter.VariableLengthCodeGenerator.Core.DocumentModel;

namespace winscripter.VariableLengthCodeGenerator.Core.Generator
{
    public static class CSharpGenerator
    {
        public static void Generate(TextWriter writer, VlcRazorLikeDocument razorLikeDocument)
        {
            var ccu = CodeDomTreeGenerator.CreateCompileUnit(razorLikeDocument);

            var csp = new CSharpCodeProvider();
            csp.GenerateCodeFromCompileUnit(ccu, writer, new CodeGeneratorOptions()
            {
                BlankLinesBetweenMembers = true,
                IndentString = "    "
            });
        }

        public static string Generate(VlcRazorLikeDocument razorLikeDocument)
        {
            using var sw = new StringWriter();
            Generate(sw, razorLikeDocument);
            return sw.ToString();
        }
    }
}
