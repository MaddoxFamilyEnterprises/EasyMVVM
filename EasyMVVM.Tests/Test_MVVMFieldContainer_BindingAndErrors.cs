namespace EasyMVVM.Tests;
using Xunit;
using EasyMVVM;
using System;
using System.ComponentModel;
using System.Linq.Expressions;

public class Test_MVVMFieldContainer_BindingAndErrors {
   private class TestModel : IDataModel {
      private string _testProperty;
      public string TestProperty {
         get => _testProperty;
         set {
            _testProperty = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TestProperty)));
         }
      }

      public event PropertyChangedEventHandler? PropertyChanged;
   }

   [Fact]
   public void BindToModel_ModelPropertyChanged_FieldValueUpdated() {
      var container = new MVVMFieldContainer();
      container.AddField<string>("TestField");
      var model = new TestModel();

      container.BindToModel("TestField", model, m => m.TestProperty);

      model.TestProperty = "New Value";

      Assert.Equal("New Value", container.GetValue<string>("TestField"));
   }

   [Fact]
   public void BindToModel_FieldValueChanged_ModelPropertyUpdated() {
      var container = new MVVMFieldContainer();
      container.AddField<string>("TestField");
      var model = new TestModel();

      container.BindToModel("TestField", model, m => m.TestProperty);

      container.SetValue("TestField", "New Value");

      Assert.Equal("New Value", model.TestProperty);
   }

   [Fact]
   public void UnbindFromModel_ModelPropertyChanged_FieldValueUnchanged() {
      var container = new MVVMFieldContainer();
      container.AddField<string>("TestField");
      var model = new TestModel();

      container.BindToModel("TestField", model, m => m.TestProperty);
      container.UnbindFromModel("TestField");

      model.TestProperty = "New Value";

      Assert.NotEqual("New Value", container.GetValue<string>("TestField"));
   }

   [Fact]
   public void BindToModel_NonexistentField_ThrowsArgumentException() {
      var container = new MVVMFieldContainer();
      var model = new TestModel();

      Assert.Throws<ArgumentException>(() =>
          container.BindToModel("NonexistentField", model, m => m.TestProperty));
   }

   [Fact]
   public void HasErrors_NoErrors_ReturnsFalse() {
      var container = new MVVMFieldContainer();
      container.AddField<int>("TestField");

      Assert.False(container.HasErrors);
   }

   [Fact]
   public void HasErrors_FieldHasError_ReturnsTrue() {
      var container = new MVVMFieldContainer();
      var field = container.AddField<int>("TestField")
            .WithConstraint(v => v > 0, "Value must be positive");

      container.SetValue("TestField", -1);

      Assert.True(container.HasErrors);
   }

   [Fact]
   public void GetErrors_FieldHasError_ReturnsErrors() {
      var container = new MVVMFieldContainer();
      var field = container.AddField<int>("TestField")
            .WithConstraint(v => v > 0, "Value must be positive");

      container.SetValue("TestField", -1);

      var errors = container.GetErrors("TestField");
      Assert.Single(errors);
      Assert.Contains("Value must be positive", (IEnumerable<string>) errors);
   }

   [Fact]
   public void GetErrors_NoPropertySpecified_ReturnsAllErrors() {
      var container = new MVVMFieldContainer();
      container.AddField<int>("Field1")
          .WithConstraint(v => v > 0, "Value must be positive");
      container.AddField<string>("Field2")
          .WithConstraint(v => !string.IsNullOrEmpty(v), "Value cannot be empty");

      container.SetValue("Field1", -1);
      container.SetValue("Field2", "");

      var errors = container.GetErrors(null);
      Assert.Equal(2, errors.Cast<string>().Count());
   }

   [Fact]
   public void ErrorsChanged_FieldValidationChanged_EventRaised() {
      var container = new MVVMFieldContainer();
      var field = container.AddField<int>("TestField")
            .WithConstraint(v => v > 0, "Value must be positive");

      var eventRaised = false;
      container.ErrorsChanged += (sender, args) => eventRaised = true;

      container.SetValue("TestField", -1);

      Assert.True(eventRaised);
   }
}
