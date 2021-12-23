using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.CodeAnalysis;

namespace GoLive.Generator.PropertyChangedNotifier
{
    [Generator]
    public class Generator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var config = LoadConfig(context);

            try
            {
              //  var source = new SourceStringBuilder();
                var compilation = context.Compilation;

                var controllerRoutes = compilation.SyntaxTrees.Select(t => compilation.GetSemanticModel(t))
                    .Select(e => Scanner.ScanForControllers(e, config))
                    .SelectMany(c => c)
                    .ToArray();

                foreach (var classToGenerate in controllerRoutes)
                {
                    var sourceStringBuilder = new SourceStringBuilder();
                    SourceCodeGenerator.Generate(sourceStringBuilder, config, context, classToGenerate);
                    if (sourceStringBuilder.ToString() is {Length: > 0} s)
                    {
                        File.WriteAllText(classToGenerate.Filename.Replace(".cs", ".generated.cs"), s);
                    }
                }
                
                /*SourceCodeGenerator.Generate(source, config, context, controllerRoutes);

                if (source.ToString() is {Length: > 0} s)
                {
                    //File.WriteAllText(config.OutputFile, s);
                }*/
            }
            catch (Exception e)
            {
                File.WriteAllText(config.LogFile, e.ToString());
            }
        }

        private Settings LoadConfig(GeneratorExecutionContext context)
        {
            var configFilePath =
                context.AdditionalFiles.FirstOrDefault(e => e.Path.EndsWith("PropertyChangeNotifier.json"));

            if (configFilePath == null)
            {
                return null;
            }

            var jsonString = File.ReadAllText(configFilePath.Path);
            var config = JsonSerializer.Deserialize<Settings>(jsonString);
            var configFileDirectory = Path.GetDirectoryName(configFilePath.Path);
            //config.OutputFile = Path.Combine(configFileDirectory, "output-generated.txt");

            var fullPath = Path.Combine(configFileDirectory, config.LogFile);
            config.LogFile = Path.GetFullPath(fullPath);

            return config;
        }
    }
}