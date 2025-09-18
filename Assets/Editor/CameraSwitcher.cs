using System.Collections;
using UnityEngine;
using UnityEditor;


/// <summary>
/// Adds a 'Camera' menu containing various views to switch between in the current SceneView
/// </summary>
public class CameraSwitcher
{
	/// <summary>
	/// The rotation to restore when going back to perspective view. If we don't have anything,
	/// default to the 'Front' view. This avoids the problem of an invalid rotation locking out
	/// any further mouse rotation
	/// </summary>
	static private Quaternion sLastRotation = Quaternion.Euler(0, 0, 0);

	/// <summary>
	/// Whether the camera should tween between views or snap directly to them
	/// </summary>
	static private bool sShouldTween = true;

	/// <summary>
	/// When switching from a perspective view to an orthographic view, record the rotation so
	/// we can restore it later
	/// </summary>
	static private void StorePerspective()
	{
		sLastRotation = SceneView.lastActiveSceneView.rotation;
	}

	/// <summary>
	/// Apply an orthographic view to the scene views camera. This stores the previously active
	/// perspective rotation if required
	/// </summary>
	/// <param name="newRotation">The new rotation for the orthographic camera</param>
	static private void ApplyOrthoRotation(Quaternion newRotation)
	{
		StorePerspective();

		if (sShouldTween)
		{
			SceneView.lastActiveSceneView.LookAt(SceneView.lastActiveSceneView.pivot, newRotation);
		}
		else
		{
			SceneView.lastActiveSceneView.LookAtDirect(SceneView.lastActiveSceneView.pivot, newRotation);
		}

		SceneView.lastActiveSceneView.Repaint();
	}

	[UnityEditor.MenuItem("Camera/Top")]
	static void TopCamera()
	{
		ApplyOrthoRotation(Quaternion.Euler(90, 0, 0));
	}

	[UnityEditor.MenuItem("Camera/Bottom")]
	static void BottomCamera()
	{
		ApplyOrthoRotation(Quaternion.Euler(-90, 0, 0));
	}

	[UnityEditor.MenuItem("Camera/Left")]
	static void LeftCamera()
	{
		ApplyOrthoRotation(Quaternion.Euler(0, 90, 0));
	}

	[UnityEditor.MenuItem("Camera/Right")]
	static void RightCamera()
	{
		ApplyOrthoRotation(Quaternion.Euler(0, -90, 0));
	}

	[UnityEditor.MenuItem("Camera/Front")]
	static void FrontCamera()
	{
		ApplyOrthoRotation(Quaternion.Euler(0, 0, 0));
	}

	[UnityEditor.MenuItem("Camera/Back")]
	static void BackCamera()
	{
		ApplyOrthoRotation(Quaternion.Euler(0, 180, 0));
	}

	[UnityEditor.MenuItem("Camera/Back to Last Rotation")]
	static void BackToLastRotationCamera()
	{
		if (sShouldTween)
		{
			SceneView.lastActiveSceneView.LookAt(SceneView.lastActiveSceneView.pivot, sLastRotation);
		}
		else
		{
			SceneView.lastActiveSceneView.LookAtDirect(SceneView.lastActiveSceneView.pivot, sLastRotation);
		}

		StorePerspective();

		SceneView.lastActiveSceneView.Repaint();
	}

	[UnityEditor.MenuItem("Camera/Switch View Type #8")]
	static void SwitchViewTypeCamera()
	{
		SceneView.lastActiveSceneView.orthographic = !SceneView.lastActiveSceneView.orthographic;
	}
}