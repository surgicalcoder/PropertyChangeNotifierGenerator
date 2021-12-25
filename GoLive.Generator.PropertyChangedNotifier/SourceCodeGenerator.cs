using System.Linq;
using GoLive.Generator.PropertyChangedNotifier.Model;
using Microsoft.CodeAnalysis;

namespace GoLive.Generator.PropertyChangedNotifier
{
    public static class SourceCodeGenerator
    {
        public static void Generate(SourceStringBuilder source, Settings config, GeneratorExecutionContext context,
            ClassToGenerate classToGen)
        {
            source.AppendLine("using System.ComponentModel;");
            source.AppendLine("using System.Runtime.CompilerServices;");
            source.AppendLine("using GoLive.Generator.PropertyChangedNotifier.Utilities;");
            source.AppendLine("using System.Collections.Specialized;");
            source.AppendLine("using FastMember;");

            source.AppendLine($"namespace {classToGen.Namespace}");
            source.AppendOpenCurlyBracketLine();
            source.AppendIndent();

            source.AppendLine($"public partial class {classToGen.Name} : INotifyPropertyChanged {{");
            source.AppendIndent();

            
            source.AppendLine("public void GeneratedCtor()");
            source.AppendOpenCurlyBracketLine();
            foreach (var coll in classToGen.Members.Where(f=>f.IsCollection))
            {
                var collTargetName = coll.Name.FirstCharToUpper();
                source.AppendLine($"{collTargetName} = new ();");
                source.AppendLine($"{collTargetName}.ItemPropertyChanged += {collTargetName}OnItemPropertyChanged;");
                source.AppendLine($"{collTargetName}.CollectionChanged += {collTargetName}OnCollectionChanged;");
            }
            source.AppendCloseCurlyBracketLine();
            
            
            foreach (var coll in classToGen.Members.Where(f => f.IsCollection))
            {
                var collTargetName = coll.Name.FirstCharToUpper();
                source.AppendLine($"TypeAccessor {coll.CollectionType.Name}TypeAccessor = TypeAccessor.Create(typeof({coll.CollectionType.ToString()}));");
                source.AppendLine();
                
                source.AppendLine($"private void {collTargetName}OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)");
                source.AppendOpenCurlyBracketLine();
                source.AppendLine($"Changes.Add($\"{collTargetName}.{{e.OldStartingIndex}}\", e.NewItems);");
                source.AppendCloseCurlyBracketLine();
                
                source.AppendLine();

                source.AppendLine($"private void {collTargetName}OnItemPropertyChanged(object? sender, ItemPropertyChangedEventArgs e)");
                source.AppendOpenCurlyBracketLine();

                source.AppendLine($"Changes.Add($\"{collTargetName}.{{e.CollectionIndex}}.{{e.PropertyName}}\", {coll.CollectionType.Name}TypeAccessor[{collTargetName}[e.CollectionIndex], e.PropertyName]);");
                source.AppendCloseCurlyBracketLine();

            }

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

            foreach (var member in classToGen.Members)
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