namespace EasyMVVM;
using System.Collections.Generic;

public interface IField {
   string Name { get; }
   bool HasErrors { get; }
   IEnumerable<string> Errors { get; }
   void Validate();
}

