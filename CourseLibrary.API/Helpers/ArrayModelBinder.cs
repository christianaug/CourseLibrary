using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CourseLibrary.API.Helpers
{
	public class ArrayModelBinder : IModelBinder
	{
		public Task BindModelAsync(ModelBindingContext bindingContext)
		{
			//make sure the binder is working on an enumerable type
			if (!bindingContext.ModelMetadata.IsEnumerableType)
			{
				bindingContext.Result = ModelBindingResult.Failed();
				return Task.CompletedTask;
			}

			//get the value through the value provider
			var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).ToString();

			//if the value is null or is white space, return null
			if(string.IsNullOrEmpty(value))
			{
				bindingContext.Result = ModelBindingResult.Success(null);
				return Task.CompletedTask;
			}

			//if the conditions above are satisfied, get the enumerables type and a converter
			var elementType = bindingContext.ModelType.GetTypeInfo().GenericTypeArguments[0];
			var converter = TypeDescriptor.GetConverter(elementType);

			//convert each item in te value list to the enumerable type
			var values = value.Split(new[] { "," }, System.StringSplitOptions.RemoveEmptyEntries)
				.Select(x => converter.ConvertFromString(x.Trim()))
				.ToArray();

			//create an array of that type and set it as the model value
			var typedValues = Array.CreateInstance(elementType, values.Length);
			values.CopyTo(typedValues, 0);
			bindingContext.Model = typedValues;

			//return a successful result, passing in the model
			bindingContext.Result = ModelBindingResult.Success(bindingContext.Model);
			return Task.CompletedTask;
		}
	}
}