/// <summary>
/// SCG Char extension.
/// </summary>
public static class SCGCharExtension
{
	/// <summary>
	/// Determines if the specified mChar is a vowel.
	/// </summary>
	/// <returns><c>true</c> if the specified mChar is a vowel; otherwise, <c>false</c>.</returns>
	/// <param name="mChar">Char.</param>
	public static bool IsVowel (this char mChar)
	{
		return mChar.Equals ('a')
			|| mChar.Equals ('e')
			|| mChar.Equals ('i')
			|| mChar.Equals ('o')
			|| mChar.Equals ('u');
	}
}