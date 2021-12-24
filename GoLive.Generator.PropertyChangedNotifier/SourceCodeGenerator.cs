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
            source.AppendLine("using GoLive.Generator.PropertyChangedNotifier.Utilities;");

            source.AppendLine($"namespace {controllerRoutes.Namespace}");
            source.AppendOpenCurlyBracketLine();
            source.AppendIndent();

            source.AppendLine($"public partial class {controllerRoutes.Name} : INotifyPropertyChanged {{");
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

            foreach (var member in controllerRoutes.Members)
                if (member.IsCollection)
                    GenerateCollectionMember(source, member);
                else
                    GenerateNormalMember(source, member);

            source.AppendLine("}");

            source.DecreaseIndent();
            source.AppendCloseCurlyBracketLine();
        }

        private static void GenerateCollectionMember(SourceStringBuilder source, MemberToGenerate item)
        {
            var itemName = item.Name;
            source.AppendLine($"public FullyObservableCollection<{item.CollectionType}> {itemName.FirstCharToUpper()}");
            source.AppendOpenCurlyBracketLine();
            source.AppendLine($"get => {itemName};");
            source.AppendLine($"set => SetField(ref {itemName}, value);");
            source.AppendCloseCurlyBracketLine();
        }

        private static void GenerateNormalMember(SourceStringBuilder source, MemberToGenerate item)
        {
            var itemName = item.Name;
            source.AppendLine($"public {item.Type} {itemName.FirstCharToUpper()}");
            source.AppendOpenCurlyBracketLine();
            source.AppendLine($"get => {itemName};");
            source.AppendLine($"set => SetField(ref {itemName}, value);");
            source.AppendCloseCurlyBracketLine();
        }
    }
}