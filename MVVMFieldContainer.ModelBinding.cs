namespace EasyMVVM;
using System.ComponentModel;
using System.Reflection;

public partial class MVVMFieldContainer {
   private class ModelBinding {
      public object? Model { get; set; }
      public PropertyInfo? PropertyInfo { get; set; }
      public PropertyChangedEventHandler? PropertyChangedHandler { get; set; }
   }
}
