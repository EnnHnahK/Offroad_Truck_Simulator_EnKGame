using System;
using System.Collections.Generic;
using System.Globalization;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class Utils
{
    private static int UILayer = -1;
    //Returns 'true' if we touched or hovering on Unity UI element.
    public static bool IsPointerOverUIElement()
    {
        if (UILayer == -1)
        {
            UILayer = LayerMask.NameToLayer("UI");
        }
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }
 
 
    //Returns 'true' if we touched or hovering on Unity UI element.
    private static bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
    {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == UILayer)
            {
                return true;
            }
        }
        return false;
    }
 
 
    //Gets all event system raycast results of current mouse or touch position.
    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }



    public static string GetNumberAroundString(this int input)
    {
        if (input < 5000)
        {
            return input.ToString();
        }
        else if (input < 1_000_000)
        {
            return input / 1000 + "K";
        }
        else if (input < 1_000_000_000)
        {
            return input / 1_000_000 + "M";
        }
        else if (input < 2_147_483_647)
        {
            return input / 1_000_000_000 + "B";
        }

        return input.ToString();
    }
    public static string GetNumberAroundString(this long input)
    {
        if (input < 5000)
        {
            return input.ToString();
        }
        else if (input < 1_000_000)
        {
            return input / 1000 + "K";
        }
        else if (input < 1_000_000_000)
        {
            return input / 1_000_000 + "M";
        }
        else if (input < 1_000_000_000_000)
        {
            return input / 1_000_000_000 + "B";
        }

        return input.ToString();
    }
    public static int GetNumberAround(this int input)
    {
        if (input < 5000)
        {
            return input;
        }
        else if (input < 1_000_000)
        {
            return (input / 1000) * 1000;
        }
        else if (input < 1_000_000_000)
        {
            return (input / 1_000_000) * 1_000_000;
        }
        else if (input < 2_147_483_647)
        {
            return (input / 1_000_000_000) * 1_000_000_000;
        }
    
        return input;
    }
    public static long GetNumberAround(this long input)
    {
        if (input < 5000)
        {
            return input;
        }
        else if (input < 1_000_000)
        {
            return (input / 1000) * 1000;
        }
        else if (input < 1_000_000_000)
        {
            return (input / 1_000_000) * 1_000_000;
        }
        else if (input < 1_000_000_000_000)
        {
            return (input / 1_000_000_000) * 1_000_000_000;
        }
    
        return input;
    }

    public static string GetNumberFormat(this int input)
    {
        string formatted = string.Format(CultureInfo.InvariantCulture, "{0:N0}", input);
        return formatted;
    }
    
    public static string FormatNumber(int number)
    {
        return number.ToString("N0", System.Globalization.CultureInfo.InvariantCulture);
    }

    public static void SetNativeImage(Image image, Vector2 parentSize, float scale = 1f)
    {
        //var parentSize = image.GetComponent<RectTransform>().sizeDelta;
        var imgSize = new Vector2(image.sprite.texture.width, image.sprite.texture.height);

        var parentRatio = parentSize.x / parentSize.y;
        var imgRatio = imgSize.x / imgSize.y;

        if (parentRatio > imgRatio)
        {
            var yMax = parentSize.y;
            var x = (parentSize.y / imgSize.y) * imgSize.x;
            image.rectTransform.sizeDelta = new Vector2(x, yMax) * scale;
        }
        else
        {
            var y = (parentSize.x / imgSize.x) * imgSize.y;
            var xMax = parentSize.x;
            image.rectTransform.sizeDelta = new Vector2(xMax, y) * scale;
        }
    }

    public static void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

}


public static class Vibration
{
#if UNITY_ANDROID && !UNITY_EDITOR
    public static AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    public static AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
    public static AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
#else
    public static AndroidJavaClass unityPlayer;
    public static AndroidJavaObject currentActivity;
    public static AndroidJavaObject vibrator;
#endif

    public static void Vibrate()
    {
        // if (UIController.instace.vibrate == 0)
        // {
        //     return;
        // }

        if (isAndroid())
            vibrator.Call("vibrate");
        else
            Handheld.Vibrate();
    }


    public static void Vibrate(long milliseconds)
    {
        // if (UIController.instace.vibrate == 0)
        // {
        //     return;
        // }

        if (isAndroid())
            vibrator.Call("vibrate", milliseconds);
        else
            Handheld.Vibrate();
    }

    public static bool HasVibrator()
    {
        return isAndroid();
    }

    public static void Cancel()
    {
        if (isAndroid())
            vibrator.Call("cancel");
    }

    private static bool isAndroid()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
	return true;
#else
        return false;
#endif
    }
}