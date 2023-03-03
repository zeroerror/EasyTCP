using System.Linq;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace ZeroFrame.CodeAnalysis
{

    public class ClassEditor
    {

        CompilationUnitSyntax _root;

        public string Text => _root.GetText().ToString();

        //确定使用C# 的版本和传入的预编译量
        CSharpParseOptions option = new CSharpParseOptions(LanguageVersion.Latest);
        SyntaxRemoveOptions remove_option = SyntaxRemoveOptions.KeepNoTrivia;

        public ClassEditor(string code)
        {
            SourceText st = SourceText.From(code);
            var tree = CSharpSyntaxTree.ParseText(st, option);
            _root = tree.GetCompilationUnitRoot();
        }

        /// <param name="name">exmp:System.Collection</param>
        public void AddUsing(string name)
        {
            // Avoid Repeat Add.
            RemoveUsing(name);
            _root = _root.AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName($" {name}")));
        }

        public void RemoveUsing(string name)
        {
            foreach (var u in _root.Usings)
            {
                if (u.Name.ToString() == name)
                {
                    _root = _root.RemoveNode(u, remove_option);
                }
            }
        }

        public void AddField(string code, int classIndex = 0)
        {
            string finnalCode = "public class test{" + code + "}";
            SourceText st = SourceText.From(finnalCode);
            var tree = CSharpSyntaxTree.ParseText(st, option);
            var root = tree.GetRoot();
            var field = root.DescendantNodes().OfType<FieldDeclarationSyntax>().First();
            var _class = GetClass(classIndex);
            var _newClass = _class.AddMembers(field);
            _root = _root.ReplaceNode(_class, _newClass);
        }

        public void RemoveAllField()
        {
            var e = _root.DescendantNodes().OfType<FieldDeclarationSyntax>().GetEnumerator();
            while (e.MoveNext())
            {
                _root = _root.RemoveNode(e.Current, remove_option);
            }
        }

        public void AddInterface(string iName, string tName = null)
        {
            ClassDeclarationSyntax _classDeclaration = GetFirstClass();
            ClassDeclarationSyntax _newclassDeclaration;

            var token = SyntaxFactory.Identifier(iName);
            if (token == null) return;
            if (tName != null)
            {
                var nameSyntax = SyntaxFactory.GenericName(token);
                var separatedSyntaxList = SyntaxFactory.SeparatedList<TypeSyntax>(GetTypeSyntaxByIdentifier(tName));
                var nameSyntax2 = nameSyntax.WithTypeArgumentList(SyntaxFactory.TypeArgumentList(separatedSyntaxList));
                var bastType = SyntaxFactory.SimpleBaseType(nameSyntax2);
                _newclassDeclaration = _classDeclaration.AddBaseListTypes(bastType);
            }
            else
            {
                var identifierNameSyntax = SyntaxFactory.IdentifierName(token);
                var bastType = SyntaxFactory.SimpleBaseType(identifierNameSyntax);
                _newclassDeclaration = _classDeclaration.AddBaseListTypes(bastType);
            }

            _root = _root.ReplaceNode(_classDeclaration, _newclassDeclaration);
        }

        public void RemoveInterface(string iName, string tName = null)
        {
            ClassDeclarationSyntax _classDeclaration = GetFirstClass();
            if (_classDeclaration.BaseList == null) return;

            ClassDeclarationSyntax _newclassDeclaration = _classDeclaration;

            string name = (tName != null) ? $"{iName}<{tName}>" : iName;

            BaseTypeSyntax node = null;
            var e = _classDeclaration.BaseList.Types.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.GetText().ToString() == name)
                    node = e.Current;
            }

            if (node == null) return;

            BaseListSyntax newBaseListSyntax = SyntaxFactory.BaseList();
            var list = _classDeclaration.BaseList.RemoveNode(node, remove_option).Types;
            foreach (var type in list)
            {
                newBaseListSyntax.AddTypes(type);
            }

            _newclassDeclaration = _classDeclaration.ReplaceNode(_classDeclaration.BaseList, newBaseListSyntax);
            _root = _root.ReplaceNode(_classDeclaration, _newclassDeclaration);
        }

        public void AddMethod(MethodEditor methodEditor)
        {
            var _class = GetFirstClass();
            ClassDeclarationSyntax _newClass = _class;
            if (methodEditor.MethodDeclarationSyntax != null)
            {
                _newClass = _class.AddMembers(methodEditor.MethodDeclarationSyntax);
            }
            else
            {
                _newClass = _class.AddMembers(methodEditor.ConstructorDeclarationSyntax);
            }

            _root = _root.ReplaceNode(_class, _newClass);
        }

        public void RemoveMethod(string methodName)
        {
            ClassDeclarationSyntax _class = GetFirstClass();
            SyntaxNode node = null;
            _root.DescendantNodes().OfType<MemberDeclarationSyntax>().All((mem) =>
                {
                    var method = mem as MethodDeclarationSyntax;
                    if (method != null)
                    {
                        var enumerator = method.DescendantNodesAndSelf().GetEnumerator();
                        enumerator.MoveNext();
                        var methodNode = enumerator.Current;
                        var methodDeclarationSyntax = methodNode as MethodDeclarationSyntax;
                        var methodIdentifier = methodDeclarationSyntax.Identifier;
                        if (methodIdentifier.ToString() == methodName)
                        {
                            node = methodNode;
                            return false;
                        }
                    }
                    else
                    {
                        var constructMethod = mem as ConstructorDeclarationSyntax;
                        if (constructMethod != null)
                        {
                            var enumerator = constructMethod.DescendantNodesAndSelf().GetEnumerator();
                            enumerator.MoveNext();
                            var methodNode = enumerator.Current;
                            var methodDeclarationSyntax = methodNode as ConstructorDeclarationSyntax;
                            var methodIdentifier = methodDeclarationSyntax.Identifier;
                            if (methodIdentifier.ToString() == methodName)
                            {
                                node = methodNode;
                                return false;
                            }
                        }
                    }

                    return true;
                });

            if (node != null)
            {
                _root = _root.RemoveNode(node, remove_option);
            }
        }

        public bool IsClassHasAttribute(string attributeName)
        {
            var _class = GetFirstClass();
            foreach (var attributeList in _class.AttributeLists)
            {
                foreach (var attribute in attributeList.Attributes)
                {
                    if (attribute.Name.ToString() == attributeName)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public (List<string>, List<string>) GetTypeVariableDic()
        {
            List<string> typeList = new List<string>();
            List<string> varList = new List<string>();
            _root.DescendantNodes().OfType<FieldDeclarationSyntax>().All((fieldDeclarationSyntax) =>
            {
                typeList.Add(fieldDeclarationSyntax.Declaration.Type.ToString());
                varList.Add(fieldDeclarationSyntax.Declaration.Variables[0].ToString());
                return true;
            });

            return (typeList, varList);
        }

        public string GetClassName(int classIndex = 0)
        {
            var _classDeclarationSyntax = GetClass(classIndex);
            if (_classDeclarationSyntax == null) return null;

            string name = GetClassName(_classDeclarationSyntax);
            return name;
        }

        string GetClassName(ClassDeclarationSyntax _class)
        {
            return _class.Identifier.ToString();
        }

        IEnumerable<TypeSyntax> GetTypeSyntaxByIdentifier(string typeName)
        {
            var token = SyntaxFactory.Identifier(typeName);
            var identifierNameSyntax = SyntaxFactory.IdentifierName(token);
            yield return identifierNameSyntax;
        }

        ClassDeclarationSyntax GetFirstClass()
        {
            return _root.DescendantNodes().OfType<ClassDeclarationSyntax>().First();
        }

        ClassDeclarationSyntax GetClass(int classIndex = 0)
        {
            int index = 0;
            var enumerator = _root.DescendantNodes().OfType<ClassDeclarationSyntax>().GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (index == classIndex)
                {
                    return enumerator.Current;
                }
            }

            return null;
        }

    }
}

