using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public static class Yielder
{
    class FloatComparer : IEqualityComparer<float>
    {
        bool IEqualityComparer<float>.Equals(float x, float y)
        {
            return x == y;
        }

        int IEqualityComparer<float>.GetHashCode(float obj)
        {
            return obj.GetHashCode();
        }
    }

    private static Dictionary<float, WaitForSeconds> _timeInterval = new Dictionary<float, WaitForSeconds>(100, new FloatComparer());
    private static WaitForEndOfFrame _endOfFrame = new WaitForEndOfFrame();
    private static WaitForFixedUpdate _fixedUpdate = new WaitForFixedUpdate();

    public static WaitForEndOfFrame GetWaitForEndOfFrame()
    {
        return _endOfFrame;
    }

    public static WaitForFixedUpdate GetWaitForFixedUpdate()
    {
        return _fixedUpdate;
    }

    public static WaitForSeconds GetWaitForSeconds(float seconds)
    {
		if (!_timeInterval.TryGetValue(seconds, out WaitForSeconds waitForSeconds))
		{
			_timeInterval.Add(seconds, waitForSeconds = new WaitForSeconds(seconds));
		}

		return waitForSeconds;
    }
}