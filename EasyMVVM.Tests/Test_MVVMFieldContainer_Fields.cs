namespace EasyMVVM.Tests;

using Xunit;
using EasyMVVM;
using System;

public class Test_MVVMFieldContainer_Fields {
   [Fact]
   public void AddField_GetFieldValue_GetValueEqualsDefault() {
      var container = new MVVMFieldContainer();
      container.AddField<int>("TestField");

      var result = container.GetValue<int>("TestField");

      Assert.Equal(default(int), result);
   }

   [Fact]
   public void AddFieldAndSetFieldValue_GetFieldValue_GetValueEqualsSetValue() {
      var container = new MVVMFieldContainer();
      container.AddField<string>("TestField");
      container.SetValue("TestField", "TestValue");

      var result = container.GetValue<string>("TestField");

      Assert.Equal("TestValue", result);
   }

   [Fact]
   public void SetValueForNonexistentField_GetFieldValue_ReturnsDefaultValue() {
      var container = new MVVMFieldContainer();
      container.SetValue("NonexistentField", "TestValue");

      var result = container.GetValue<string>("NonexistentField");

      Assert.Null(result);
   }

   [Fact]
   public void AddMultipleFields_GetFieldNames_ReturnsAllFieldNames() {
      var container = new MVVMFieldContainer();
      container.AddField<int>("Field1");
      container.AddField<string>("Field2");
      container.AddField<bool>("Field3");

      var fieldNames = container.GetFieldNames();

      Assert.Contains("Field1", fieldNames);
      Assert.Contains("Field2", fieldNames);
      Assert.Contains("Field3", fieldNames);
      Assert.Equal(3, fieldNames.Count());
   }

   [Fact]
   public void AddField_HasField_ReturnsTrue() {
      var container = new MVVMFieldContainer();
      container.AddField<int>("TestField");

      var result = container.HasField("TestField");

      Assert.True(result);
   }

   [Fact]
   public void HasFieldForNonexistentField_ReturnsFalse() {
      var container = new MVVMFieldContainer();

      var result = container.HasField("NonexistentField");

      Assert.False(result);
   }
}