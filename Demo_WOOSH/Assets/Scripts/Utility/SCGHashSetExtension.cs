using System.Collections.Generic;

/// <summary>
/// SCG Hash set extension.
/// </summary>
public static class SCGHashSetExtension
{	
	/// <summary>
	/// Safely add the value.
	/// </summary>
	/// <param name="hashSet">Hash set.</param>
	/// <param name="value">Value.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static void SafelyAdd <T> (this HashSet<T> hashSet, T value)
	{
		if (!hashSet.Contains (value)) hashSet.Add (value);
	}

	/// <summary>
	/// Safely removes the value
	/// </summary>
	/// <param name="hashSet">Hash set.</param>
	/// <param name="value">Value.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static void SafelyRemove <T> (this HashSet<T> hashSet, T value)
	{
		if (hashSet.Contains (value)) hashSet.Remove (value);
	}

	/// <summary>
	/// Determines if the specified hashSet is empty.
	/// </summary>
	/// <returns><c>true</c> if the specified hashSet is empty; otherwise, <c>false</c>.</returns>
	/// <param name="hashSet">Hash set.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static bool IsNullOrEmpty <T> (this HashSet<T> hashSet)
	{
		return hashSet.Exists () ? hashSet.Count.IsNeutral () : true;
	}

	/// <summary>
	/// Determines if the specified hashSet is filled.
	/// </summary>
	/// <returns><c>true</c> if the specified hashSet is filled; otherwise, <c>false</c>.</returns>
	/// <param name="hashSet">Hash set.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static bool IsFilled<T>(this HashSet<T> hashSet)
	{
		return hashSet.Exists () ? hashSet.Count.IsPositive () : false;
	}
}