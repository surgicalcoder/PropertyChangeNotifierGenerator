using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using GoLive.Generator.PropertyChangedNotifier.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GoLive.Generator.PropertyChangedNotifier
{
    public static class Scanner
    {
        public static IEnumerable<ClassToGenerate> ScanForEligibleClasses(SemanticModel semantic, Settings config, GeneratorExecutionContext generatorExecutionContext)
        {
            var controllerBase = semantic.Compilation.GetTypeByMetadataName(config.BaseTypeName);

            if (controllerBase == null) yield break;

            var allNodes = semantic.SyntaxTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>();

            foreach (var node in allNodes)
                if (semantic.GetDeclaredSymbol(node) is INamedTypeSymbol classSymbol &&
                    InheritsFrom(classSymbol, controllerBase))
                {
                    var syntaxTreeFilePath = node.SyntaxTree.FilePath;

                    if (syntaxTreeFilePath.EndsWith("generated.cs"))
                    {
                        continue;
                    }

                    yield return GenerateClassDefinition(syntaxTreeFilePath, classSymbol, generatorExecutionContext);
                }
        }

        public static ClassToGenerate GenerateClassDefinition(string syntaxTreeFilePath, INamedTypeSymbol classSymbol, GeneratorExecutionContext generatorExecutionContext)
        {
            var gen = new ClassToGenerate();

            gen.Name = classSymbol.Name;
            gen.Namespace = classSymbol.ContainingNamespace.ToDisplayString();
            gen.Filename = syntaxTreeFilePath;

            foreach (var member in classSymbol.GetMembers())
                if (member is IFieldSymbol
                    {
                        //DeclaredAccessibility: Accessibility.Private, IsAbstract: false
                    } fieldSymbol /*and not {MethodKind: MethodKind.Constructor}*/)
                {
                    var memberToGenerate = new MemberToGenerate
                    {
                        Name = fieldSymbol.Name,
                        Type = fieldSymbol.Type
                    };
                 
                    var attr = fieldSymbol.GetAttributes();
                    
                    if(attr.Any(e=>e.AttributeClass.ToString() == "GoLive.Generator.PropertyChangedNotifier.Utilities.DoNotTrackChangesAttribute"))
                    {
                        continue;
                    }
                    else if (attr.Any(e => e.AttributeClass.ToString() == "GoLive.Generator.PropertyChangedNotifier.Utilities.ReadonlyAttribute"))
                    {
                        memberToGenerate.ReadOnly = true;
                    }                    
                    else if (attr.Any(e => e.AttributeClass.ToString() == "GoLive.Generator.PropertyChangedNotifier.Utilities.WriteOnlyAttribute"))
                    {
                        memberToGenerate.WriteOnly = true;
                    }
                    
                    switch (fieldSymbol.Type)
                    {
                        case INamedTypeSymbol s2 when s2.OriginalDefinition.ToString() == "FastMember.TypeAccessor":
                            continue;
                        case INamedTypeSymbol s1 when s1.OriginalDefinition.ToString() == "ObservableCollections.ObservableList<T>":
                            memberToGenerate.IsCollection = true;
                            memberToGenerate.CollectionType = s1.TypeArguments.FirstOrDefault();
                            break;
                    }

                    gen.Members.Add(memberToGenerate);
                }

            return gen;
        }

        private static bool InheritsFrom(INamedTypeSymbol classDeclaration, INamedTypeSymbol targetBaseType)
        {
            var currentDeclared = classDeclaration;

            while (currentDeclared.BaseType != null)
            {
                var currentBaseType = currentDeclared.BaseType;

                if (currentBaseType.Equals(targetBaseType, SymbolEqualityComparer.Default)) return true;

                currentDeclared = currentDeclared.BaseType;
            }

            return false;
        }
    }
}