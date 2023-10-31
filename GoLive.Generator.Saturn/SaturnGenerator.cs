#region

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CSharpExtensions = Microsoft.CodeAnalysis.CSharp.CSharpExtensions;

#endregion

namespace GoLive.Generator.Saturn;

[Generator]
public class SaturnGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(AddAdditionalFiles);

        var classDeclarations = context.SyntaxProvider.CreateSyntaxProvider(static (s, _) => Scanner.CanBeEntity(s),
                static (ctx, _) => GetEntityDeclarations(ctx))
            .Where(static c => c is not null)
            .Select(static (c, _) => Scanner.ConvertToMapping(c));

        context.RegisterSourceOutput(classDeclarations.Collect(), static (spc, source) => Execute(spc, source));
    }

    private static void Execute(SourceProductionContext spc, ImmutableArray<ClassToGenerate> classesToGenerate)
    {
        foreach (var toGenerate in classesToGenerate)
        {
            var sourceStringBuilder = new SourceStringBuilder();
            SourceCodeGenerator.Generate(sourceStringBuilder, toGenerate);

            if (sourceStringBuilder.ToString() is { Length: > 0 } s)
            {
                spc.AddSource($"{toGenerate.Name}.g.cs", sourceStringBuilder.ToString());
            }
        }
    }

    private void AddAdditionalFiles(IncrementalGeneratorPostInitializationContext context)
    {
        using var reader = new StreamReader(typeof(SaturnGenerator).Assembly.GetManifestResourceStream(EmbeddedResources.Resources_AdditionalFiles_cs), Encoding.UTF8);
        {
            var additionalFileContents = reader.ReadToEnd();
            context.AddSource("_additionalfiles.g.cs", additionalFileContents);
        }
    }

    private static void Execute(ImmutableArray<ClassToGenerate> classesToGenerate, ImmutableArray<AdditionalText> configurationFiles, SourceProductionContext spc)
    {
        foreach (var toGenerate in classesToGenerate)
        {
            var sourceStringBuilder = new SourceStringBuilder();
            SourceCodeGenerator.Generate(sourceStringBuilder, toGenerate);

            if (sourceStringBuilder.ToString() is { Length: > 0 } s)
            {
                var fileName = System.IO.Path.GetFileNameWithoutExtension(toGenerate.Filename);
                spc.AddSource($"{fileName}.g.cs", sourceStringBuilder.ToString());
            }
        }
    }

    private static INamedTypeSymbol GetEntityDeclarations(GeneratorSyntaxContext context)
    {
        var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;
        var symbol = CSharpExtensions.GetDeclaredSymbol(context.SemanticModel, classDeclarationSyntax);

        return symbol is not null && Scanner.IsEntity(symbol) ? symbol : null;
    }

    private static Settings LoadConfig(IEnumerable<AdditionalText> configFiles)
    {
        var configFilePath = configFiles.FirstOrDefault();

        if (configFilePath == null)
        {
            return null;
        }

        var jsonString = File.ReadAllText(configFilePath.Path);
        var config = JsonSerializer.Deserialize<Settings>(jsonString);
        var configFileDirectory = Path.GetDirectoryName(configFilePath.Path);

        config.AdditionalFilesLocation = Path.GetFullPath(Path.Combine(configFileDirectory, config.AdditionalFilesLocation));

        return config;
    }

    public static bool IsConfigurationFile(AdditionalText text)
    {
        return text.Path.EndsWith("PropertyChangeNotifier.json");
    }
}

public class ClassToGenerate
{
    public override string ToString()
    {
        return $"{nameof(Name)}: {Name}, {nameof(Members)}: {Members?.Count}, {nameof(Filename)}: {Filename}, {nameof(Namespace)}: {Namespace}";
    }

    public string Name { get; set; }
    public List<MemberToGenerate> Members { get; set; } = new();
    public string Filename { get; set; }
    public string Namespace { get; set; }
}

public class MemberToGenerate
{
    public override string ToString()
    {
        return $"{nameof(Name)}: {Name}, {nameof(IsCollection)}: {IsCollection}, {nameof(ReadOnly)}: {ReadOnly}, {nameof(WriteOnly)}: {WriteOnly}, {nameof(IsScoped)}: {IsScoped}, {nameof(LimitedViews)}: {LimitedViews?.Count}";
    }

    public string Name { get; set; }
    public ITypeSymbol Type { get; set; }

    public bool IsCollection { get; set; }
    public ITypeSymbol? CollectionType { get; set; }

    public bool ReadOnly { get; set; }
    public bool WriteOnly { get; set; }
    public bool IsScoped { get; set; }

    public List<LimitedViewToGenerate> LimitedViews { get; set; } = new();
}

public class LimitedViewToGenerate
{
    public string Name { get; set; }
    public string OverrideReturnTypeToUseLimitedView { get; set; }
}

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