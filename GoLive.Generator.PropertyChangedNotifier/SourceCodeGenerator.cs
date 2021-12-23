using System.Linq;
using GoLive.Generator.PropertyChangedNotifier.Model;
using Microsoft.CodeAnalysis;

namespace GoLive.Generator.PropertyChangedNotifier
{
    public static class SourceCodeGenerator
    {
        public static void Generate(SourceStringBuilder source, Settings config, GeneratorExecutionContext context,
            ClassToGenerate controllerRoutes)
        {
            source.AppendLine("using System.ComponentModel;");
            source.AppendLine("using System.Runtime.CompilerServices;");
            
            
            source.AppendLine($"namespace {controllerRoutes.Namespace}");
            source.AppendOpenCurlyBracketLine();
            source.AppendIndent();
            
            source.AppendLine($"public partial class {controllerRoutes.Name} {{");
            source.AppendIndent();
            
            source.AppendLine(@"public event PropertyChangedEventHandler? PropertyChanged;

protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = """")
{
    if (EqualityComparer<T>.Default.Equals(field, value)) return false;
    field = value;
    OnPropertyChanged(propertyName);
    Changes.Add(propertyName,value);
    return true;
}");
            
            foreach (var memberToGenerates in controllerRoutes.Members.GroupBy(e=>e.Name.Substring(4)))
            {
                var item = memberToGenerates.FirstOrDefault();
                var itemName = item.Name;
                source.AppendLine($"public {item.Type} {itemName.FirstCharToUpper()}");
                source.AppendOpenCurlyBracketLine();
                source.AppendLine($"get => {itemName};");
                source.AppendLine($"set => SetField(ref {itemName}, value);");

                source.AppendCloseCurlyBracketLine();
            }
            
            foreach (var memberToGenerate in controllerRoutes.Members)
            {
             //   source.AppendLine($"{memberToGenerate.Name} :: {memberToGenerate.Type}");
            }
            source.AppendLine("}");
            
            source.DecreaseIndent();
            source.AppendCloseCurlyBracketLine();
        
        
        }
    }
}