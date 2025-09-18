using DG.Tweening;
using UnityEngine;

public class TruckItemCount : MonoBehaviour
{
	private Vector3[] _pos;
	
    [Header("Mission Default"), Tooltip("Used for transporting wood, stone, vehicles || Mission Type 0 -> 2")]
	[SerializeField] private Transform itemStatic;
	[SerializeField] private GameObject itemsMission;
	
	[Header("Vehicle Transport"), Tooltip("Used by Vehicle Transport missions || Mission Type 2")]
	[SerializeField] private Transform[] itemPos;

	[Header("Liquid Mission"), Tooltip("Used by Liquid Transport missions || Mission Type 4")]
	[SerializeField] private Material materialAlpha;
	[SerializeField] private MeshRenderer meshContainer, liquidRender;
	private Material _liquidMat;
	private static readonly int Fill = Shader.PropertyToID("_Fill");

	[Header("Trailer Mission"), Tooltip("Used by Trailer Transport missions || Mission Type 6")]
	[SerializeField] private GameObject trailer;
	[SerializeField] private GameObject trailerController;
	
	private void Awake()
	{
		if (itemStatic.childCount <= 0)
		{
			return;	
		}
		_pos = new Vector3[itemStatic.childCount];
		for (int i = 0; i < itemStatic.childCount; i++)
		{
			_pos[i] = itemStatic.GetChild(i).localPosition;
		}
		
	}

	#region Mission Default

	public void ToggleItem(bool isActive, bool effect = true)
	{
		itemsMission.SetActive(isActive);
		itemStatic.gameObject.SetActive(!isActive);
		_TruckManager.Instance.ToggleCache(!isActive);
		if (!isActive)
		{
			ResetPos();	
		}
		else
		{
			//if(effect) FallItemEffect();
		}
	}
	
	
	void FallItemEffect()
	{
		if (itemsMission.transform.childCount <= 0)
		{
			return;
		}
		var position = itemsMission.transform.position;
		itemsMission.transform.DOMoveY(position.y, 1f).SetUpdate(true).From(position.y + 1f).SetEase(Ease.Linear);
		/*
		foreach (Transform item in itemsMission.transform)
		{
			var position = item.position;
			item.DOMoveY(position.y, 1f).SetUpdate(true).From(position.y + 1f).SetEase(Ease.Linear);
		}*/
	}
	
	public void TurnOffItemMission()
	{
		itemsMission.SetActive(false);
	}
	
	void ResetPos()
	{
		if (itemStatic.childCount <= 0)
		{
			return;	
		}
		for (int i = 0; i < itemStatic.childCount; i++)
		{
			itemStatic.GetChild(i).localPosition = _pos[i];
		}
	}

	#endregion

	#region Vehicle Transport Mission
	public void ReceivingVehicle(Transform item, int index)
	{
		if(index >= itemPos.Length) return;
		
		if (itemStatic.gameObject.activeSelf)
		{
			itemStatic.gameObject.SetActive(false);
			itemsMission.SetActive(true);
			_TruckManager.Instance.ToggleCache(false);
		}
		
		item.gameObject.SetActive(true);
		item.GetComponent<BoxCollider>().enabled = false;
		item.SetParent(itemPos[index]);
		item.localScale = Vector3.one;
		item.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
		item.DOLocalMoveY(0, 1f).SetUpdate(true).From(-1f).SetEase(Ease.Linear);
	}

	public void ReturnVehicle(int index)
	{
		if(index >= itemPos.Length) return;
		
		itemPos[index].gameObject.SetActive(false);
	}

	public void ToggleItemTransform(int index, bool isActive = false)
	{
		if (index > itemPos.Length) index = 0;
		itemPos[index].gameObject.SetActive(isActive);
	}

	public Transform GetVehicleItemTransform(int index)
	{
		if (index > itemPos.Length) index = 0;
		return itemPos[index];
	}
	
	public Vector3 GetVehicleItemPos(int index)
	{
		if (index >= itemPos.Length) index = 0;
		var pos = itemPos[index].position;
		pos.y += 2f;
		return pos;
	}
	
	public Vector3 GetVehicleItemEuler(int index)
	{
		if (index >= itemPos.Length) index = 0;
		return itemPos[index].transform.eulerAngles;
	}

	public Transform GetMissionItemTransform()
	{
		return itemsMission.transform;
	}

	#endregion

	#region Trailer Mission

	public void ToggleTrailer(bool isActive)
	{
		trailerController.transform.position = _TruckManager.Instance.GetItemPos();

		trailerController.transform.localRotation = _TruckManager.Instance.GetVehicleLocalRotation();
		
		trailer.SetActive(isActive);
	}

	#endregion
	
	#region Liquid Mission
	
	public void AlphaContainer(bool liquidUp)
	{
		if(_liquidMat == null) _liquidMat = liquidRender.material;

		float startValue = 1f, endValue = 0f;
		
		if (liquidUp)
		{
			startValue = 0f;
			endValue = 1f;
		}
		
		Material materialTemp = new Material(meshContainer.material);
		meshContainer.material = materialAlpha;
		DOTween.Sequence().Append(meshContainer.material.DOFade(.1f, 1f).From(1)).Append(DOTween.To(
				() => _liquidMat.GetFloat(Fill),
				value => _liquidMat.SetFloat(Fill, value),
				endValue, 
				2f
			).From(startValue).SetEase(Ease.OutQuad))
			.AppendCallback(() => meshContainer.material.DOFade(1f, 1).OnComplete(() =>
			{
				meshContainer.material = materialTemp;
			}));
		
	}

	#endregion

}
