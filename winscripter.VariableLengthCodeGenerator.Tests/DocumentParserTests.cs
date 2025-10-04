using winscripter.VariableLengthCodeGenerator.Core.DocumentModel;
using winscripter.VariableLengthCodeGenerator.Core.Warnings;

namespace winscripter.VariableLengthCodeGenerator.Tests
{
    public class DocumentParserTests
    {
        [Fact]
        public void Document_Should_Parse_No_Exceptions()
        {
            const string document =
"""
1 1010 1010
2 1011 0110
""";
            _ = new VlcRazorLikeDocument().Parse(new StringReader(document));
        }

        [Fact]
        public void Document_Should_Create_Warnings_Of_No_Separators()
        {
            const string document =
"""
11010
21011
""";
            var vlc = new VlcRazorLikeDocument();
            var result = vlc.Parse(new StringReader(document));

            Assert.Contains(result.Warnings, (x) => x is LineHasNoCodeBitsSeparator);
            Assert.Empty(vlc.CodeDefinitions);
            Assert.Equal(2, result.Warnings.Count);
        }

        [Fact]
        public void Document_Options_Should_Have_Namespace()
        {
            const string document =
"""
@namespace MyCompany.MyProduct
""";

            var vlc = new VlcRazorLikeDocument();
            vlc.Parse(new StringReader(document));

            Assert.NotNull(vlc.NamespaceConfiguration);
            Assert.Equal("MyCompany.MyProduct", vlc.NamespaceConfiguration);
        }

        [Fact]
        public void Document_Options_Should_Have_ClassName()
        {
            const string document =
"""
@class VLC
""";

            var vlc = new VlcRazorLikeDocument();
            vlc.Parse(new StringReader(document));

            Assert.NotNull(vlc.ClassNameConfiguration);
            Assert.Equal("VLC", vlc.ClassNameConfiguration);
        }

        [Fact]
        public void Document_Options_Should_Change_Namespace()
        {
            const string document =
"""
@namespace MyCompany.MyProduct
""";

            var vlc = new VlcRazorLikeDocument();
            vlc.Parse(new StringReader(document));

            vlc.NamespaceConfiguration = "GreatCompany.GreatProduct.🐤💤";

            Assert.NotNull(vlc.NamespaceConfiguration);
            Assert.Equal("GreatCompany.GreatProduct.🐤💤", vlc.NamespaceConfiguration);
        }

        [Fact]
        public void Document_Options_Should_Change_Class()
        {
            const string document =
"""
@class FlapFlap🐣
""";

            var vlc = new VlcRazorLikeDocument();
            vlc.Parse(new StringReader(document));

            vlc.ClassNameConfiguration = "PeepPeep🐣";

            Assert.NotNull(vlc.ClassNameConfiguration);
            Assert.Equal("PeepPeep🐣", vlc.ClassNameConfiguration);
        }

        [Fact]
        public void Document_Options_Should_Create_Namespace()
        {
            var vlc = new VlcRazorLikeDocument
            {
                NamespaceConfiguration = "GreatCompany.GreatProduct.🐤💤"
            };

            Assert.NotNull(vlc.NamespaceConfiguration);
            Assert.Equal("GreatCompany.GreatProduct.🐤💤", vlc.NamespaceConfiguration);
        }

        [Fact]
        public void Document_Options_Should_Create_Class()
        {
            var vlc = new VlcRazorLikeDocument
            {
                ClassNameConfiguration = "PeepPeep🐣"
            };

            Assert.NotNull(vlc.ClassNameConfiguration);
            Assert.Equal("PeepPeep🐣", vlc.ClassNameConfiguration);
        }
    }
}
