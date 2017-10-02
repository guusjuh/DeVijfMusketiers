using System.Collections.Generic;

/// <summary>
/// SCG Dictionary extension.
/// </summary>
public static class SCGDictionaryExtension
{
	/// <summary>
	/// Sets the specified value and key
	/// </summary>
	/// <param name="dictionary">Dictionary.</param>
	/// <param name="key">Key.</param>
	/// <param name="value">Value.</param>
	/// <typeparam name="TKey">The 1st type parameter.</typeparam>
	/// <typeparam name="TValue">The 2nd type parameter.</typeparam>
	public static void Set <TKey, TValue> (this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
	{
		// If dictionary contains key, set value
		if (dictionary.ContainsKey (key)) dictionary [key] = value;
		// Else add data
		else dictionary.Add (key, value);
	}

	/// <summary>
	/// Gets the value from the specified key
	/// </summary>
	/// <param name="dictionary">Dictionary.</param>
	/// <param name="key">Key.</param>
	/// <param name="defaultValue">Default value.</param>
	/// <typeparam name="TKey">The 1st type parameter.</typeparam>
	/// <typeparam name="TValue">The 2nd type parameter.</typeparam>
	public static TValue Get <TKey, TValue> (this Dictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default (TValue))
	{
		return dictionary.Exists () && dictionary.ContainsKey (key) ? dictionary [key] : defaultValue; 	
	}

	/// <summary>
	/// Determines if the specified dictionary is filled.
	/// </summary>
	/// <returns><c>true</c> if the specified dictionary is filled; otherwise, <c>false</c>.</returns>
	/// <param name="dictionary">Dictionary.</param>
	/// <typeparam name="TKey">The 1st type parameter.</typeparam>
	/// <typeparam name="TValue">The 2nd type parameter.</typeparam>
	public static bool IsFilled <TKey, TValue> (this Dictionary<TKey, TValue> dictionary)
	{
		return dictionary.Exists () ? dictionary.Count.IsPositive () : false;
	}
	
	/// <summary>
	/// Determines if the specified dictionary is empty.
	/// </summary>
	/// <returns><c>true</c> if the specified dictionary is empty; otherwise, <c>false</c>.</returns>
	/// <param name="dictionary">Dictionary.</param>
	/// <typeparam name="TKey">The 1st type parameter.</typeparam>
	/// <typeparam name="TValue">The 2nd type parameter.</typeparam>
	public static bool IsNullOrEmpty <TKey, TValue> (this Dictionary<TKey, TValue> dictionary)
	{
		return dictionary.Exists () ? dictionary.Count.IsNeutral () : true;
	}
}