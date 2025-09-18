using System.Collections.Generic;
using UnityEngine;

public static class RectExtension {
	public static RectTransform rect(this Component c)
    {
        return c.GetComponent<RectTransform>();
    }
}
