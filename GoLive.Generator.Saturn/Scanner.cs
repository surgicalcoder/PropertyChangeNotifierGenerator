using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GoLive.Generator.Saturn;

public static class Scanner
{
    private static readonly SymbolDisplayFormat symbolDisplayFormat = new(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);

    public static bool CanBeEntity(SyntaxNode node)
    {
        return node is ClassDeclarationSyntax { BaseList.Types.Count: > 0 };
    }

    public static bool IsEntity(INamedTypeSymbol classDeclaration)
    {
        return InheritsFrom(classDeclaration, "GoLive.Saturn.Data.Entities.Entity");
    }

    private static bool InheritsFrom(INamedTypeSymbol classDeclaration, string qualifiedBaseTypeName)
    {
        var currentDeclared = classDeclaration;

        while (currentDeclared.BaseType != null)
        {
            var currentBaseType = currentDeclared.BaseType;

            if (string.Equals(currentBaseType.ToDisplayString(symbolDisplayFormat), qualifiedBaseTypeName, StringComparison.Ordinal))
            {
                return true;
            }

            currentDeclared = currentBaseType;
        }

        return false;
    }

    public static ClassToGenerate ConvertToMapping(INamedTypeSymbol classSymbol)
    {
        ClassToGenerate retr = new();

        retr.Filename = classSymbol.Locations.FirstOrDefault(e => !e.SourceTree.FilePath.EndsWith(".generated.cs")).SourceTree.FilePath;
        retr.Name = classSymbol.Name;
        retr.Namespace = classSymbol.ContainingNamespace.ToDisplayString();
        retr.Members = ConvertToMembers(classSymbol).ToList();

        return retr;
    }

    private static IEnumerable<MemberToGenerate> ConvertToMembers(INamedTypeSymbol classSymbol)
    {
        foreach (var member in classSymbol.GetMembers())
        {
            if (member is not IFieldSymbol
                {
                    DeclaredAccessibility: Accessibility.Private, IsAbstract: false, AssociatedSymbol: null
                } fieldSymbol /*and not {MethodKind: MethodKind.Constructor}*/)
            {
                continue;
            }

            var memberToGenerate = new MemberToGenerate
            {
                Name = fieldSymbol.Name,
                Type = fieldSymbol.Type
            };

            var attr = fieldSymbol.GetAttributes();

            if (attr.Any(e => e.AttributeClass.ToString() == "GoLive.Generator.Saturn.Resources.DoNotTrackChangesAttribute"))
            {
                continue;
            }

            if (attr.Any(e => e.AttributeClass.ToString() == "GoLive.Generator.Saturn.Resources.AddRefToScopeAttribute"))
            {
                memberToGenerate.IsScoped = true;
            }

            if (attr.Any(e => e.AttributeClass.ToString() == "GoLive.Generator.Saturn.Resources.ReadonlyAttribute"))
            {
                memberToGenerate.ReadOnly = true;
            }
            else if (attr.Any(e => e.AttributeClass.ToString() == "GoLive.Generator.Saturn.Resources.WriteOnlyAttribute"))
            {
                memberToGenerate.WriteOnly = true;
            }

            if (attr.Any(e => e.AttributeClass.ToString() == "GoLive.Generator.Saturn.Resources.AddToLimitedViewAttribute"))
            {
                memberToGenerate.LimitedViews = attr.Where(f => f.AttributeClass.ToString() == "GoLive.Generator.Saturn.Resources.AddToLimitedViewAttribute")
                    .Select(e =>
                    {
                        var retr =  new LimitedViewToGenerate();
                        retr.Name = e.ConstructorArguments.FirstOrDefault(r => r.Value != null).Value as string;

                        if (e.NamedArguments.Any())
                        {
                            retr.OverrideReturnTypeToUseLimitedView = e.NamedArguments.FirstOrDefault(r => r.Key == "UseLimitedView").Value.Value.ToString();
                        }

                        return retr;
                    }).ToList();
            }

            if (attr.Any(r => !r.AttributeClass.ToString().StartsWith("GoLive.Generator.Saturn.Resources.")))
            {
                foreach (var at in attr.Where(r => !r.AttributeClass.ToString().StartsWith("GoLive.Generator.Saturn.Resources.")))
                {
                    MemberAttribute memAt = new();

                    memAt.Name = at.AttributeClass.ToString();

                    if (at.ConstructorArguments != null && at.ConstructorArguments.Length > 0)
                    {
                        foreach (var atConstructorArgument in at.ConstructorArguments)
                        {
                            memAt.ConstructorParameters.AddRange(atConstructorArgument.Values.Select(f=>f.Value?.ToString()));
                        }
                    }

                    if (at.NamedArguments != null && at.NamedArguments.Length > 0)
                    {
                        memAt.NamedParameters = at.NamedArguments.Select
                            (r => new KeyValuePair<string, string>(r.Key, r.Value.Value?.ToString())).ToList();
                    }
                    
                    memberToGenerate.AdditionalAttributes.Add(memAt);
                }
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

            yield return memberToGenerate;
        }
    }
}