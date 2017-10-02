using System.Collections;

/// <summary>
/// SCG Array list extension.
/// </summary>
public static class SCGArrayListExtension
{
	/// <summary>
	/// Set the specified value at index of ArrayList.
	/// </summary>
	/// <param name="arrayList">ArrayList</param>
	/// <param name="index">ArrayList index</param>
	/// <param name="value">ArrayList value</param>
	public static void Set (this ArrayList arrayList, int index, object value)
	{
		// If index exists, set value
		if (index < arrayList.Count) arrayList [index] = value;
		// Add value
		else arrayList.Add (value);
	}

	/// <summary>
	/// Gets the array list.
	/// </summary>
	/// <returns>The array list.</returns>
	/// <param name="arrayList">Array list.</param>
	/// <param name="index">Index.</param>
	public static ArrayList GetArrayList (this ArrayList arrayList, int index)
	{
		return arrayList.Count > index ? arrayList [index].ToArrayList () : new ArrayList ();
	}

	/// <summary>
	/// Determines if the specified arrayList is filled.
	/// </summary>
	/// <returns><c>true</c> if the specified arrayList is filled; otherwise, <c>false</c>.</returns>
	/// <param name="arrayList">Array list.</param>
	public static bool IsFilled (this ArrayList arrayList)
	{
		return arrayList.Exists () ? arrayList.Count.IsPositive () : false;
	}

	/// <summary>
	/// Determines if the specified arrayList is null or empty.
	/// </summary>
	/// <returns><c>true</c> if the specified arrayList is null or empty; otherwise, <c>false</c>.</returns>
	/// <param name="arrayList">Array list.</param>
	public static bool IsNullOrEmpty (this ArrayList arrayList)
	{
		return arrayList.Exists () ? arrayList.Count.IsNeutral () : true;
	}

	/// <summary>
	/// Convert arrayList index to hashtable.
	/// </summary>
	/// <returns>The to hashtable.</returns>
	/// <param name="arrayList">Array list.</param>
	/// <param name="index">Index.</param>
	public static Hashtable ToHashtable (this ArrayList arrayList, int index)
	{
		return arrayList.Count > index ? arrayList [index].ToHashtable () : new Hashtable ();
	}
}