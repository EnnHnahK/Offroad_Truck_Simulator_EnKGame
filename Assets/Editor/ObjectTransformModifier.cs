using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class ObjectTransformModifier
{
	private static Quaternion _lastRotation = Quaternion.Euler(0, 0, 0);
	private static Vector3 _lastPosition = Vector3.zero;

	private static void ApplyRotationToSelectedGameObject(Quaternion rotation)
	{
		_lastRotation = Selection.activeTransform.rotation;

		Selection.activeTransform.rotation = rotation;
	}

	private static void ApplyPositionToSelectedGameObject(Vector3 position)
	{
		_lastPosition = Selection.activeTransform.position;

		Selection.activeTransform.position = position;
	}

	[UnityEditor.MenuItem("Camera/GameObject to Camera Position")]
	private static void ApplyCurrentPositionToSelectedGameObject()
	{
		ApplyPositionToSelectedGameObject(SceneView.lastActiveSceneView.camera.transform.position);
		_lastRotation = Selection.activeTransform.rotation;
	}

	[UnityEditor.MenuItem("Camera/GameObject to Camera Position & Rotation")]
	private static void ApplyCurrentPositionAndRotationToSelectedGameObject()
	{
		ApplyPositionToSelectedGameObject(SceneView.lastActiveSceneView.camera.transform.position);
		ApplyRotationToSelectedGameObject(SceneView.lastActiveSceneView.rotation);
	}

	[UnityEditor.MenuItem("Camera/Undo GameObject Position & Rotation")]
	private static void UndoPositionAndRotation()
	{
		ApplyPositionToSelectedGameObject(_lastPosition);
		ApplyRotationToSelectedGameObject(_lastRotation);
	}

	[UnityEditor.MenuItem("GameObject/Mass Scale Randomizer (0.75f ~ 1.25f)")]
	private static void MassScaleRandomizer()
	{
		foreach (var targetGameObject in UnityEditor.Selection.gameObjects)
		{
			if (!targetGameObject)
				continue;

			float randomizedValue = UnityEngine.Random.Range(0.75f, 1.25f);

			if (targetGameObject.transform.childCount != 0 && targetGameObject.transform.GetChild(0).name == "ScalableSection")
			{
				targetGameObject.transform.GetChild(0).localScale = targetGameObject.transform.localScale * randomizedValue;
			}
			else
			{
				targetGameObject.transform.localScale = targetGameObject.transform.localScale * randomizedValue;
			}

			if (targetGameObject.TryGetComponent<Rigidbody>(out Rigidbody rigidbody))
			{
				rigidbody.mass *= randomizedValue;
			} 
		}
	}
}