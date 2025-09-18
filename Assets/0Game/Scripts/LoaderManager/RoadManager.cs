using System.Collections.Generic;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
	public static RoadManager Instance { get; private set; }

	[SerializeField] private List<Transform> _listOfRoadPoints = new();

	private List<float> _listOfLengthBetweenRoadPoints;
	private List<float> _listOfLengthPercentageBetweenRoadPoints;
	private bool _isRaceFinish;
	private int _curRoadPointIndex = 0;
	private float _curMaxLength = 0f;
	private float _curSectionPercentage = 0f;
	
	[SerializeField] private LineRenderer pathLine;
	[Min(1f)]
	[SerializeField] private float segmentSize = 10f;

	private void Awake()
	{
		Instance = this;
	}

	public void CalculatorDisRoad()
	{
		_listOfLengthBetweenRoadPoints = new List<float>();
		float totalLength = 0f;

		//		Calculate individual length for each Section
		for (int i = 0; i < _listOfRoadPoints.Count; i++)
		{
			Vector3 sectionPoint1;
			
			sectionPoint1 = i - 1 < 0 ? _TruckManager.Instance.GetVehicleControl().transform.position : _listOfRoadPoints[i - 1].transform.position;
			Vector3 sectionPoint2 = _listOfRoadPoints[i].position;

			float sectionLength = Vector3.Distance(sectionPoint1, sectionPoint2);
			totalLength += sectionLength;

			_listOfLengthBetweenRoadPoints.Add(sectionLength);
		}

		//		Convert that into percentages
		_listOfLengthPercentageBetweenRoadPoints = new List<float>();
		for (int i = 0; i < _listOfLengthBetweenRoadPoints.Count; i++)
		{
			_listOfLengthPercentageBetweenRoadPoints.Add(_listOfLengthBetweenRoadPoints[i] / totalLength);
		}

		_curRoadPointIndex = 0;

		_isRaceFinish = false;

		_curMaxLength = _listOfLengthBetweenRoadPoints[0];
	}
	
	private void CalcuPercent()
	{
		if (_TruckManager.Instance)
		{
			if (_curRoadPointIndex < 0)
			{
				_curSectionPercentage = 1;
				return;
			}

			_curSectionPercentage = 1f - Vector3.Distance(_TruckManager.Instance.GetVehicleControl().transform.position,
				_listOfRoadPoints[_curRoadPointIndex].position) / _curMaxLength;
		}
	}

	public void OnPointPast(Transform pointObject)
	{
		int indexPoint = _listOfRoadPoints.IndexOf(pointObject);
		if (!_listOfRoadPoints.Contains(pointObject) || _curRoadPointIndex == indexPoint)
		{
			return;
		}

		_curRoadPointIndex = indexPoint;

		if (_curRoadPointIndex + 3 < _listOfRoadPoints.Count)
		{
			DrawPath(_listOfRoadPoints.GetRange(_curRoadPointIndex, 4));
		}

		_curSectionPercentage = 0f;
		_curMaxLength = _listOfLengthBetweenRoadPoints[_curRoadPointIndex];
	}

	public void FirstDraw()
	{
		Transform t = _TruckManager.Instance.GetVehicleControl().transform;
		var firstPos = new List<Transform>(4) {t};
		//firstPos.AddRange(_listOfRoadPoints.Skip(_curRoadPointIndex).Take(2));
		firstPos.AddRange(_listOfRoadPoints.GetRange(_curRoadPointIndex, 3));
		DrawPath(firstPos);

		_curRoadPointIndex = -1;
	}
	
	public float GetRoadProgress()
	{
		if (_isRaceFinish) return 1;

		CalcuPercent();

		float actualPercentage = 0f;

		for (int i = 0; i < _listOfLengthPercentageBetweenRoadPoints.Count; i++)
		{
			if (i < _curRoadPointIndex)
			{
				actualPercentage += _listOfLengthPercentageBetweenRoadPoints[i];
			}
			else if (i == _curRoadPointIndex)
			{
				actualPercentage += _curSectionPercentage * _listOfLengthPercentageBetweenRoadPoints[i];
			}
		}
		
		return actualPercentage > 0f ? actualPercentage : 0f;
	}

	public float GetRoadLength()
	{
		float actualMeters = 0f;

		for (int i = 0; i < _listOfLengthBetweenRoadPoints.Count; i++)
		{
			actualMeters += _listOfLengthBetweenRoadPoints[i];
		}

		return actualMeters * 5;
	}

	public void SetFinish()
	{
		_isRaceFinish = true;
	}
	
	#if UNITY_EDITOR
	[Button]
	private void PreviewPath()
	{
		if (pathLine == null || _listOfRoadPoints.Count == 0)
		{
			Debug.LogError("LineRenderer or positions are not set.");
			return;
		}

		Vector3[] pos = new Vector3[_listOfRoadPoints.Count];

		
		for (int i = 0; i < _listOfRoadPoints.Count; i++)
		{
			pos[i] = _listOfRoadPoints[i].position;
		}
		
		pos = SmoothLine(pos, segmentSize);
		pathLine.positionCount = pos.Length;
		
		for (int i = 0; i < pos.Length; i++)
		{
			pathLine.SetPosition(i, pos[i]);
		}
	}

	[Button]
	private void EndPreview()
	{
		pathLine.positionCount = 0;
	}

	[SerializeField] Transform RoadPointTriggers;

	[Button]
	private void SetUpPathLine()
	{
		_listOfRoadPoints.Clear();
        foreach (Transform item in RoadPointTriggers)
        {
			_listOfRoadPoints.Add(item);
        }
    }


	#endif

	public void DrawCurTruckPos()
	{
		Transform t = _TruckManager.Instance.GetVehicleControl().transform;
		var listPos = new List<Transform>(4) {t};
		listPos.AddRange(_listOfRoadPoints.GetRange(_curRoadPointIndex, 3));
		DrawPath(listPos);
	}

	public void DrawCurTruckPos(Transform pos)
	{
		Transform t = _TruckManager.Instance.GetVehicleControl().transform;
		var listPos = new List<Transform>(2) {t};
		listPos.Add(pos);
		DrawPath(listPos);
	}
	
	private void DrawPath(List<Transform> listPos)
	{
		if (pathLine == null || listPos.Count == 0)
		{
			Debug.LogError("LineRenderer or positions are not set.");
			return;
		}
		
		Vector3[] pos = new Vector3[listPos.Count];

		for (int i = 0; i < listPos.Count; i++)
		{
			pos[i] = listPos[i].position;
		}
		
		pos = SmoothLine(pos, segmentSize);
		pathLine.positionCount = pos.Length;
		
		for (int i = 0; i < pos.Length; i++)
		{
			pathLine.SetPosition(i, pos[i]);
		}
	}
	

	private static Vector3[] SmoothLine(Vector3[] inputPoints, float segmentSize)
    {
        AnimationCurve curveX = new AnimationCurve();
        AnimationCurve curveY = new AnimationCurve();
        AnimationCurve curveZ = new AnimationCurve();
        
        Keyframe[] keysX = new Keyframe[inputPoints.Length];
        Keyframe[] keysY = new Keyframe[inputPoints.Length];
        Keyframe[] keysZ = new Keyframe[inputPoints.Length];

  
        for (int i = 0; i < inputPoints.Length; i++)
        {
            keysX[i] = new Keyframe(i, inputPoints[i].x);
            keysY[i] = new Keyframe(i, inputPoints[i].y);
            keysZ[i] = new Keyframe(i, inputPoints[i].z);
        }
        
        curveX.keys = keysX;
        curveY.keys = keysY;
        curveZ.keys = keysZ;
        
        for (int i = 0; i < inputPoints.Length; i++)
        {
            curveX.SmoothTangents(i, 0);
            curveY.SmoothTangents(i, 0);
            curveZ.SmoothTangents(i, 0);
        }
        
        List<Vector3> lineSegments = new List<Vector3>();
        
        for (int i = 0; i < inputPoints.Length; i++)
        {
            lineSegments.Add(inputPoints[i]);
            if (i + 1 < inputPoints.Length)
            {
                float distanceToNext = Vector3.Distance(inputPoints[i], inputPoints[i + 1]);
                int segments = (int)(distanceToNext / segmentSize);

                for (int s = 1; s < segments; s++)
                {
                    float time = ((float)s / (float)segments) + (float)i;
                    Vector3 newSegment = new Vector3(curveX.Evaluate(time), curveY.Evaluate(time), curveZ.Evaluate(time));
                    lineSegments.Add(newSegment);
                }
            }
        }

        return lineSegments.ToArray();
    }

	public bool CheckGotoCorrect()
	{
		return _curRoadPointIndex * 1.6f > _listOfRoadPoints.Count;
	}
	
}
