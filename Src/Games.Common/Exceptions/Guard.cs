using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Games.Common.Exceptions
{
    public class Guard
    {
        private readonly List<GuardValidationResult> _validationResults = new List<GuardValidationResult>();

        private Guard() { }

        public static void Validate(Action<Guard> action)
        {
            var guard = new Guard();
            action.Invoke(guard);

            if (guard._validationResults.Any())
            {
                throw new GuardValidationException(guard._validationResults.AsReadOnly());
            }
        }

        public static IEnumerable<GuardValidationResult> IsValidFor(Action<Guard> action)
        {
            var guard = new Guard();
            action.Invoke(guard);

            return guard._validationResults;
        }

        public Guard NotNull(object obj, string name, string message)
        {
            if (obj == null)
            {
                _validationResults.Add(new GuardValidationResult(name, message));
            }

            return this;
        }

        public Guard NotNullOrEmptyString(string obj, string name, string message)
        {
            if (string.IsNullOrWhiteSpace(obj))
            {
                _validationResults.Add(new GuardValidationResult(name, message));
            }

            return this;
        }

        public Guard NotEmptyGuid(Guid obj, string name, string message)
        {
            if (obj == default)
            {
                _validationResults.Add(new GuardValidationResult(name, message));
            }

            return this;
        }

        public Guard Min(int obj, int min, string name, string message)
        {
            if (obj < min)
            {
                _validationResults.Add(new GuardValidationResult(name, message));
            }

            return this;
        }

        public Guard IsAct(in bool? obj, string name, string message)
        {
            if (obj.HasValue && !obj.Value)
            {
                _validationResults.Add(new GuardValidationResult(name, message));
            }

            return this;
        }

        public Guard IsEmail(string email, string name, string message)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return this;
            }

            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(email);

            if (!match.Success)
            {
                _validationResults.Add(new GuardValidationResult(name, message));
            }

            return this;
        }


        public Guard NotEmptyList<T>(List<T> obj, string name, string message)
        {
            if (obj == default || !obj.Any())
            {
                _validationResults.Add(new GuardValidationResult(name, message));
            }

            return this;
        }
    }
}
