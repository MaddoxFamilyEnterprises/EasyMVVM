namespace EasyMVVM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

public class Field<T> : INotifyPropertyChanged, IField {
   private T? _value;
   private readonly List<(Func<T, bool> Constraint, string ErrorMessage)> _constraints = [];
   private readonly List<string> _errors = [];

   public required string Name { get; init; }

   public T? Value {
      get => _value;
      set {
         if( !EqualityComparer<T>.Default.Equals(_value, value) ) {
            _value = value;
            OnPropertyChanged();
            Validate();
         }
      }
   }

   public bool HasErrors => _errors.Count > 0;
   public IEnumerable<string> Errors => _errors;

   public event PropertyChangedEventHandler? PropertyChanged;

   public void AddConstraint(Func<T, bool> constraint, string errorMessage) {
      _constraints.Add((constraint, errorMessage));
   }

   public void Validate() {
      _errors.Clear();
      if( _value != null ) {
         foreach( var (constraint, errorMessage) in _constraints ) {
            if( !constraint(_value) ) {
               _errors.Add(errorMessage);
            }
         }
      }
   }

   protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
   }
}
