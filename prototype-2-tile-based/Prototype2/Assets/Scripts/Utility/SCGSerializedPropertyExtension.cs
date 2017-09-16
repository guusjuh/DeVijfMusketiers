using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// SCG Object extension.
/// </summary>
public static class SCGSerializedPropertyExtension
{
	/// <summary>
	/// Adds the object to list.
	/// </summary>
	/// <param name="serializedProperty">Serialized property.</param>
	/// <param name="objectList">Object list.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static int AddObjectToList <T> (this SerializedProperty serializedProperty, List <T> objectList, T listObject, int index = -1, SerializedProperty indexProperty = null) 
	{
		// Update index
		if (index.IsNegative ()) index = serializedProperty.arraySize;

		// Insert index
		serializedProperty.InsertArrayElementAtIndex (index);

		// If index property exists, update index property
		if (indexProperty.Exists ()) indexProperty.intValue = index;

		// Flush serializedProperty
		serializedProperty.serializedObject.ApplyModifiedProperties ();

		// Set object
		objectList [index] = listObject;

		// Return index
		return index;
	}

	/// <summary>
	/// Removes the object from list.
	/// </summary>
	/// <param name="serializedProperty">Serialized property.</param>
	/// <param name="index">Index.</param>
	public static void RemoveObjectFromList (this SerializedProperty serializedProperty, int index, SerializedProperty indexProperty = null)
	{
		// Remove serializedProperty at index
		serializedProperty.DeleteArrayElementAtIndex (index);

		// If index property exists and update necessary, update index property
		if (indexProperty.Exists () && index >= serializedProperty.arraySize) indexProperty.intValue = (index - 1).Min (0);

		// Flush serializedProperty
		serializedProperty.serializedObject.ApplyModifiedProperties ();
	}
}