using System;
using System.Collections.Generic;

/// <summary>
/// SCG List extension.
/// </summary>
public static class SCGListExtension
{	
	/// <summary>
	/// Update the specified list.
	/// </summary>
	/// <param name="list">List.</param>
	/// <param name="value">Value.</param>
	/// <param name="count">Count.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static void Update <T> (this List<T> list, T value, int count)
	{
		// If count does not match current list count
		if (!list.Count.Equals (count))
		{
			// Update value if count is larger than 0
			if (count.IsPositive ())
			{
				// Add value T while list is smaller than count
				while (list.Count < count) list.Add (value);
			
				// Remove last value while list is larger than count
				while (list.Count > count) list.RemoveAt (list.Count - 1);
			}
			// Else clear list
			else list.Clear();
		}
	}

	/// <summary>
	/// Update the specified list.
	/// </summary>
	/// <param name="list">List.</param>
	/// <param name="count">Count.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static void Update <T> (this List<T> list, int count, Action <T> clearAction = null)
	{
		// If count does not match current list count
		if (!list.Count.Equals (count))
		{
			// Update value if count is larger than 0
			if (count.IsPositive ())
			{
				// Add value T while list is smaller than count
				while (list.Count < count) list.Add (typeof (T).IsClass ? (T) System.Activator.CreateInstance (typeof (T)) : default (T));
				
				// Remove last value while list is larger than count
				while (list.Count > count) 
				{
					// If clear action, handle action
					if (clearAction.Exists ()) clearAction (list.Last ());

					// Remove from list
					list.Remove (list.Last ());
				}
			}
			// Else clear list
			else list.Clear (clearAction);
		}
	}

	/// <summary>
	/// Adds the multiple.
	/// </summary>
	/// <param name="list">List.</param>
	/// <param name="values">Values.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static void AddMultiple <T> (this List<T> list, params T[] values)
	{
		list.AddRange (values);
	}

	/// <summary>
	/// Safely add value to list
	/// </summary>
	/// <param name="list">List.</param>
	/// <param name="value">Value.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static void SafelyAdd <T> (this List<T> list, T value)
	{
		if (!list.Contains (value)) list.Add (value);
	}

	/// <summary>
	/// Safely remove value from list
	/// </summary>
	/// <param name="list">List.</param>
	/// <param name="value">Value.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static void SafelyRemove <T> (this List<T> list, T value)
	{
		if (list.Contains (value)) list.Remove (value);
	}

	/// <summary>
	/// Updates all.
	/// </summary>
	/// <param name="list">List.</param>
	/// <param name="value">Value.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static void UpdateAll <T> (this List<T> list, T value)
	{
		for(int i = 0; i < list.Count; ++i)
		{
			list[i] = value;
		}
	}

	/// <summary>
	/// Determines if the specified list is null or empty.
	/// </summary>
	/// <returns><c>true</c> if the specified list is null or empty; otherwise, <c>false</c>.</returns>
	/// <param name="list">List.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static bool IsNullOrEmpty <T> (this List<T> list)
	{
		return list.Exists () ? list.Count.IsNeutral () : true;
	}

	/// <summary>
	/// Determines if the specified list is filled.
	/// </summary>
	/// <returns><c>true</c> if the specified list is filled; otherwise, <c>false</c>.</returns>
	/// <param name="list">List.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static bool IsFilled <T> (this List<T> list)
	{
		return list.Exists () ? list.Count.IsPositive () : false;
	}

	/// <summary>
	/// Determines if has index the specified list index.
	/// </summary>
	/// <returns><c>true</c> if has index the specified list index; otherwise, <c>false</c>.</returns>
	/// <param name="list">List.</param>
	/// <param name="index">Index.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static bool HasIndex <T> (this List<T> list, int index)
	{
		return list.IsFilled () ? !index.IsNegative () && index < list.Count : false;
	}

	/// <summary>
	/// Returns first list entry
	/// </summary>
	/// <param name="list">List.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static T First <T> (this List<T> list)
	{
		return list.IsFilled () ? list [0] : default (T);
	}

	/// <summary>
	/// Returns last list entry
	/// </summary>
	/// <param name="list">List.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static T Last <T> (this List<T> list)
	{
		return list.IsFilled () ? list [list.Count - 1] : default (T);
	}

