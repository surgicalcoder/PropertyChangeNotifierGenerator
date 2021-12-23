using System.Linq;
using GoLive.Generator.PropertyChangedNotifier.Model;
using Microsoft.CodeAnalysis;

namespace GoLive.Generator.PropertyChangedNotifier
{
    public static class SourceCodeGenerator
    {
        public static void Generate(SourceStringBuilder source, Settings config, GeneratorExecutionContext context,
            ClassToGenerate[] controllerRoutes)
        {
            source.AppendLine("using System.ComponentModel;");
            source.AppendLine("using System.Runtime.CompilerServices;");
            

            
            foreach (var classToGenerate in controllerRoutes)
            {
                
                source.AppendLine($"namespace {classToGenerate.Namespace}");
                source.AppendOpenCurlyBracketLine();
                source.AppendIndent();
                
                source.AppendLine($"public partial class {classToGenerate.Name} {{");
                source.AppendIndent();
                
                source.AppendLine("public event PropertyChangedEventHandler? PropertyChanged;\n\nprotected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));\n\nprotected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = \"\")\n{\n    if (EqualityComparer<T>.Default.Equals(field, value)) return false;\n    field = value;\n    OnPropertyChanged(propertyName);\n    return true;\n}");
                
                foreach (var memberToGenerates in classToGenerate.Members.GroupBy(e=>e.Name.Substring(4)))
                {
                    var item = memberToGenerates.FirstOrDefault();
                    var itemName = item.Name;//.Substring(4);
                    source.AppendLine($"public {item.Type} {itemName.FirstCharToUpper()}");
                    source.AppendOpenCurlyBracketLine();
                    source.AppendLine($"get => {itemName};");
                    source.AppendLine($"set => SetField(ref {itemName}, value);");

                    source.AppendCloseCurlyBracketLine();
                }
                
                foreach (var memberToGenerate in classToGenerate.Members)
                {
                 //   source.AppendLine($"{memberToGenerate.Name} :: {memberToGenerate.Type}");
                }
                source.AppendLine("}");
                
                source.DecreaseIndent();
                source.AppendCloseCurlyBracketLine();
            }
            
        }
    }
}