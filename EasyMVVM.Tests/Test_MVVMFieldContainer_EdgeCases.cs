namespace EasyMVVM.Tests;
using Xunit;
using EasyMVVM;
using System;
using System.ComponentModel;

public class Test_MVVMFieldContainer_EdgeCases {
   [Fact]
   public void GetValue_NonexistentField_ReturnsDefaultValue() {
      var container = new MVVMFieldContainer();

      var result = container.GetValue<int>("NonexistentField");

      Assert.Equal(default(int), result);
   }

   [Fact]
   public void GetValue_WrongType_ThrowsInvalidCastException() {
      var container = new MVVMFieldContainer();
      container.AddField<int>("TestField");
      container.SetValue("TestField", 42);

      Assert.Throws<InvalidCastException>(() => container.GetValue<string>("TestField"));
   }

   [Fact]
   public void SetValue_NullValueForNullableType_Succeeds() {
      var container = new MVVMFieldContainer();
      container.AddField<int?>("TestField");

      container.SetValue<int?>("TestField", null);

      Assert.Null(container.GetValue<int?>("TestField"));
   }

   [Fact]
   public void SetValue_NullValueForReferenceType_Succeeds() {
      var container = new MVVMFieldContainer();
      container.AddField<string>("TestField");

      container.SetValue<string>("TestField", null);

      Assert.Null(container.GetValue<string>("TestField"));
   }

   [Fact]
   public void AddField_DuplicateFieldName_ThrowsArgumentException() {
      var container = new MVVMFieldContainer();
      container.AddField<int>("TestField");

      Assert.Throws<ArgumentException>(() => container.AddField<string>("TestField"));
   }

   [Fact]
   public void BindToModel_AlreadyBoundField_ThrowsInvalidOperationException() {
      var container = new MVVMFieldContainer();
      container.AddField<string>("TestField");
      var model1 = new TestModel();
      var model2 = new TestModel();

      container.BindToModel("TestField", model1, m => m.TestProperty);

      Assert.Throws<InvalidOperationException>(() =>
          container.BindToModel("TestField", model2, m => m.TestProperty));
   }

   [Fact]
   public void UnbindFromModel_NotBoundField_DoesNotThrow() {
      var container = new MVVMFieldContainer();
      container.AddField<string>("TestField");

      var exception = Record.Exception(() => container.UnbindFromModel("TestField"));

      Assert.Null(exception);
   }

   private class TestModel : IDataModel {
      public string TestProperty { get; set; }
      public event PropertyChangedEventHandler PropertyChanged;
   }
}