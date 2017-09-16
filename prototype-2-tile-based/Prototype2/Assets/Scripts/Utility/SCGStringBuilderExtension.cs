using System.Collections;
using System.Text;

/// <summary>
/// SCG String builder extension.
/// </summary>
public static class SCGStringBuilderExtension
{
	/// <summary>
	/// Appends the identifier.
	/// </summary>
	/// <param name="builder">Builder.</param>
	/// <param name="id">Identifier.</param>
	/// <param name="seperator">Seperator.</param>
	public static void AppendId (this StringBuilder builder, string id, string seperator = "|")
	{
		// Seperate entry
		if (builder.Length.IsPositive ()) Seperate (builder);

		// Add id
		builder.AppendRange (id, seperator);
	}

	/// <summary>
	/// Appends the range.
	/// </summary>
	/// <param name="builder">Builder.</param>
	/// <param name="value">Value.</param>
	public static void AppendRange(this StringBuilder builder, params string[] values)
	{
		// For each value, append value
		foreach (string value in values)
		{
			builder.Append (value);
		}
	}

	/// <summary>
	/// Seperate the specified builder and seperator.
	/// </summary>
	/// <param name="builder">Builder.</param>
	/// <param name="seperator">Seperator.</param>
	public static void Seperate (this StringBuilder builder, string seperator = "\n")
	{
		builder.Append (seperator);
	}

	/// <summary>
	/// Determines if is null or empty the specified builder.
	/// </summary>
	/// <returns><c>true</c> if is null or empty the specified builder; otherwise, <c>false</c>.</returns>
	/// <param name="builder">Builder.</param>
	public static bool IsNullOrEmpty (this StringBuilder builder)
	{
		return builder.Equals (null) || builder.Length.IsNeutral ();
	}

	/// <summary>
	/// Determines if the specified builder is filled.
	/// </summary>
	/// <returns><c>true</c> if the specified builder is filled; otherwise, <c>false</c>.</returns>
	/// <param name="builder">Builder.</param>
	public static bool IsFilled (this StringBuilder builder)
	{
		return builder.Exists () ? builder.Length.IsPositive () : false;
	}

	/// <summary>
	/// Clear the specified builder.
	/// </summary>
	/// <param name="builder">Builder.</param>
	public static void Clear (this StringBuilder builder)
	{
		builder.Remove (0, builder.Length);
	}
}