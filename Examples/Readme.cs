namespace EasyMVVM.Examples;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

class Person : IDataModel {
   private string _firstName;
   private string _lastName;
   private int _age;

   public string FirstName {
      get => _firstName;
      set => SetProperty(ref _firstName, value);
   }

   public string LastName {
      get => _lastName;
      set => SetProperty(ref _lastName, value);
   }

   public int Age {
      get => _age;
      set => SetProperty(ref _age, value);
   }

   public event PropertyChangedEventHandler PropertyChanged;

   protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
   }

   protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null) {
      if( EqualityComparer<T>.Default.Equals(field, value) ) return false;
      field = value;
      OnPropertyChanged(propertyName);
      return true;
   }
}

class Program {
   static void Main() {
      var container = new MVVMFieldContainer();
      var person = new Person { FirstName = "John", LastName = "Doe", Age = 30 };

      container.AddField<string>("FirstName")
          .WithConstraint(name => !string.IsNullOrEmpty(name), "First name cannot be empty");
      container.AddField<string>("LastName")
          .WithConstraint(name => !string.IsNullOrEmpty(name), "Last name cannot be empty");
      container.AddField<int>("Age")
          .WithConstraint(age => age >= 0, "Age must be non-negative")
          .WithConstraint(age => age < 150, "Age must be less than 150");

      container.BindToModel(nameof(Person.FirstName), person, p => p.FirstName);
      container.BindToModel(nameof(Person.LastName), person, p => p.LastName);
      container.BindToModel(nameof(Person.Age), person, p => p.Age);

      // Print initial values
      PrintValues(container, person);

      // Update through container
      container.SetValue("FirstName", "Jane");
      container.SetValue("Age", 31);

      Console.WriteLine("\nAfter updating through container:");
      PrintValues(container, person);

      // Update through model
      person.LastName = "Smith";

      Console.WriteLine("\nAfter updating through model:");
      PrintValues(container, person);

      // Demonstrate validation
      container.SetValue("Age", 200); // Should fail
      foreach( var error in container.GetErrors("Age") ) {
         Console.WriteLine($"Validation error: {error}");
      }

      // Unbind and update
      container.UnbindFromModel("FirstName");
      person.FirstName = "Alice";
      container.SetValue("FirstName", "Bob");

      Console.WriteLine("\nAfter unbinding FirstName and updating separately:");
      PrintValues(container, person);
   }

   static void PrintValues(IMVVMFieldContainer container, Person person) {
      Console.WriteLine($"Container - FirstName: {container.GetValue<string>("FirstName")}, LastName: {container.GetValue<string>("LastName")}, Age: {container.GetValue<int>("Age")}");
      Console.WriteLine($"Model - FirstName: {person.FirstName}, LastName: {person.LastName}, Age: {person.Age}");
   }
}
