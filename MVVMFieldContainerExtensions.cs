namespace EasyMVVM;
using System;
using System.Windows;

public static class MVVMFieldContainerExtensions {
   public static void RegisterDependencyProperties(this MVVMFieldContainer container) {
      foreach( var fieldName in container.GetFieldNames() ) {
         RegisterProperty(container.GetType(), fieldName);
      }
   }

   private static void RegisterProperty(Type ownerType, string propertyName) {
      var property = DependencyProperty.Register(propertyName, typeof(object), ownerType, new PropertyMetadata(null));
   }
}
