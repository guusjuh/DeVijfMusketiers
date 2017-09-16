using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;

/// <summary>
/// SCG WWW form extension.
/// </summary>
public static class SCGWWWFormExtension
{
	/// <summary>
	/// The signature key.
	/// </summary>
	private static readonly byte[] SIGNATURE_KEY = Encoding.ASCII.GetBytes ("S1sglsWxrUNxwQTOFx53YI");

	/// <summary>
	/// Create the specified form with parameters and sign.
	/// </summary>
	/// <param name="form">Form.</param>
	/// <param name="parameters">Parameters.</param>
	/// <param name="sign">If set to <c>true</c> sign.</param>
	public static WWWForm Create (this WWWForm form, Dictionary<string, object> parameters, bool sign = true)
	{
		// Add parameters
		foreach(KeyValuePair<string, object> p in parameters)
		{
			form.AddField(p.Key, p.Value.ToString());
		}

		// Sign form
		if (sign) form.Sign();

		// Return form
		return form;
	}

	/// <summary>
	/// Sign the specified form.
	/// </summary>
	/// <param name="form">Form.</param>
	public static void Sign (this WWWForm form)
	{
		form.AddField ("sig", GetSignature(form.data));
	}

	/// <summary>
	/// Gets the signature.
	/// </summary>
	/// <returns>The signature.</returns>
	/// <param name="input">Input.</param>
	private static string GetSignature (byte[] input)
	{
		// Convert input
		string inputString 		= Encoding.UTF8.GetString (input).ToLower ();
		byte[] lowerCaseInput 	= Encoding.UTF8.GetBytes (inputString);
		
		// Get hashed string
		return GetHashedString (lowerCaseInput, SIGNATURE_KEY);
	}

	/// <summary>
	/// Gets the hashed string by HMACSHA1.
	/// </summary>
	/// <returns>The hashed string.</returns>
	/// <param name="input">Input.</param>
	/// <param name="key">Key.</param>
	private static string GetHashedString (byte[] input, byte[] key)
	{
		// Compute hash from key
		HMACSHA1 hmacSHA1 = new HMACSHA1 (key);
		
		// Create stream
		MemoryStream stream	= new MemoryStream (input);
		
		// Compute hash from stream
		byte[] hashedInput = hmacSHA1.ComputeHash (stream);
		
		// Return hashed string
		return BitConverter.ToString (hashedInput).Replace ("-", "").ToLower ();
	}
}