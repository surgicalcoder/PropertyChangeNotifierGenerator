using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace GoLive.Generator.Saturn
{
    public static class SourceCodeGenerator
    {
        public static void Generate(SourceStringBuilder source, ClassToGenerate classToGen)
        {
            source.AppendLine("using System.ComponentModel;");
            source.AppendLine("using System.Runtime.CompilerServices;");
            source.AppendLine("using GoLive.Generator.Saturn.Resources;");
            source.AppendLine("using GoLive.Saturn.Data.Entities;");
            source.AppendLine("using System.Collections.Specialized;");
            source.AppendLine("using FastMember;");

            source.AppendLine($"namespace {classToGen.Namespace};");


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

            bool addedRefAccessor = false;
            
            foreach (var s in typeAccessorsToCreate)
            {
                if (s.Name == "Ref" && ((INamedTypeSymbol)s).ConstructedFrom.ToString() == "GoLive.Saturn.Data.Entities.Ref<T>")
                {
                    if (!addedRefAccessor)
                    {
                        source.AppendLine($"TypeAccessor RefTypeAccessor = TypeAccessor.Create(typeof(GoLive.Saturn.Data.Entities.Ref<>));");
                        source.AppendLine();
                        addedRefAccessor = true;
                    }
                    continue;
                }
                var collTargetName = s.Name.FirstCharToUpper();
                source.AppendLine($"TypeAccessor {collTargetName}TypeAccessor = TypeAccessor.Create(typeof({s}));");
                source.AppendLine();
            }
            
            /*source.AppendLine(@"public event PropertyChangedEventHandler? PropertyChanged;

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
}");*/

            foreach (var member in classToGen.Members)
            {
                if (member.IsCollection)
                {
                    GenerateCollectionMember(source, member);
                }
                else
                {
                    GenerateNormalMember(source, member);
                }
                
            }
            source.AppendLine("}");

            source.DecreaseIndent();
            source.AppendLine();
            source.AppendLine();
            
            
            foreach (var item in classToGen.Members.Where(r => r.LimitedViews.Any()).SelectMany(f => f.LimitedViews.Select(r => new { classDef = f, LimitedView = r }))
                         .GroupBy(e => e.LimitedView.Name))
            {
                source.AppendLine($"public partial class {classToGen.Name}_{item.Key} : IUpdatableFrom<{classToGen.Name}> ");
                source.AppendOpenCurlyBracketLine();

                foreach (var v1 in item)
                {
                    if (string.IsNullOrWhiteSpace(v1.LimitedView.OverrideReturnTypeToUseLimitedView))
                    {
                        source.AppendLine($"public {v1.classDef.Type} {v1.classDef.Name.FirstCharToUpper()} {{get;set;}}");
                    }
                    else
                    {
                        source.AppendLine($"public {v1.classDef.Type}_{v1.LimitedView.OverrideReturnTypeToUseLimitedView} {v1.classDef.Name.FirstCharToUpper()} {{get;set;}}");
                    }
                }
                
                source.AppendLine();
                
                source.AppendLine();
                source.AppendLine($"public void UpdateFrom({classToGen.Name} source)");
                source.AppendOpenCurlyBracketLine();
                foreach (var v1 in item)
                {
                    if (string.IsNullOrWhiteSpace(v1.LimitedView.OverrideReturnTypeToUseLimitedView))
                    {
                        source.AppendLine($"this.{v1.classDef.Name.FirstCharToUpper()} = source.{v1.classDef.Name.FirstCharToUpper()};");
                    }
                    else
                    {
                        source.AppendLine($"this.{v1.classDef.Name.FirstCharToUpper()} = {v1.classDef.Type}_{v1.LimitedView.OverrideReturnTypeToUseLimitedView}.Generate(source.{v1.classDef.Name.FirstCharToUpper()}); ");
                    }
                }
                source.AppendLine();
                source.AppendCloseCurlyBracketLine();
                
                source.AppendLine($"public static {classToGen.Name}_{item.Key} Generate({classToGen.Name} source)");
                source.AppendOpenCurlyBracketLine();
                
                source.AppendLine($"var retr = new {classToGen.Name.FirstCharToUpper()}_{item.Key}();");
                source.AppendLine("retr.UpdateFrom(source);");
                source.AppendLine("return retr;");
                source.AppendCloseCurlyBracketLine();
                
                
                source.AppendCloseCurlyBracketLine();
                
            }
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