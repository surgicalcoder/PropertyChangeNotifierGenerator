using System.ComponentModel;
using System.Runtime.CompilerServices;
using GoLive.Generator.PropertyChangedNotifier.Utilities;
using System.Collections.Specialized;
using FastMember;

namespace GoLive.Generator.ProperyChangedNotifier.Playground
{
    public partial class FourthItem : INotifyPropertyChanged
    {
        public FourthItem()
        {
        }

        TypeAccessor StringTypeAccessor = TypeAccessor.Create(typeof(string));
        TypeAccessor RefTypeAccessor = TypeAccessor.Create(typeof(GoLive.Saturn.Data.Entities.Ref<GoLive.Generator.ProperyChangedNotifier.Playground.MainItem>));
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;
            field = value;
            if (EnableChangeTracking)
            {
                OnPropertyChanged(propertyName);
                Changes.Upsert(propertyName, value);
            }

            return true;
        }

        public string Blarg { get => blarg; set => SetField(ref blarg, value); }

        public GoLive.Saturn.Data.Entities.Ref<GoLive.Generator.ProperyChangedNotifier.Playground.MainItem> MainItem
        {
            get => mainItem;
            set
            {
                if (value != null && !string.IsNullOrWhiteSpace(value.Id))
                {
                    if (mainItem != null && !string.IsNullOrWhiteSpace(mainItem.Id) && Scopes.Contains(mainItem.Id))
                    {
                        Scopes.Remove(mainItem.Id);
                    }

                    Scopes.Add(value.Id);
                    SetField(ref mainItem, value.Id);
                }
                else
                {
                    if (mainItem != null && !string.IsNullOrWhiteSpace(mainItem.Id) && Scopes.Contains(mainItem.Id))
                    {
                        Scopes.Remove(mainItem.Id);
                        SetField(ref mainItem, string.Empty);
                    }
                }
            }
        }
    }
}