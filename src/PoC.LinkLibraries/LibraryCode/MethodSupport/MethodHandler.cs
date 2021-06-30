using System.Collections.Generic;
using System.Linq;
using Nexus.Link.Libraries.Core.Assert;
using Nexus.Link.Libraries.Core.Misc;

namespace PoC.LinkLibraries.LibraryCode.MethodSupport
{
    public class MethodHandler
    {

        private Dictionary<int, MethodParameter> _parameters = new Dictionary<int, MethodParameter>();

        private readonly Dictionary<string, MethodArgument> _arguments = new Dictionary<string, MethodArgument>();
        private readonly string _definitionName;
        private string? _instanceName;

        public MethodHandler(string definitionName)
        {
            _definitionName = definitionName;
        }

        public MethodHandler NewInstance(string instanceName, object[] arguments)
        {
            var instance = new MethodHandler(_definitionName)
            {
                _parameters = _parameters,
                _instanceName = instanceName
            };
            instance.SetArguments(arguments);
            return instance;
        }

        private void AddParameter(string name, System.Type type)
        {
            var position = _parameters.Count + 1;
            _parameters.Add(position, new MethodParameter(name, type));
        }
        public void AddParameter<TParameter>(string name)
        {
            AddParameter(name, typeof(TParameter));
        }

        public string GetParameterName(int position)
        {
            return _parameters[position].Name;
        }

        public object GetArgument(string parameterName)
        {
            if (!_arguments.TryGetValue(parameterName, out var argument))
            {
                var argumentParameters = string.Join(", ", _arguments.Values.Select(a => a.Parameter.Name));
                InternalContract.Fail($"{_definitionName} has a parameter named {parameterName}, but {_instanceName} had no argument for that parameter. Found these: {argumentParameters}");
                return default;
            }

            FulcrumAssert.IsTrue(argument.Parameter.Type.IsInstanceOfType(argument.Value), CodeLocation.AsString());

            return argument.Value;
        }

        public TArgument GetArgument<TArgument>(string parameterName)
        {
            return (TArgument)GetArgument(parameterName);
        }

        private void SetArguments(object[] values)
        {
            for (var i = 0; i < values.Length; i++)
            {
                var position = i + 1;
                var parameter = _parameters[position];
                SetArgument(parameter.Name, values[i]);
            }
        }

        private MethodParameter GetMethodParameter(string parameterName)
        {
            var parameter = _parameters.Values.FirstOrDefault(p => p.Name == parameterName);
            InternalContract.RequireNotNull(parameter, nameof(parameterName),
                $"No parameter named {parameterName}. These are registered: {string.Join(", ", _parameters.Keys)}");
            return parameter;
        }

        private void SetArgument(string parameterName, object value)
        {
            var parameter = GetMethodParameter(parameterName);
            if (value == null)
            {
                InternalContract.Require(parameter!.IsNullable,
                    $"The parameter {parameter} does not accept the value null.");
            }
            else
            {
                InternalContract.Require(parameter!.Type.IsInstanceOfType(value),
                    $"Expected {nameof(value)} to be an instance of type {parameter.Type.FullName}, but was of type {value.GetType().FullName}.");
            }

            var argument = new MethodArgument(parameter, value);
            _arguments.Add(parameter.Name, argument);
        }
    }
}