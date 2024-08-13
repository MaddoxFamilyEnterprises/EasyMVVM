namespace EasyMVVM.Tests;

using EasyMVVM;

using Xunit;
using EasyMVVM;
using System;

public class Test_FieldBuilder {
   [Fact]
   public void WithConstraint_AddsConstraintToField() {
      var container = new MVVMFieldContainer();
      var field = new Field<int> { Name = "TestField" };
      var builder = new FieldBuilder<int>(field, container);

      builder.WithConstraint(v => v > 0, "Value must be positive");
      field.Value = -5;

      Assert.True(field.HasErrors);
      Assert.Single(field.Errors);
      Assert.Contains("Value must be positive", field.Errors);
   }

   [Fact]
   public void WithConstraint_ChainMultipleConstraints() {
      var container = new MVVMFieldContainer();
      var field = new Field<int> { Name = "TestField" };
      var builder = new FieldBuilder<int>(field, container);

      builder
          .WithConstraint(v => v > 0, "Value must be positive")
          .WithConstraint(v => v % 2 == 0, "Value must be even");

      field.Value = -3;

      Assert.True(field.HasErrors);
      Assert.Equal(2, field.Errors.Count());
      Assert.Contains("Value must be positive", field.Errors);
      Assert.Contains("Value must be even", field.Errors);
   }

   [Fact]
   public void WithConstraint_ValidValue_NoErrors() {
      var container = new MVVMFieldContainer();
      var field = new Field<int> { Name = "TestField" };
      var builder = new FieldBuilder<int>(field, container);

      builder
          .WithConstraint(v => v > 0, "Value must be positive")
          .WithConstraint(v => v % 2 == 0, "Value must be even");

      field.Value = 4;

      Assert.False(field.HasErrors);
      Assert.Empty(field.Errors);
   }
}