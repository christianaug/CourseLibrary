using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseLibrary.API.Services;
using System.Linq.Dynamic.Core;

namespace CourseLibrary.API.Helpers
{
	public static class IQueryableExtensions
	{
		public static IQueryable<T> ApplySort<T>(this IQueryable<T> source, string orderBy, Dictionary<string, PropertyMappingValue> mappingDictionary)
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source))
;			}

			if (mappingDictionary == null)
			{
				throw new ArgumentNullException(nameof(mappingDictionary));
			}

			if (string.IsNullOrEmpty(orderBy))
			{
				return source;
			}

			var orderByString = string.Empty;

			//orderBy string is seperate by ","
			var orderBySplit = orderBy.Split(",");

			//apply each orderbyclause in reverse order - otherwise
			//IQueryable will be ordered int he wrong order
			foreach (var orderByClause in orderBySplit.Reverse())
			{
				//trim the orde rby clauses to remove leading or trailing white space
				var trimmedOrderByClause = orderByClause.Trim();

				//if sort options ends with the "desc", we order in descending, ortherwise ascending
				var orderDescending = trimmedOrderByClause.EndsWith(" desc");

				//remove asc or desc
				var indexOfFirstSpace = trimmedOrderByClause.IndexOf(" ");
				var propertyName = indexOfFirstSpace == -1 ? trimmedOrderByClause : trimmedOrderByClause.Remove(indexOfFirstSpace);
			
				//find the matching property
				if (!mappingDictionary.ContainsKey(propertyName))
				{
					throw new ArgumentException($"Key mapping for {propertyName} is missing");
				}

				//get the PropertyMatchingValue
				var propertyMappingValue = mappingDictionary[propertyName];

				if (propertyMappingValue == null)
				{
					throw new ArgumentNullException("propertyMappingValue");
				}

				//run through the property name
				//so the order by clauses are applied in tyhe correct order
				foreach (var destinationProperty in propertyMappingValue.DestinationProperties)
				{
					//revert the sort order if necessary
					if (propertyMappingValue.Revert)
					{
						orderDescending = !orderDescending;
					}

					orderByString = orderByString +
						(string.IsNullOrWhiteSpace(orderByString) ? string.Empty : ", ")
						+ destinationProperty
						+ (orderDescending ? " descending" : " ascending");
				}
			}

			return source.OrderBy(orderByString);
		}

	}
}
