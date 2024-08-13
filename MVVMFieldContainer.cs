
namespace EasyMVVM;

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

public partial class MVVMFieldContainer : INotifyPropertyChanged, INotifyDataErrorInfo, IMVVMFieldContainer {
   private readonly Dictionary<string, object> _fields = new Dictionary<string, object>();
   private readonly Dictionary<string, ModelBinding> _modelBindings = new Dictionary<string, ModelBinding>();

   public event PropertyChangedEventHandler? PropertyChanged;
   public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

   public bool HasErrors => _fields.Values.OfType<IField>().Any(f => f.HasErrors);

   public MVVMFieldContainer() { }

   public FieldBuilder<T> AddField<T>(string name) {
      var field = new Field<T> { Name = name };
      _fields [ name ] = field;
      OnPropertyChanged(name);
      return new FieldBuilder<T>(field, this);
   }
   public T? GetValue<T>(string name) {
      if( _fields.TryGetValue(name, out var field) && field is Field<T> typedField ) {
         return typedField.Value;
      }

      return default;
   }
   public IEnumerable GetErrors(string? propertyName) {
      if( string.IsNullOrEmpty(propertyName) ) {
         return _fields.Values.OfType<IField>().SelectMany(f => f.Errors).ToList();
      }

      if( _fields.TryGetValue(propertyName, out var field) && field is IField typedField ) {
         return typedField.Errors.ToList();
      }

      return new List<string>();
   }
   private void ValidateField(string name) {
      if( _fields.TryGetValue(name, out var field) && field is IField typedField ) {
         typedField.Validate();
         ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(name));
      }
   }
   protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
   }
   public IEnumerable<string> GetFieldNames() => _fields.Keys;
   public bool HasField(string name) => _fields.ContainsKey(name);
   public void BindToModel<TModel, TProperty>(string fieldName, TModel model, Expression<Func<TModel, TProperty>> propertyExpression) where TModel : IDataModel {
      if( !_fields.TryGetValue(fieldName, out var fieldObj) || !( fieldObj is IField field ) ) {
         throw new ArgumentException($"Field '{fieldName}' not found.", nameof(fieldName));
      }

      var propertyInfo = (PropertyInfo)((MemberExpression)propertyExpression.Body).Member;
      var getter = propertyExpression.Compile();

      void ModelPropertyChanged(object? sender, PropertyChangedEventArgs e) {
         if( e.PropertyName == propertyInfo.Name ) {
            SetValue(fieldName, getter(model));
         }
      }

      model.PropertyChanged += ModelPropertyChanged;

      _modelBindings [ fieldName ] = new ModelBinding {
         Model = model,
         PropertyInfo = propertyInfo,
         PropertyChangedHandler = ModelPropertyChanged
      };

      // Initial sync from model to field
      SetValue(fieldName, getter(model));
   }
   public void UnbindFromModel(string fieldName) {
      if( _modelBindings.TryGetValue(fieldName, out var binding) ) {
         if( binding.Model is INotifyPropertyChanged notifyPropertyChanged && binding.PropertyChangedHandler != null ) {
            notifyPropertyChanged.PropertyChanged -= binding.PropertyChangedHandler;
         }

         _modelBindings.Remove(fieldName);
      }
   }
   public void SetValue<T>(string name, T value) {
      if( _fields.TryGetValue(name, out var fieldObj) && fieldObj is Field<T> field ) {
         field.Value = value;
         OnPropertyChanged(name);
         ValidateField(name);
      }

      if( _modelBindings.TryGetValue(name, out var binding) && binding.PropertyInfo != null && binding.Model != null ) {
         binding.PropertyInfo.SetValue(binding.Model, value);
      }
   }
}






