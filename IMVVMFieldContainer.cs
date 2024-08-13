namespace EasyMVVM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;

/// <summary>
/// Represents a data model that can be bound to an MVVMFieldContainer.
/// </summary>
public interface IDataModel : INotifyPropertyChanged {
}

/// <summary>
/// Represents a container for dynamic fields in an MVVM architecture.
/// Implements property change notification and data error information.
/// Supports binding to IDataModel instances.
/// </summary>
public interface IMVVMFieldContainer : INotifyPropertyChanged, INotifyDataErrorInfo {
   /// <summary>
   /// Adds a new field to the container.
   /// </summary>
   /// <typeparam name="T">The type of the field value.</typeparam>
   /// <param name="name">The name of the field.</param>
   /// <returns>A FieldBuilder to configure the field.</returns>
   FieldBuilder<T> AddField<T>(string name);

   /// <summary>
   /// Gets the value of a field.
   /// </summary>
   /// <typeparam name="T">The type of the field value.</typeparam>
   /// <param name="name">The name of the field.</param>
   /// <returns>The value of the field, or default(T) if the field doesn't exist.</returns>
   T? GetValue<T>(string name);

   /// <summary>
   /// Sets the value of a field.
   /// </summary>
   /// <typeparam name="T">The type of the field value.</typeparam>
   /// <param name="name">The name of the field.</param>
   /// <param name="value">The value to set.</param>
   void SetValue<T>(string name, T value);

   /// <summary>
   /// Gets the names of all fields in the container.
   /// </summary>
   /// <returns>An enumerable of field names.</returns>
   IEnumerable<string> GetFieldNames();

   /// <summary>
   /// Checks if a field with the given name exists in the container.
   /// </summary>
   /// <param name="name">The name of the field to check.</param>
   /// <returns>True if the field exists, false otherwise.</returns>
   bool HasField(string name);

   /// <summary>
   /// Binds a field to a property of an IDataModel.
   /// </summary>
   /// <typeparam name="TModel">The type of the data model.</typeparam>
   /// <typeparam name="TProperty">The type of the property to bind to.</typeparam>
   /// <param name="fieldName">The name of the field to bind.</param>
   /// <param name="model">The data model instance.</param>
   /// <param name="propertyExpression">An expression representing the property to bind to.</param>
   void BindToModel<TModel, TProperty>(string fieldName, TModel model, Expression<Func<TModel, TProperty>> propertyExpression) where TModel : IDataModel;

   /// <summary>
   /// Unbinds a field from its data model.
   /// </summary>
   /// <param name="fieldName">The name of the field to unbind.</param>
   void UnbindFromModel(string fieldName);
}
