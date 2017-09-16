/// <summary>
/// SCG Array extension.
/// </summary>
public static class SCGArrayExtension
{
	/// <summary>
	/// Create new T[] array and adds T value
	/// </summary>
	/// <param name="array">T[] array</param>
	/// <param name="value">T value</param>
	/// <typeparam name="T">The element type of the array</typeparam>
	public static T[] Add <T> (this T[] array, T value)
	{
		// Create new array
		T[] nArray = new T [array.Length + 1];

		// Fill array with values
		for (int i = 0; i < array.Length; ++i) nArray [i] = array [i];

		// Set last object
		nArray [array.Length] = value;

		// Set array
		return nArray;
	}
	
	/// <summary>
	/// Create new T[] array and copies original T[] array partially
	/// </summary>
	/// <param name="array">T[] array</param>
	/// <param name="value">T value</param>
	/// <typeparam name="T">The element type of the array</typeparam>
	public static void Remove <T> (this T[] array, T value)
	{
		// For each value
		for (int i = 0; i < array.Length; ++i)
		{
			// If array value equals value
			if (array [i].Equals (value))
			{
				// Remove at index
				array.RemoveAt (i);

				// Stop for loop
				break;
			}
		}
	}
	
	/// <summary>
	/// Create new T[] array and copies original T[] array partially
	/// </summary>
	/// <param name="array">T[] array</param>
	/// <param name="value">T value</param>
	/// <typeparam name="T">The element type of the array</typeparam>
	public static T[] RemoveAt <T> (this T[] array, int index)
	{
		// If index exists
		if (array.Length > index)
		{
			// If last object, set null
			if (array.Length.Equals (1)) return null;
			// Else update path
			else
			{
				// Create new array
				T[] nArray = new T [array.Length - 1];
				
				// Fill array with values
				for (int i = 0; i < array.Length; ++i)
				{
					// If i is not index, update array
					if (!i.Equals (index)) nArray [i > index ? i - 1 : i] = array [i];
				}
				
				// Set array
				return nArray;
			}
		}

		// Return arrya
		return array;
	}
	
	/// <summary>
	/// Fill T[] array with value
	/// </summary>
	/// <param name="array">T[] array</param>
	/// <param name="value">T value</param>
	/// <typeparam name="T">The element type of the array</typeparam>
	public static void Fill <T> (this T[] array, T value)
	{
		// For each entry, set value
		for (int i = 0; i < array.Length; i++) array [i] = value;
	}
	
	/// <summary>
	/// Fill T[,] array with value
	/// </summary>
	/// <param name="array">T[,] array</param>
	/// <param name="value">T value</param>
	/// <typeparam name="T">The element type of the array</typeparam>
	public static void Fill <T> (this T[,] array, T value)
	{
		// For each row
		for (int i = array.GetLowerBound (0); i <= array.GetUpperBound (0); ++i)
        {
			// For each column, set value
            for (int j = array.GetLowerBound (1); j <= array.GetUpperBound (1); ++j)
            {
                array [i, j] = value;
            }
        }
	}
	
	/// <summary>
	/// Is array empty or null
	/// </summary>
	/// <param name="array">T[] array</param>
	/// <typeparam name="T">The element type of the array</typeparam>
	public static bool IsNullOrEmpty <T> (this T[] array)
	{
		return array.Exists () ? array.Length.IsNeutral () : true;
	}
	
	/// <summary>
	/// Is array filled
	/// </summary>
	/// <param name="array">T[] array</param>
	/// <typeparam name="T">The element type of the array</typeparam>
	public static bool IsFilled <T> (this T[] array)
	{
		return array.Exists () ? array.Length.IsPositive () : false;
	}
	
	/// <summary>
	/// Returns first array entry
	/// </summary>
	/// <param name="array">T[] array</param>
	/// <typeparam name="T">The element type of the array</typeparam>
	public static T First <T> (this T[] array)
	{
		return array.IsFilled () ? array [0] : default (T);
	}
	
	/// <summary>
	/// Returns last array entry
	/// </summary>
	/// <param name="array">T[] array</param>
	/// <typeparam name="T">The element type of the array</typeparam>
	public static T Last <T> (this T[] array)
	{
		return array.IsFilled () ? array [array.Length - 1] : default (T);
	}
	
	/// <summary>
	/// Returns random array entry
	/// </summary>
	/// <param name="array">T[] array</param>
	/// <typeparam name="T">The element type of the array</typeparam>
	public static T RandomEntry <T> (this T[] array)
	{
		return array.IsFilled () ? array [UnityEngine.Random.Range (0, array.Length)] : default (T);
	}
}