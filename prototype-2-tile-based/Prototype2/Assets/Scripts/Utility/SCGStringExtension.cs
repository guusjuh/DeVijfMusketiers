using System.Text;
using System.ComponentModel;

/// <summary>
/// SCG String extension.
/// </summary>
public static class SCGStringExtension
{	
	/// <summary>
	/// Determines if is null or empty the specified mString.
	/// </summary>
	/// <returns><c>true</c> if is null or empty the specified mString; otherwise, <c>false</c>.</returns>
	/// <param name="mString">M string.</param>
	public static bool IsNullOrEmpty (this string mString)
	{
		return mString == null || mString.Equals (null) || string.IsNullOrEmpty (mString);
	}

	/// <summary>
	/// Exists the specified mString.
	/// </summary>
	/// <param name="mString">M string.</param>
	public static bool Exists (this string mString)
	{
		return !mString.IsNullOrEmpty ();
	}
}