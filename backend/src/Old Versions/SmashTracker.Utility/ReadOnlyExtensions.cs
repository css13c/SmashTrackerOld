using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SmashTracker.Utility
{
    public static class ReadOnlyExtensions
	{
		public static ReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> source)
		{
			return source.ToList().AsReadOnly();
		}

		public static ReadOnlyDictionary<TKey, TValue> ToReadOnlyDictionary<TKey, TValue>(this IDictionary<TKey, TValue> source)
		{
			return new ReadOnlyDictionary<TKey, TValue>(source);
		}

		public static ReadOnlyDictionary<TKey, TValue> ToReadOnlyDictionary<TKey, TValue>(this IEnumerable<TValue> source, Func<TValue, TKey> keySelector)
		{
			return source.ToDictionary(keySelector).ToReadOnlyDictionary();
		}
	}
}
