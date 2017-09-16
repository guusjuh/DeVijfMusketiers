using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// SCG Int extension.
/// </summary>
public static class SCGIntExtension
{
	/// <summary>
	/// Max the specified mInt.
	/// </summary>
	/// <param name="mInt">Int.</param>
	/// <param name="maximum">Maximum.</param>
	public static int Max (this int mInt, int maximum)
	{
		return mInt = (mInt > maximum ? maximum : mInt);
	}
	
	/// <summary>
	/// Minimum the specified mInt and minimum.
	/// </summary>
	/// <param name="mInt">Int.</param>
	/// <param name="minimum">Minimum.</param>
	public static int Min (this int mInt, int minimum)
	{
		return mInt = (mInt < minimum ? minimum : mInt);
	}

	/// <summary>
	/// Determines if the specified mInt is positive.
	/// </summary>
	/// <returns><c>true</c> if the specified mInt is positive; otherwise, <c>false</c>.</returns>
	/// <param name="mInt">Int.</param>
	public static bool IsPositive(this int mInt)
	{
		return mInt > 0;
	}

	/// <summary>
	/// Determines if the specified mInt is neutral.
	/// </summary>
	/// <returns><c>true</c> if the specified mInt is neutral; otherwise, <c>false</c>.</returns>
	/// <param name="mInt">Int.</param>
	public static bool IsNeutral (this int mInt)
	{
		return mInt.Equals (0);
	}

	/// <summary>
	/// Determines if the specified mInt is singular.
	/// </summary>
	/// <returns><c>true</c> if is singular the specified mInt; otherwise, <c>false</c>.</returns>
	/// <param name="mInt">M int.</param>
	public static bool IsSingular (this int mInt)
	{
		return mInt.Equals (1);
	}

	/// <summary>
	/// Determines if the specified mInt is negative.
	/// </summary>
	/// <returns><c>true</c> if the specified mInt is negative; otherwise, <c>false</c>.</returns>
	/// <param name="mInt">Int.</param>
	public static bool IsNegative (this int mInt)
	{
		return mInt < 0;
	}
}