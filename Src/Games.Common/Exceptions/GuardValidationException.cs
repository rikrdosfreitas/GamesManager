using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Games.Common.Exceptions
{
    public class GuardValidationException : Exception
    {
      public GuardValidationException()
            : base("One or more validation failures have occurred.")
        {
            Errors = new Dictionary<string, string[]>();
        }

        public GuardValidationException(ReadOnlyCollection<GuardValidationResult> failures) : this()
        {
            var failureGroups = failures
                .GroupBy(e => e.Name, e => e.Message);

            foreach (var failureGroup in failureGroups)
            {
                var propertyName = failureGroup.Key;
                var propertyFailures = failureGroup.ToArray();

                Errors.Add(propertyName, propertyFailures);
            }
        }

        public GuardValidationException(string message) : base(message)
        {
        }

        public GuardValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public IDictionary<string, string[]> Errors { get; }
    }
}