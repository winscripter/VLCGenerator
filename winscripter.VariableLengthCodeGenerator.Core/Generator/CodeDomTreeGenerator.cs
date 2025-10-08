using System.CodeDom;
using System.Reflection;
using winscripter.VariableLengthCodeGenerator.Core.DocumentModel;
using winscripter.VariableLengthCodeGenerator.Core.Internal;

namespace winscripter.VariableLengthCodeGenerator.Core.Generator
{
    internal static class CodeDomTreeGenerator
    {
        private static TypeAttributes GetVisibilityAttribute(string csharp)
        {
            return csharp switch
            {
                "public" => TypeAttributes.Public,
                "internal" or _ => TypeAttributes.NotPublic
            };
        }

        public static CodeCompileUnit CreateCompileUnit(VlcRazorLikeDocument document)
        {
            var ccu = new CodeCompileUnit();
            var cns = new CodeNamespace(document.NamespaceConfiguration ?? "GeneratedVlcSerializers");
            
            var ctd = new CodeTypeDeclaration(document.ClassNameConfiguration ?? "VlcSerializer")
            {
                TypeAttributes = GetVisibilityAttribute(document.ClassVisibility ?? "internal")
            };
            var cmm = new CodeMemberMethod()
            {
                Name = "Decode",
                Attributes = MemberAttributes.Public | MemberAttributes.Static,
                ReturnType = new CodeTypeReference(typeof(long))
            };
            cmm.Parameters.Add(new CodeParameterDeclarationExpression(document.BitReaderType ?? "IBitStream", "reader"));
            EmitNode(NodeComputation.CreateNodeUsingVlcDocument(document), cmm.Statements);

            cmm.Statements.Add(new CodeMethodReturnStatement(new CodePrimitiveExpression(0)));

            var cmmEncode = new CodeMemberMethod()
            {
                Name = "Encode",
                Attributes = MemberAttributes.Public | MemberAttributes.Static,
                ReturnType = new CodeTypeReference(typeof(void))
            };
            cmmEncode.Parameters.Add(new CodeParameterDeclarationExpression(document.BitWriterType ?? "IBitStream", "writer"));
            cmmEncode.Parameters.Add(new CodeParameterDeclarationExpression(typeof(long), "valueToEncode"));

            foreach (var cd in document.CodeDefinitions)
            {
                // ccs will be initialized by PopulateIf
                var ccs = new CodeConditionStatement(new CodeSnippetExpression(string.Empty));

                PopulateIf(cd, ccs);

                cmmEncode.Statements.Add(ccs);
            }

            ctd.Members.Add(cmm);
            ctd.Members.Add(cmmEncode);

            cns.Types.Add(ctd);

            ccu.Namespaces.Add(cns);

            return ccu;

            void PopulateIf(VlcCodeDefinition cd, CodeConditionStatement cond)
            {
                cond.Condition = new CodeBinaryOperatorExpression(
                    new CodeVariableReferenceExpression("valueToEncode"), CodeBinaryOperatorType.ValueEquality, new CodeSnippetExpression($"({cd.Code})"));
                foreach (char c in cd.Bits)
                {
                    cond.TrueStatements.Add(new CodeMethodInvokeExpression(
                        new CodeVariableReferenceExpression("writer"), document.WriteBitMethod ?? "WriteBit", [
                            new CodePrimitiveExpression(c == '1')
                        ]));
                }
                cond.TrueStatements.Add(new CodeMethodReturnStatement());
            }

            void EmitNode(Node node, CodeStatementCollection statements, int recursionCounter = 0)
            {
                if (recursionCounter >= 2500)
                    throw new InvalidOperationException("infinite loop! :skull:");
                
                if (node != null)
                {
                    if (node.Value != null)
                    {
                        statements.Add(new CodeMethodReturnStatement(new CodeSnippetExpression(node.Value)));
                        return;
                    }

                    var readBitCall = new CodeMethodInvokeExpression(
                        new CodeVariableReferenceExpression("reader"),
                        document.ReadBitMethod ?? "ReadBit"
                    );

                    if (node.One != null || node.Zero != null)
                    {
                        var ifStmt = new CodeConditionStatement(
                            new CodeMethodInvokeExpression(
                                new CodeVariableReferenceExpression("reader"),
                                document.ReadBitMethod ?? "ReadBit"
                            )
                        );

                        if (node.One != null)
                            EmitNode(node.One!, ifStmt.TrueStatements, recursionCounter + 1);

                        if (node.Zero != null)
                            EmitNode(node.Zero!, ifStmt.FalseStatements, recursionCounter + 1);

                        statements.Add(ifStmt);
                    }
                }
            }
        }
    }
}
