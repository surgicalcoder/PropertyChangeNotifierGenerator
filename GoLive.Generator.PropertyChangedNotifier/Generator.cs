using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Microsoft.CodeAnalysis;

namespace GoLive.Generator.PropertyChangedNotifier
{
    [Generator]
    public class Generator : ISourceGenerator
    {
        private string additionalFileContents;
        public void Initialize(GeneratorInitializationContext context)
        {
            if (additionalFileContents is not null)
            {
                return;
            }

            using var reader = new StreamReader(GetType().Assembly.GetManifestResourceStream(EmbeddedResources.Resources_AdditionalFiles_cs), Encoding.UTF8);
            additionalFileContents = reader.ReadToEnd();
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var config = LoadConfig(context);

            try
            {
                var compilation = context.Compilation;

                var classesToGen = compilation.SyntaxTrees.Select(t => compilation.GetSemanticModel(t))
                    .Select(e => Scanner.ScanForEligibleClasses(e, config, context))
                    .SelectMany(c => c)
                    .ToArray();

                foreach (var classToGenerate in classesToGen)
                {
                    var sourceStringBuilder = new SourceStringBuilder();
                    SourceCodeGenerator.Generate(sourceStringBuilder, config, context, classToGenerate);

                    if (sourceStringBuilder.ToString() is { Length: > 0 } s)
                    {
                        File.WriteAllText(classToGenerate.Filename.Replace(".cs", ".generated.cs"), s);
                    }
                }

                if (config.AdditionalFilesLocation is not null)
                {
                    File.WriteAllText(config.AdditionalFilesLocation, OutputAdditionalFiles(config));
                }
                else
                {
                    context.AddSource("AdditionalFiles.cs", OutputAdditionalFiles(config));
                }
            }
            catch (Exception e)
            {
                if (!string.IsNullOrWhiteSpace(config.LogFile))
                {
                    File.WriteAllText(config.LogFile, e.ToString());
                }
            }
        }

        private string OutputAdditionalFiles(Settings settings)
        {
            var builder = new StringBuilder();
            builder.AppendLine(@"using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;");
            builder.AppendLine($"namespace  {settings.AdditionalFilesNamespace} {{");

            builder.AppendLine(additionalFileContents);
            builder.AppendLine("}");

            return builder.ToString();
        }

        private Settings LoadConfig(GeneratorExecutionContext context)
        {
            var configFilePath = context.AdditionalFiles.FirstOrDefault(e => e.Path.EndsWith("PropertyChangeNotifier.json"));

            if (configFilePath == null) return null;

            var jsonString = File.ReadAllText(configFilePath.Path);
            
            var config = JsonSerializer.Deserialize<Settings>(jsonString, new JsonSerializerOptions { AllowTrailingCommas = true });
            var configFileDirectory = Path.GetDirectoryName(configFilePath.Path);

            if (!string.IsNullOrWhiteSpace(config.AdditionalFilesLocation))
            {
                config.AdditionalFilesLocation.MakeFullyQualified(configFilePath);
            }

            if (string.IsNullOrWhiteSpace(config.AdditionalFilesNamespace))
            {
                config.AdditionalFilesNamespace = "GoLive.Generator.PropertyChangedNotifier.Utilities";
            }

            var fullPath = Path.Combine(configFileDirectory, config.LogFile);
            config.LogFile = Path.GetFullPath(fullPath);

            return config;
        }
    }
}