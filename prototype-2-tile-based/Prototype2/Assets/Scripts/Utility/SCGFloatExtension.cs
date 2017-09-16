/// <summary>
/// SCG Float extension.
/// </summary>
public static class SCGFloatExtension
{
	/// <summary>
	/// Max the specified mFloat.
	/// </summary>
	/// <param name="mFloat">Float.</param>
	/// <param name="maximum">Maximum.</param>
	public static float Max (this float mFloat, float maximum)
	{
		return mFloat = (mFloat > maximum ? maximum : mFloat);
	}

	/// <summary>
	/// Minimum the specified mFloat.
	/// </summary>
	/// <param name="mFloat">Float.</param>
	/// <param name="minimum">Minimum.</param>
	public static float Min (this float mFloat, float minimum)
	{
		return mFloat = (mFloat < minimum ? minimum : mFloat);
	}

	/// <summary>
	/// Determines if the specified <see cref="float"/> is positive.
	/// </summary>
	/// <returns><c>true</c> if the specified float is positive; otherwise, <c>false</c>.</returns>
	/// <param name="mFloat">Float.</param>
	public static bool IsPositive (this float mFloat)
	{
		return mFloat > 0f;
	}
	
	/// <summary>
	/// Determines if the specified <see cref="float"/> is neutral.
	/// </summary>
	/// <returns><c>true</c> if the specified float is neutral; otherwise, <c>false</c>.</returns>
	/// <param name="mFloat">Float.</param>
	public static bool IsNeutral (this float mFloat)
	{
		return mFloat.Equals (0f);
	}
	
	/// <summary>
	/// Determines if the specified <see cref="float"/> is negative.
	/// </summary>
	/// <returns><c>true</c> if the specified float is negative; otherwise, <c>false</c>.</returns>
	/// <param name="mFloat">Float.</param>
	public static bool IsNegative (this float mFloat)
	{
		return mFloat < 0f;
	}
}