	/// <summary>
	/// Returns a random entry
	/// </summary>
	/// <returns>The entry.</returns>
	/// <param name="list">List.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static T RandomEntry <T> (this List<T> list)
	{
		return list.IsFilled () ? list [UnityEngine.Random.Range (0, list.Count)] : default (T);
	}

	/// <summary>
	/// Reposition the specified listObject to index.
	/// </summary>
	/// <param name="list">List.</param>
	/// <param name="listObject">List object.</param>
	/// <param name="index">Index.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static void Reposition <T> (this List<T> list, T listObject, int index)
	{
		// If object is not null
		if (listObject.Exists () 
		    // And list contains object
		    && list.Contains (listObject))
		{
			// Remove object
			list.Remove (listObject);
			
			// Add object at index
			list.Insert (index, listObject);
		}
	}

	/// <summary>
	/// Switch the specified firstListObject and secondListObject.
	/// </summary>
	/// <param name="list">List.</param>
	/// <param name="firstListObject">First list object.</param>
	/// <param name="secondListObject">Second list object.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static void Switch <T> (this List<T> list, T firstListObject, T secondListObject)
	{
		// Get indexes
		int firstIndex 	= list.IndexOf(firstListObject);
		int secondIndex = list.IndexOf(secondListObject);

		// Assign objects
		list[firstIndex] 	= secondListObject;
		list[secondIndex]	= firstListObject;
	}

	/// <summary>
	/// Replace the specified oldListObject and newListObject.
	/// </summary>
	/// <param name="list">List.</param>
	/// <param name="oldListObject">Old list object.</param>
	/// <param name="newListObject">New list object.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static void Replace <T> (this List<T> list, T oldListObject, T newListObject)
	{
		if (list.Contains (oldListObject)) list [list.IndexOf (oldListObject)] = newListObject;
	}

	/// <summary>
	/// Handles the action.
	/// </summary>
	/// <param name="list">List.</param>
	/// <param name="filter">Filter.</param>
	/// <param name="action">Action.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static void HandleAction <T> (this List<T> list, Predicate<T> filter, Action<T> action)
	{
		// If list is filled
		if (list.IsFilled ())
		{
			// For each entry
			for (int i = 0; i < list.Count; ++i)
			{
				// If list entry equals filter, handle action
				if (filter (list[i])) action (list [i]);
			}
		}
	}

	/// <summary>
	/// Handles the action.
	/// </summary>
	/// <param name="list">List.</param>
	/// <param name="action">Action.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static void HandleAction <T> (this List<T> list, Action<T> action)
	{
		// If list is filled
		if (list.IsFilled ())
		{
			// For each entry, handle action
			for (int i = 0; i < list.Count; ++i)
			{
				action (list [i]);
			}
		}
	}
	
	/// <summary>
	/// Iterates through list. Handles action reversed.
	/// </summary>
	public static void HandleActionReversed <T> (this List<T> list, Action<T> action)
	{
		// If list is filled
		if (list.IsFilled ())
		{
			// For each entry, handle action
			for (int i = (list.Count - 1); i >= 0; --i)
			{
				action (list [i]);
			}
		}
	}

	/// <summary>
	/// Clear the specified list with filter and action.
	/// </summary>
	/// <param name="list">List.</param>
	/// <param name="filter">Filter.</param>
	/// <param name="action">Action.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static void Clear <T> (this List<T> list, Predicate<T> filter, Action<T> action)
	{
		// For each entry
		for (int i = 0; i < list.Count; ++i)
		{
			// If list entry equals filter
			if (filter (list [i]))
			{
				// Handle action
				action (list [i]);
				
				// Remove from list
				list.RemoveAt (i);
				
				// Subtract i
				--i;
			}
		}
	}

	/// <summary>
	/// Clear the specified list with action.
	/// </summary>
	/// <param name="list">List.</param>
	/// <param name="action">Action.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static void Clear <T> (this List<T> list, Action<T> action)
	{
		// If list is filled
		if (list.IsFilled ())
		{
			// Handle action
			if (action.Exists ()) list.HandleAction (action);

			// Clear list
			list.Clear ();
		}
	}
}