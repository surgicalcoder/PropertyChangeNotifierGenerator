using System.ComponentModel;
using System.Runtime.CompilerServices;
using GoLive.Generator.PropertyChangedNotifier.Utilities;
using System.Collections.Specialized;
using FastMember;

namespace GoLive.Generator.ProperyChangedNotifier.Playground
{
    public partial class IgnoreTest : INotifyPropertyChanged
    {
        public IgnoreTest()
        {
        }

        TypeAccessor StringTypeAccessor = TypeAccessor.Create(typeof(string));
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

        public string ReadonlyTest { get => readonlyTest; }

        public string EverythingElseTest { get => everythingElseTest; set => SetField(ref everythingElseTest, value); }
    }
}