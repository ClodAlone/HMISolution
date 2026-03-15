using System;
using System.Collections.Generic;
using System.Text;

namespace ViewModelLib
{
    /// <summary>
    /// This class provides the mechanisms for performing adding and performing the 
    /// actual validation. Basically, the derived classes add the rules as Actions,
    /// which are then called in the validation.
    /// </summary>
    /// <typeparam name="T">The derived class type.</typeparam>
    public class ModelBase<T>
    {
        private Dictionary<string, List<Action<T>>> _validation;
        private List<string> _failures = new List<string>();

        /// <summary>
        /// Add in the validation to be performed.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="validation">The action to be performed.</param>
        protected void AddValidation(string key, Action<T> validation)
        {
            if (_validation == null)
            {
                _validation = new Dictionary<string, List<Action<T>>>();
            }
            if (!_validation.ContainsKey(key))
            {
                List<Action<T>> value = new List<Action<T>>();
                _validation.Add(key, value);
            }
            _validation[key].Add(validation);
        }

        /// <summary>
        /// The mechanism to actually perform the validation.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="instance">The instance to validate.</param>
        /// <returns>A textual definition of any validation failures.</returns>
        protected string PerformValidation(string key, T instance)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key");
            _failures.Clear();
            if (_validation.ContainsKey(key))
            {
                foreach (Action<T> action in _validation[key])
                {
                    action(instance);
                }
            }

            StringBuilder failures = new StringBuilder();
            foreach (string failure in _failures)
            {
                failures.AppendFormat("{0}{1}", failure, Environment.NewLine);
            }
            return failures.ToString();
        }

        /// <summary>
        /// Add the failure message in.
        /// </summary>
        /// <param name="message">The message to add in to the failures.</param>
        protected void AddFailure(string message)
        {
            _failures.Add(message);
        }
    }
}
