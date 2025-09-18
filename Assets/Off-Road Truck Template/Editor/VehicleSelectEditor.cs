//--------------------------------------------------------------
//
//                    Car Parking Kit
//          Writed by AliyerEdon in summer 2016
//           Contact me : aliyeredon@gmail.com
//
//--------------------------------------------------------------

// This script is VehicleSelect.cs Editor\Inspector layout


using UnityEngine;
using System.Collections;
using UnityEditor;


[CustomEditor (typeof(VehicleSelect))][CanEditMultipleObjects]
public class VehicleSelectSelectEditor : Editor
{
	public override void OnInspectorGUI ()
	{

		serializedObject.Update ();

		EditorGUILayout.Space ();

		GUI.color = Color.green;
		EditorGUILayout.Space ();
		EditorGUILayout.HelpBox ("\n                           Vehicle Select System\n", MessageType.None);
		EditorGUILayout.Space ();
		GUI.color = Color.white;






		EditorGUILayout.HelpBox ("List of the vehicles", MessageType.None);

		EditorGUILayout.PropertyField (serializedObject.FindProperty ("vehicles"),
			new GUIContent ("Vehicles", "Drag youre Vehicle prefabs"), true);

		EditorGUILayout.Space ();

		EditorGUILayout.HelpBox ("Vehicle spawn point", MessageType.None);
		EditorGUILayout.PropertyField (serializedObject.FindProperty ("point"),
			new GUIContent ("Spawn Point", "Drag spawn point transform"), true);
		EditorGUILayout.Space ();



		EditorGUILayout.HelpBox ("List of the Vehicle prices", MessageType.None);
		EditorGUILayout.PropertyField (serializedObject.FindProperty ("Values"),
			new GUIContent ("Vehicle Price List", "Enter list of the Vehicle prices"), true);
		EditorGUILayout.Space ();



		EditorGUILayout.HelpBox ("Icon - Button - Shop", MessageType.None);
		EditorGUILayout.PropertyField (serializedObject.FindProperty ("Lock"),
			new GUIContent ("Lock Icon", "Drag lock icon image   "), true);
		EditorGUILayout.PropertyField (serializedObject.FindProperty ("Shop"),
			new GUIContent ("Shop Window", "Drag Shop Window image   "), true);

		EditorGUILayout.PropertyField (serializedObject.FindProperty ("Buy"),
			new GUIContent ("Buy Button", "Buy Button Object   "), true);
		EditorGUILayout.Space ();



		EditorGUILayout.HelpBox ("Info Texts", MessageType.None);
		EditorGUILayout.PropertyField (serializedObject.FindProperty ("TotalScore"),
			new GUIContent ("Total Score Text", "Drag Total Score Text   "), true);
		EditorGUILayout.PropertyField (serializedObject.FindProperty ("VehicleValue"),
			new GUIContent ("Vehicle Price Text", "Drag Vehicle Price Text   "), true);
		EditorGUILayout.Space ();



		EditorGUILayout.HelpBox ("Main Level Name", MessageType.None);
		EditorGUILayout.PropertyField (serializedObject.FindProperty ("LevelNameDay"),
			new GUIContent ("Level Name Day", "Enter Main Scene Day Mode Name  "), true);
		EditorGUILayout.Space ();
		EditorGUILayout.PropertyField (serializedObject.FindProperty ("LevelNameNight"),
			new GUIContent ("Level Name Night", "Enter Main Scene Night Mode Name  "), true);
		EditorGUILayout.Space ();



		EditorGUILayout.HelpBox ("Loading Window", MessageType.None);
		EditorGUILayout.PropertyField (serializedObject.FindProperty ("Loading"),
			new GUIContent ("Loading Object", "Drag Loading GameObject  "), true);
		EditorGUILayout.Space ();










		serializedObject.ApplyModifiedProperties ();
	}

}
