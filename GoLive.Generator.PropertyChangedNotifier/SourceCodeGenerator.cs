using System.Collections.Generic;
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
            
            source.AppendLine($"public {classToGen.Name}()");
            source.AppendOpenCurlyBracketLine();

            
            
            foreach (var coll in classToGen.Members.Where(f=>f.IsCollection))
            {
                var collTargetName = coll.Name.FirstCharToUpper();
                source.AppendLine($"{collTargetName} = new();");
                source.AppendLine($"{collTargetName}.CollectionChanged += (in ObservableCollections.NotifyCollectionChangedEventArgs<{coll.CollectionType.Name}> eventArgs) => Changes.Upsert($\"{collTargetName}.{{eventArgs.NewStartingIndex}}\", eventArgs.NewItem);");
            }
            source.AppendCloseCurlyBracketLine();
            
            List<ITypeSymbol> typeAccessorsToCreate = new();

            foreach (var s in classToGen.Members.Where(f => !f.IsCollection).Select(f => f.Type).Distinct())
            {
                if (!typeAccessorsToCreate.Contains(s))
                {
                    typeAccessorsToCreate.Add(s);
                }
            }

            foreach (var s in classToGen.Members.Where(f => f.CollectionType != null).Select(f => f.CollectionType).Distinct())
            {
                if (!typeAccessorsToCreate.Contains(s))
                {
                    typeAccessorsToCreate.Add(s);
                }
            }
            
            foreach (var s in typeAccessorsToCreate)
            {
                var collTargetName = s.Name.FirstCharToUpper();
                source.AppendLine($"TypeAccessor {collTargetName}TypeAccessor = TypeAccessor.Create(typeof({s}));");
                source.AppendLine();
            }
            
            source.AppendLine(@"public event PropertyChangedEventHandler? PropertyChanged;

protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = """")
{
    if (EqualityComparer<T>.Default.Equals(field, value)) return false;
    field = value;
    if (EnableChangeTracking){
        OnPropertyChanged(propertyName);
        Changes.Upsert(propertyName,value);
    }
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
            source.AppendLine($"public ObservableCollections.ObservableList<{item.CollectionType}> {itemName.FirstCharToUpper()}");
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
            
            if (!item.WriteOnly)
            {
                source.AppendLine($"get => {itemName};");
            }

            if (item.IsScoped)
            {
                source.AppendLine($@"set
        {{
            if (value != null && !string.IsNullOrWhiteSpace(value.Id))
            {{
                if ({itemName} != null && !string.IsNullOrWhiteSpace({itemName}.Id) && Scopes.Contains({itemName}.Id))
                {{
                    Scopes.Remove({itemName}.Id);
                }}
                Scopes.Add(value.Id);
                SetField(ref {itemName}, value.Id);
            }}
            else
            {{
                if ({itemName} != null && !string.IsNullOrWhiteSpace({itemName}.Id) && Scopes.Contains({itemName}.Id))
                {{
                    Scopes.Remove({itemName}.Id);
                    SetField(ref {itemName}, string.Empty);
                }}
            }}
        }}");
            } else if (!item.ReadOnly)
            {
                source.AppendLine($"set => SetField(ref {itemName}, value);");
            }
            
            source.AppendCloseCurlyBracketLine();
        }
    }
}