using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

/// <summary>
/// SCG Object extension.
/// </summary>
public static class SCGObjectExtension
{
	/// <summary>
	/// Parse the specified object to value T.
	/// </summary>
	/// <param name="mObject">object.</param>
	/// <param name="original">original value.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static T Parse<T>(this object mObject, T originalValue)
	{
		// Return if empty
		if (mObject.IsNullOrEmpty () || string.IsNullOrEmpty (mObject.ToString ())) return originalValue;
		
		// Create converter
		TypeConverter converter = TypeDescriptor.GetConverter (typeof (T));
		
		// Return result
		return converter.Exists () && converter.IsValid (mObject) ? (T) converter.ConvertFrom (mObject) : originalValue;
	}
	
	/// <summary>
	/// Determines if is null or empty the specified mObject.
	/// </summary>
	/// <returns><c>true</c> if is null or empty the specified mObject; otherwise, <c>false</c>.</returns>
	/// <param name="mObject">Object.</param>
	public static bool IsNullOrEmpty (this object mObject)
	{
		return mObject == null || mObject.Equals (null);
	}

	/// <summary>
	/// The specified mObject exists.
	/// </summary>
	/// <param name="mObject">object.</param>
	public static bool Exists (this object mObject)
	{
		return !mObject.IsNullOrEmpty ();
	}
	
	/// <summary>
	/// Convert object to hashtable.
	/// </summary>
	/// <returns>The hashtable.</returns>
	/// <param name="mObject">Object.</param>
	public static Hashtable ToHashtable (this object mObject)
	{
		return mObject.IsNullOrEmpty () ? new Hashtable () : new Hashtable (mObject as Dictionary<string, object>);
	}
	
	/// <summary>
	/// Convert object to array list.
	/// </summary>
	/// <returns>The array list.</returns>
	/// <param name="_object">Object.</param>
	public static ArrayList ToArrayList (this object mObject)
	{
		return mObject.IsNullOrEmpty () ? new ArrayList () : new ArrayList (mObject as ICollection);
	}
}
