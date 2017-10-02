using System.Collections;

/// <summary>
/// SCG Hashtable extension.
/// </summary>
public static class SCGHashtableExtension
{
	/// <summary>
	/// Sets the specified value and key
	/// </summary>
	/// <param name="hashtable">Hashtable.</param>
	/// <param name="key">Key.</param>
	/// <param name="value">Value.</param>
	public static void Set (this Hashtable hashtable, object key, object value)
	{
		// Set value
		if (hashtable.ContainsKey (key)) hashtable [key] = value;
		// Add pair
		else hashtable.Add (key, value);
	}

	/// <summary>
	/// Sets the specified value and key, if settable
	/// </summary>
	/// <param name="hashtable">Hashtable.</param>
	/// <param name="key">Key.</param>
	/// <param name="value">Value.</param>
	/// <param name="canSet">If set to <c>true</c> can set.</param>
	public static void SetIf (this Hashtable hashtable, object key, object value, bool canSet)
	{
		// If able to set data
		if (canSet) Set (hashtable, key, value);
		// Else remove from data
		else hashtable.SafelyRemove (key.ToString ());
	}

	/// <summary>
	/// Gets the value from key.
	/// </summary>
	/// <param name="hashtable">Hashtable.</param>
	/// <param name="key">Key.</param>
	public static string Get (this Hashtable hashtable, string key)
	{
		return hashtable.ContainsKey (key) ? hashtable [key].ToString() : string.Empty;
	}

	/// <summary>
	/// Gets the hashtable from key.
	/// </summary>
	/// <returns>The hashtable.</returns>
	/// <param name="hashtable">Hashtable.</param>
	/// <param name="key">Key.</param>
	public static Hashtable GetHashtable (this Hashtable hashtable, string key)
	{
		return hashtable.ContainsKey (key) ? hashtable [key].ToHashtable () : new Hashtable ();
	}

	/// <summary>
	/// Gets the ArrayList from key
	/// </summary>
	/// <returns>The array list.</returns>
	/// <param name="hashtable">Hashtable.</param>
	/// <param name="key">Key.</param>
	public static ArrayList GetArrayList (this Hashtable hashtable, string key)
	{
		return hashtable.ContainsKey (key) ? hashtable [key].ToArrayList () : new ArrayList ();
	}

	/// <summary>
	/// Safely removes the value from key
	/// </summary>
	/// <param name="hashtable">Hashtable.</param>
	/// <param name="key">Key.</param>
	public static void SafelyRemove(this Hashtable hashtable, string key)
	{
		// Set value
		if (hashtable.ContainsKey (key)) hashtable.Remove (key);
	}

	/// <summary>
	/// Determines if the specified hashtable is filled.
	/// </summary>
	/// <returns><c>true</c> if the specified hashtable is filled; otherwise, <c>false</c>.</returns>
	/// <param name="hashtable">Hashtable.</param>
	public static bool IsFilled (this Hashtable hashtable)
	{
		return hashtable.Exists () ? hashtable.Count.IsPositive () : false;
	}

	/// <summary>
	/// Determines if the specified hashtable is empty.
	/// </summary>
	/// <returns><c>true</c> if the specified hashtable is empty; otherwise, <c>false</c>.</returns>
	/// <param name="hashtable">Hashtable.</param>
	public static bool IsNullOrEmpty (this Hashtable hashtable)
	{
		return hashtable.Exists () ? hashtable.Count.IsNeutral () : true;
	}
}