namespace EasyMVVM.Tests;

using EasyMVVM;

public class Test_Field {
   [Fact]
   public void SetValue_GetValue_ValueMatchesSet() {
      var field = new Field<int> { Name = "TestField" };
      field.Value = 42;

      Assert.Equal(42, field.Value);
   }

   [Fact]
   public void SetValue_PropertyChangedEventRaised() {
      var field = new Field<string> { Name = "TestField" };
      var eventRaised = false;
      field.PropertyChanged += (sender, args) => eventRaised = true;

      field.Value = "New Value";

      Assert.True(eventRaised);
   }

   [Fact]
   public void AddConstraint_ValidValue_NoErrors() {
      var field = new Field<int> { Name = "TestField" };
      field.AddConstraint(v => v > 0, "Value must be positive");

      field.Value = 5;
      field.Validate();

      Assert.False(field.HasErrors);
      Assert.Empty(field.Errors);
   }

   [Fact]
   public void AddConstraint_InvalidValue_HasErrors() {
      var field = new Field<int> { Name = "TestField" };
      field.AddConstraint(v => v > 0, "Value must be positive");

      field.Value = -5;
      field.Validate();

      Assert.True(field.HasErrors);
      Assert.Single(field.Errors);
      Assert.Contains("Value must be positive", field.Errors);
   }

   [Fact]
   public void MultipleConstraints_SomeInvalid_MultipleErrors() {
      var field = new Field<int> { Name = "TestField" };
      field.AddConstraint(v => v > 0, "Value must be positive");
      field.AddConstraint(v => v % 2 == 0, "Value must be even");

      field.Value = -3;
      field.Validate();

      Assert.True(field.HasErrors);
      Assert.Equal(2, field.Errors.Count());
      Assert.Contains("Value must be positive", field.Errors);
      Assert.Contains("Value must be even", field.Errors);
   }

   [Fact]
   public void SetValue_AutomaticallyValidates() {
      var field = new Field<int> { Name = "TestField" };
      field.AddConstraint(v => v > 0, "Value must be positive");

      field.Value = -5;

      Assert.True(field.HasErrors);
      Assert.Single(field.Errors);
   }
}