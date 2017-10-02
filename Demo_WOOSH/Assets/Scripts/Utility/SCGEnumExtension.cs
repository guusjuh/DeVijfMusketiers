using System;

/// <summary>
/// SCG Enum extension.
/// </summary>
public static class SCGEnumExtension
{
	/// <summary>
	/// Determines if the specified mChar is a vowel.
	/// </summary>
	/// <returns><c>true</c> if the specified mChar is a vowel; otherwise, <c>false</c>.</returns>
	/// <param name="mChar">Char.</param>
	public static bool HasFlag (this Enum mEnum, Enum value)
	{
		// Convert value
		long nValue = Convert.ToInt64 (value);

		// Return has flag
		return (Convert.ToInt64 (mEnum) & nValue).Equals (nValue);
	}
}