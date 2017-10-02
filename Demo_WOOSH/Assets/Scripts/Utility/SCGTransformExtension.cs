using UnityEngine;

/// <summary>
/// SCG Transform extension.
/// </summary>
public static class SCGTransformExtension
{
	/// <summary>
	/// Reset the specified transform.
	/// </summary>
	/// <param name="transform">Transform.</param>
	public static void Reset (this Transform transform)
	{
		transform.position 		= Vector3.zero;
		transform.eulerAngles 	= Vector3.zero;
		transform.localScale 	= Vector3.one;
	}

	/// <summary>
	/// Resets the specified local transform.
	/// </summary>
	/// <param name="transform">Transform.</param>
	public static void ResetLocal(this Transform transform)
	{
		transform.localPosition 	= Vector3.zero;
		transform.localEulerAngles 	= Vector3.zero;
		transform.localScale 		= Vector3.one;
	}
}