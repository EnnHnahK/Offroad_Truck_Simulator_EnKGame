using UnityEditor;
using UnityEngine;
using System.Linq;
using System;
using System.Reflection;
using UnityEngine.UI;

[CustomPropertyDrawer(typeof(ButtonAttribute))]
public class ButtonAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        base.OnGUI(position, property, label);
    }
}

[CustomEditor(typeof(MonoBehaviour), true)]
public class ButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var methods = target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        foreach (var method in methods)
        {
            var bMethod = method.GetCustomAttribute(typeof(ButtonAttribute), true);
            if (bMethod != null)
            {
                if (GUILayout.Button(method.Name))
                {
                    method.Invoke(target, null);
                }
            }
        }

        DrawDefaultInspector();
    }
}

public static class MenuExtention
{
    [MenuItem("GameObject/UI/button_effect $b")]
    public static void CreateButtonEffect()
    {
        var btn = new GameObject("button_effect", typeof(RectTransform), typeof(ButtonEffectLogic),
            typeof(UnityEngine.UI.Image));
        btn.transform.SetParent(Selection.transforms[0]);
        btn.transform.localScale = Vector3.one;
        btn.transform.localPosition = Vector3.zero;
        btn.layer = 5;
        var bImg = btn.GetComponent<Image>();
        bImg.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_GAME/Sprites/Square.png");
        bImg.color = new Color32(0, 0, 0, 1);
        var img = new GameObject("sprite", typeof(RectTransform), typeof(Image)).GetComponent<Image>();
        img.raycastTarget = false;
        img.maskable = false;
        img.transform.SetParent(btn.transform);
        img.transform.localPosition = Vector3.zero;
        img.transform.localScale = Vector3.one;
        img.gameObject.layer = 5;
        Selection.activeObject = btn;
    }
}