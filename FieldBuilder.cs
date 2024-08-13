namespace EasyMVVM;
using System;

public class FieldBuilder<T> {
   private readonly Field<T> _field;
   private readonly MVVMFieldContainer _container;

   public FieldBuilder(Field<T> field, MVVMFieldContainer container) {
      _field = field;
      _container = container;
   }

   public FieldBuilder<T> WithConstraint(Func<T, bool> constraint, string errorMessage) {
      _field.AddConstraint(constraint, errorMessage);
      return this;
   }
}

