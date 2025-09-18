/*
	A simple little editor extension to copy and paste all components
	Help from http://answers.unity3d.com/questions/541045/copy-all-components-from-one-character-to-another.html
	license: WTFPL (http://www.wtfpl.net/)
	author: aeroson
	advise: ChessMax
	editor: frekons
*/

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

public class ComponentsCopier
{
	static Component[] copiedComponents;
	static Transform copiedTransform;

	[MenuItem("GameObject/Copy Transform")]
	static void CopyTransform()
	{
		if (UnityEditor.Selection.activeGameObject == null)
		{
			Debug.Log("Nothing is chosen!");

			return;
		}

		Debug.Log("Successfully copied Transform.");

		copiedTransform = UnityEditor.Selection.activeGameObject.GetComponent<Transform>();
	}

	[MenuItem("GameObject/Paste Transform")]
	static void PasteTransform()
	{
		foreach (var targetGameObject in UnityEditor.Selection.gameObjects)
		{
			if (!targetGameObject)
				continue;

			Undo.RegisterCompleteObjectUndo(targetGameObject, targetGameObject.name + ": Paste Transform");

			UnityEditorInternal.ComponentUtility.CopyComponent(copiedTransform);

			Transform targetTransform = targetGameObject.GetComponent<Transform>();

			if (UnityEditorInternal.ComponentUtility.PasteComponentValues(targetTransform))
			{
				Debug.Log("Successfully pasted Transform.");
			}
			else
			{
				Debug.LogError("Failed to copy Transform.");
			}
		}
	}

	[MenuItem("GameObject/Copy all components %&C")]
	static void Copy()
	{
		if (UnityEditor.Selection.activeGameObject == null)
			return;

		Debug.Log("Successfully copied every Components.");

		copiedComponents = UnityEditor.Selection.activeGameObject.GetComponents<Component>();
	}

	[MenuItem("GameObject/Paste all components %&P")]
	static void Paste()
	{
		if (copiedComponents == null)
		{
			Debug.LogError("Nothing is copied!");
			return;
		}

		foreach (var targetGameObject in UnityEditor.Selection.gameObjects)
		{
			if (!targetGameObject)
				continue;

			Undo.RegisterCompleteObjectUndo(targetGameObject, targetGameObject.name + ": Paste All Components"); // sadly does not record PasteComponentValues, i guess

			foreach (var copiedComponent in copiedComponents)
			{
				if (!copiedComponent)
					continue;

				UnityEditorInternal.ComponentUtility.CopyComponent(copiedComponent);

				var targetComponent = targetGameObject.GetComponent(copiedComponent.GetType());

				if (targetComponent) // if gameObject already contains the component
				{
					if (UnityEditorInternal.ComponentUtility.PasteComponentValues(targetComponent))
					{
						Debug.Log("Successfully pasted: " + copiedComponent.GetType());
					}
					else
					{
						Debug.LogError("Failed to copy: " + copiedComponent.GetType());
					}
				}
				else // if gameObject does not contain the component
				{
					if (UnityEditorInternal.ComponentUtility.PasteComponentAsNew(targetGameObject))
					{
						Debug.Log("Successfully pasted: " + copiedComponent.GetType());
					}
					else
					{
						Debug.LogError("Failed to copy: " + copiedComponent.GetType());
					}
				}
			}
		}

		copiedComponents = null; // to prevent wrong pastes in future
	}

}
#endif