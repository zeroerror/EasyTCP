using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace ZeroFrame.CodeAnalysis
{

    public class MethodEditor
    {

        SyntaxTree tree;

        CompilationUnitSyntax root;

        public MethodDeclarationSyntax MethodDeclarationSyntax {get;private set;}
        public ConstructorDeclarationSyntax ConstructorDeclarationSyntax { get; private set; }


        //确定使用C# 的版本和传入的预编译量
        CSharpParseOptions option = new CSharpParseOptions(LanguageVersion.CSharp8, preprocessorSymbols: new List<string>() { "Debug" });

        public MethodEditor(string code)
        {
            // 不知道如何只需要方法字符串，现在只能随便外面套上一个class
            string finnalCode = "public class test{" + code + "}";
            SourceText st = SourceText.From(finnalCode);
            tree = CSharpSyntaxTree.ParseText(st, option);
            root = tree.GetCompilationUnitRoot();
            var e1 = root.DescendantNodes().OfType<MethodDeclarationSyntax>().GetEnumerator();
            if (e1.MoveNext())
            {
                MethodDeclarationSyntax = e1.Current;
                return;
            }

            var e2 = root.DescendantNodes().OfType<ConstructorDeclarationSyntax>().GetEnumerator();
            if (e2.MoveNext())
            {
                ConstructorDeclarationSyntax = e2.Current;
                return;
            }

        }

        public MethodDeclarationSyntax CreateMethodDeclarationSyntax(string returnTypeName, string methodName, string[] parameterTypes, string[] paramterNames)
        {
            SyntaxList<AttributeListSyntax> attributeLists = SyntaxFactory.List<AttributeListSyntax>();
            SyntaxTokenList modifiers = SyntaxFactory.TokenList();
            TypeSyntax returnType = SyntaxFactory.ParseTypeName(returnTypeName);
            ExplicitInterfaceSpecifierSyntax explicitInterfaceSpecifier = null;
            SyntaxToken identifier = SyntaxFactory.Identifier(methodName);
            TypeParameterListSyntax typeParameterList = null;
            ParameterListSyntax parameterList = SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(GetParametersList(parameterTypes, paramterNames)));
            SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses = SyntaxFactory.List<TypeParameterConstraintClauseSyntax>();
            BlockSyntax body = null;
            ArrowExpressionClauseSyntax expressionBody = null;
            SyntaxToken semicolonToken = SyntaxFactory.Token(SyntaxKind.SemicolonToken);

            return SyntaxFactory.MethodDeclaration(
              attributeLists,
              modifiers,
              returnType,
              explicitInterfaceSpecifier,
              identifier,
              typeParameterList,
              parameterList,
              constraintClauses,
              body,
              expressionBody,
              semicolonToken);
        }

        IEnumerable<ParameterSyntax> GetParametersList(string[] parameterTypes, string[] paramterNames)
        {
            for (int i = 0; i < parameterTypes.Length; i++)
            {
                yield return SyntaxFactory.Parameter(attributeLists: SyntaxFactory.List<AttributeListSyntax>(),
                                                         modifiers: SyntaxFactory.TokenList(),
                                                         type: SyntaxFactory.ParseTypeName(parameterTypes[i] + " "),
                                                         identifier: SyntaxFactory.Identifier(paramterNames[i]),
                                                         @default: null);
            }
        }

    }
}
