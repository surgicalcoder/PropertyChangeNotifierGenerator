using System.Collections.Generic;
using System.Linq;
using GoLive.Generator.PropertyChangedNotifier.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GoLive.Generator.PropertyChangedNotifier
{
    public static class Scanner
    {
        public static IEnumerable<ClassToGenerate> ScanForControllers(SemanticModel semantic, Settings config)
        {
            var controllerBase = semantic.Compilation.GetTypeByMetadataName(config.BaseTypeName);

            if (controllerBase == null)
            {
                yield break;
            }

            var allNodes = semantic.SyntaxTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>();

            foreach (var node in allNodes)
            {
                if (semantic.GetDeclaredSymbol(node) is INamedTypeSymbol classSymbol &&
                    InheritsFrom(classSymbol, controllerBase))
                {
                    var syntaxTreeFilePath = node.SyntaxTree.FilePath;
                    
                    if (syntaxTreeFilePath.EndsWith(".generated.cs"))
                    {
                        continue;
                    }
                    
                    yield return GenerateClassDefinition(syntaxTreeFilePath, classSymbol);
                }
            }
        }

        public static ClassToGenerate GenerateClassDefinition(string syntaxTreeFilePath, INamedTypeSymbol classSymbol)
        {
            ClassToGenerate gen = new ClassToGenerate();

            gen.Name = classSymbol.Name;
            gen.Namespace = classSymbol.ContainingNamespace.ToDisplayString();
            gen.Filename = syntaxTreeFilePath;
            
            foreach (var member in classSymbol.GetMembers())
            {
                if (member is IFieldSymbol
                    {
                        //DeclaredAccessibility: Accessibility.Private, IsAbstract: false
                    } fieldSymbol /*and not {MethodKind: MethodKind.Constructor}*/)
                {
                    gen.Members.Add(new MemberToGenerate()
                    {
                        Name = fieldSymbol.Name, 
                        Type = fieldSymbol.Type.ToDisplayString()
                    });
                }
            }

            return gen;
        }

        private static bool InheritsFrom(INamedTypeSymbol classDeclaration, INamedTypeSymbol targetBaseType)
        {
            var currentDeclared = classDeclaration;

            while (currentDeclared.BaseType != null)
            {
                var currentBaseType = currentDeclared.BaseType;

                if (currentBaseType.Equals(targetBaseType, SymbolEqualityComparer.Default))
                {
                    return true;
                }

                currentDeclared = currentDeclared.BaseType;
            }

            return false;
        }
    }
}