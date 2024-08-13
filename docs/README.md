# EasyMVVM

EasyMVVM is a flexible MVVM (Model-View-ViewModel) implementation for .NET applications, providing dynamic field management and data binding capabilities. For larger/more complex user interfaces, I got tired of maintaining a ton of backing fields and public getter/setter pairs to provide binding sites for the view. It dirties up the rest of the ViewModel's functionality, and it requires strict discipline to keep concerns properly separated. EasyMVVM allows you to create one field, and via fluent API, dynamically generate binding targets for all of your WPF controls. 
## Table of Contents

- [Installation](#installation)
- [Basic Usage](#basic-usage)
- [Advanced Features](#advanced-features)
  - [Field Constraints](#field-constraints)
  - [Data Model Binding](#data-model-binding)
  - [Validation](#validation)
- [API Reference](#api-reference)
- [Contributing](#contributing)
- [License](#license)

## Installation

You can install EasyMVVM via NuGet Package Manager:

```
Install-Package EasyMVVM
```

Or via .NET CLI:

```
dotnet add package EasyMVVM
```

## Basic Usage

Here's a simple example of how to use EasyMVVM:

```csharp
using EasyMVVM;

// Create a new field container
var container = new MVVMFieldContainer();

// Add fields with constraints
container.AddField<string>("FirstName")
    .WithConstraint(name => !string.IsNullOrEmpty(name), "Name cannot be empty");

container.AddField<int>("Age")
    .WithConstraint(age => age >= 0, "Age must be non-negative")
    .WithConstraint(age => age < 150, "Age must be less than 150");

// Set values
container.SetValue("FirstName", "John");
container.SetValue("Age", 30);

// Get values
string? name = container.GetValue<string>("FirstName");
int? age = container.GetValue<int>("Age");

Console.WriteLine($"Name: {name}, Age: {age}");

// Check for errors
if (container.HasErrors)
{
    foreach (var fieldName in container.GetFieldNames())
    {
        var errors = container.GetErrors(fieldName);
        foreach (string error in errors)
        {
            Console.WriteLine($"Error in {fieldName}: {error}");
        }
    }
}
```

## Advanced Features

### Field Constraints

You can add multiple constraints to a field:

```csharp
container.AddField<string>("Email")
    .WithConstraint(email => !string.IsNullOrEmpty(email), "Email cannot be empty")
    .WithConstraint(email => email.Contains("@"), "Email must contain @")
    .WithConstraint(email => email.Split('@')[1].Contains("."), "Invalid email domain");
```

### Data Model Binding

EasyMVVM supports binding fields to properties of data models:

```csharp
public class Person : IDataModel
{
    private string _name;
    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

var person = new Person { Name = "Alice" };

container.AddField<string>("PersonName");
container.BindToModel(nameof(Person.Name), person, p => p.Name);

// Updates both the field and the model
container.SetValue("PersonName", "Bob");

Console.WriteLine(person.Name); // Outputs: Bob

// Updates both the model and the field
person.Name = "Charlie";

Console.WriteLine(container.GetValue<string>("PersonName")); // Outputs: Charlie
```

### Validation

You can perform validation on all fields:

```csharp
container.SetValue("Age", 200); // This will trigger a validation error

foreach (var fieldName in container.GetFieldNames())
{
    var errors = container.GetErrors(fieldName);
    foreach (string error in errors)
    {
        Console.WriteLine($"Error in {fieldName}: {error}");
    }
}
```

## API Reference

### MVVMFieldContainer

- `AddField<T>(string name)`: Adds a new field to the container.
- `GetValue<T>(string name)`: Gets the value of a field.
- `SetValue<T>(string name, T value)`: Sets the value of a field.
- `GetFieldNames()`: Gets the names of all fields in the container.
- `HasField(string name)`: Checks if a field with the given name exists.
- `BindToModel<TModel, TProperty>(string fieldName, TModel model, Expression<Func<TModel, TProperty>> propertyExpression)`: Binds a field to a property of a data model.
- `UnbindFromModel(string fieldName)`: Unbinds a field from its data model.
- `GetErrors(string propertyName)`: Gets the validation errors for a specific field.
- `HasErrors`: Indicates whether any field has validation errors.

### FieldBuilder<T>

- `WithConstraint(Func<T, bool> constraint, string errorMessage)`: Adds a constraint to the field.

## Contributing

We welcome contributions to EasyMVVM! Please see our [Contributing Guidelines](CONTRIBUTING.md) for more details on how to submit pull requests, report issues, or request features.

## License

EasyMVVM is licensed under the Mozilla Public License 2.0 (MPL-2.0). See the [LICENSE](LICENSE.md) file for more details.

