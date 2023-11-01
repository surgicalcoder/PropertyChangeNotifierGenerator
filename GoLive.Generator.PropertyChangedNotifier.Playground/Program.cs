using System.ComponentModel;
using System.Runtime.CompilerServices;
using GoLive.Saturn.Data;
using GoLive.Saturn.Data.Abstractions;
using ObservableCollections;

namespace GoLive.Generator.PropertyChangedNotifier.Playground;

public class Program
{

        static async Task Main(string[] args)
        {
                FourthItem fr = new();
                //var wibble = FourthItem_View1.Generate(fr);
                
                /*MainItem item = new MainItem();
                item.Strings = new ObservableList<string>();
                item.AnotherString = new ObservableList<string>();
                item.Strings.CollectionChanged += (in NotifyCollectionChangedEventArgs<string> eventArgs) => item.Changes.Upsert($"Strings.{eventArgs.NewStartingIndex}", eventArgs.NewItems);
                
                item.Name = "init name value";
                item.Description = "init desc value";
                item.AnotherString = new();
                item.AnotherString.Add("Wibble 1");
                item.Id = DateTime.UtcNow.ToString("O");
                item.EnableChangeTracking = true;
                item.Changes.Clear();
                
                
                var fi = new FourthItem();
                fi.EnableChangeTracking = true;
                fi.MainItem = item;
                item.Strings.Add("strings 1");
                fi.MainItem = null;
                

                item.Name = "Updated Name";
                item.AnotherString.Add("Wibble 2");
                item.AnotherString.Insert(0, "Wibble 3");
                item.Strings.Add("string 2");
                item.Strings[0] = "overwritten";
                Console.WriteLine();*/

                RepositoryOptions options = new()
                {
                        ConnectionString = "mongodb://localhost/ReferenceTests",
                        GetCollectionName = type => type.Name
                };

                var repo = new Repository(options);

                var scope = new ReferenceTestScope();
                scope.Name = $"Test scope created at {DateTime.UtcNow:f}";
                await repo.Insert(scope);

                ReferenceTest2 ref2 = new() { TestName = $"Test item2 created at {DateTime.UtcNow:f}" };
                await repo.Insert(ref2);
                
                ReferenceTest3 ref3 =new() { AnotherName = $"Test item2 created at {DateTime.UtcNow:f}" };
                await repo.Insert(ref3);

                ReferenceTest1 ref1 = new ReferenceTest1();
                ref1.Test2 = ref2;
                ref1.Test3 = ref3;
                ref1.Scope = scope;
                await repo.Insert(ref1);
        }
}
/*public class PrimativeWrapper<T> : INotifyPropertyChanged
{
        private T primitive;
        public PrimativeWrapper() { }
        public PrimativeWrapper(T primative)
        {
                Primative = primative;
        }

        public virtual T Primative
        {
                get => primitive;
                set
                {
                        if (!EqualityComparer<T>.Default.Equals(primitive, value))
                        {
                                primitive = value;
                                SetField(ref primitive, value);
                                OnPropertyChanged(nameof(Primative));
                        }
                        
                }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
                if (EqualityComparer<T>.Default.Equals(field, value)) return false;
                field = value;
                OnPropertyChanged(propertyName);
                return true;
        }
}
public static class PrimativeWrapperExtensions
{
        public static IEnumerable<PrimativeWrapper<T>> WrapPrimatives<T>(this IEnumerable<T> enumerable)
        {
                if (enumerable == null)
                {
                        throw new ArgumentNullException(nameof(enumerable));
                }
                return WrapPrimativesImpl<T>(enumerable);
        }

        private static IEnumerable<PrimativeWrapper<T>> WrapPrimativesImpl<T>(IEnumerable<T> enumerable)
        {
                foreach (var item in enumerable)
                {
                        yield return new PrimativeWrapper<T>(item);
                }
        }
}*/
/*
public class StringContainer : INotifyPropertyChanged
{
        public static implicit operator string(StringContainer container)
        {
                return container?.value;
        }
        
        public static implicit operator StringContainer(string input)
        {
                return new StringContainer { Value = input };
        }
        
        private string value;

        public string Value
        {
                get => value;
                set
                {
                        if (!EqualityComparer<string>.Default.Equals(value, this.value))
                        {
                                this.value = value;  
                                OnPropertyChanged("Value");
                        }
                }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
                if (EqualityComparer<T>.Default.Equals(field, value)) return false;

                field = value;
                OnPropertyChanged(propertyName);

                return true;
        }
}
*/

