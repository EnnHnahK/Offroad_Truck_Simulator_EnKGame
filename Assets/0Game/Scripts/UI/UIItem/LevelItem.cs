using UnityEngine;
using UnityEngine.UI;

public class LevelItem : MonoBehaviour
{
	[SerializeField] private Text levelText;

	[SerializeField] private GameObject locked, playButton;

	[SerializeField] private Button btnPlay;

	[SerializeField] private Image levelImage;

	private int _levelId, _packID;

	private float _baseFuel;

	private bool _isLooked;

	private void Awake()
	{
		btnPlay.onClick.AddListener(() =>
		{
			if (UIController.Instance.uIChoseLevel.canTarget)
			{
				ChooseLevel();
			}
		});
	}

	public void Init(DataLevel.Level level, int packID)
	{
		_levelId = level.id;
		levelText.text = "LVL " + (_levelId + 1);

		_baseFuel = level.baseFuel;
		
		_packID = packID;
		levelImage.sprite = level.levelSprite;
		if (DataController.MapLevel >= _levelId)
		{
			_isLooked = false;
			locked.SetActive(false);
			playButton.gameObject.SetActive(true);
		}
		else
		{
			_isLooked = true;
			locked.SetActive(true);
			playButton.gameObject.SetActive(false);
		}
	}

	public void UnlockMap()
	{
		_isLooked = false;
		locked.SetActive(false);
		playButton.gameObject.SetActive(true);
	}

	private void ChooseLevel()
	{
		//Sound 
		//SoundManager.Instance.PlayShot(SoundManager.Instance.click);
		
		if (_isLooked)
		{
			#if UNITY_EDITOR
				Debug.Log("Level has been Locked");
			#endif
			UIController.Instance.Notify(Consts.unavailable);
			return;
		}
		
		UIController.Instance.uIChoseLevel.ChoseLevel(_levelId, _baseFuel);
	}
}
