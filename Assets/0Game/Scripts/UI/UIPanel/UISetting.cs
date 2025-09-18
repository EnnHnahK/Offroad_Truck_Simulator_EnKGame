using System.Collections.Generic;
using I2.Loc;
using UnityEngine;
using UnityEngine.UI;

public class UISetting : BasePanel
{
	[SerializeField] private Button btnMusic, btnSound, btnQuality, btnVibration, btnLang, btnConsent, btnPrivacy;

	[SerializeField] private GameObject buttonGroup, musicOn, soundOn, qualityOn, vibrateOn, langParent;
	[SerializeField] private LanguageItem _languageItem;
	
	private readonly List<string> _languages = new () 
	{
		"English", "Japanese", "Vietnamese", "Korean", "Polish", "French", "German", "Italian", "Portuguese", "Spanish",
		"Turkish", "Thai", "Russian", "Chinese", "Indonesian"
	};

	private int _indexTempLanguage, _indexQualityGame;
	
	public List<LanguageItem> languageItems = new();

	[SerializeField] private ShowEffect showLangEffect;
	[SerializeField] private ScrollRect langScroll;

	protected override void Awake()
	{
		base.Awake();
		btnMusic.onClick.AddListener(ChangeMusic);
		btnSound.onClick.AddListener(ChangeSound);
		btnQuality.onClick.AddListener(ChangeQuality);
		btnVibration.onClick.AddListener(ChangeVibrate);
		
		LoadLanguage();
		btnPrivacy.onClick.AddListener(OpenPrivacy);
		
		btnLang.onClick.AddListener(() =>
		{
			buttonGroup.SetActive(false);
			showLangEffect.gameObject.SetActive(true);
		});
		
		/*
		btnConsent.onClick.AddListener(UMP.ShowPrivacyOptionsForm);
		btnConsent.gameObject.SetActive(CMPManager.CMP_OK);*/
	}

	private void Start()
	{
		_indexQualityGame = QualitySettings.GetQualityLevel();
		
		soundOn.SetActive(DataController.Sound);
		qualityOn.SetActive(DataController.Quality);
		vibrateOn.SetActive(DataController.Vibrate);
		musicOn.SetActive(DataController.Music);
		
		if(!DataController.Quality)
		{
			QualitySettings.SetQualityLevel(0,false);
		}
		else
		{
			QualitySettings.SetQualityLevel(_indexQualityGame, true);
		}
	}

	public override void Hide()
	{
		if (showLangEffect.gameObject.activeSelf)
		{
			langScroll.verticalNormalizedPosition = 1f;
			showLangEffect.Hide();
			buttonGroup.SetActive(true);
			return;
		}
		ShowAfterHide(UIController.Instance.uIGarage);
	}

	private void LoadLanguage()
	{
		_indexTempLanguage = PrefData.LanguageIndex;
		
		if (_indexTempLanguage < 0)// Not select language AKA first play on device 
		{
			var languageSelect = Application.systemLanguage.ToString();
			_indexTempLanguage = _languages.IndexOf(languageSelect);

			_indexTempLanguage = _indexTempLanguage >= 0 ? _indexTempLanguage : 0; // if not support language then default is EN
			PrefData.LanguageIndex = _indexTempLanguage;
		}
		
		LocalizationManager.CurrentLanguage = _languages[_indexTempLanguage];
		LocalizationManager.UpdateSources();
		
		for (int i = 0; i < _languages.Count; i++)
		{
			var lang = Instantiate(_languageItem, langParent.transform);
			lang.Init(i, LocalizationManager.GetTranslation( _languages[i]));
			languageItems.Add(lang);
		}
		languageItems[_indexTempLanguage].ChooseLang();
	}
	
	public void ChangeLanguage(int index)
	{
		if (_indexTempLanguage == index)
		{
			return;
		}
		
		languageItems[_indexTempLanguage].UnSelected();
		_indexTempLanguage = index;

		PrefData.LanguageIndex = index;
		LocalizationManager.CurrentLanguage = _languages[_indexTempLanguage];
		LocalizationManager.UpdateSources();
		
		UIController.Instance.LocalizationUpdate();
		
		for (int i = 0; i < _languages.Count; i++)
		{
			languageItems[i].LocalizationUpdate(LocalizationManager.GetTranslation( _languages[i]));
		}
	}

	public void ChangeMusic()
	{
		//Sound 
		SoundManager.Instance.PlayShot(SoundManager.Instance.click);
		
		var music = !DataController.Music;
		PrefData.Music = music;
		musicOn.SetActive(music);
		SoundManager.Instance.UpdateMute();
	}

	public void ChangeSound()
	{
		//Sound 
		SoundManager.Instance.PlayShot(SoundManager.Instance.click);
		
		var sound = !DataController.Sound;
		PrefData.Sound = sound;
		soundOn.SetActive(sound);
		SoundManager.Instance.UpdateMute();
	}
	
	public void ChangeQuality()
	{
		//Sound 
		SoundManager.Instance.PlayShot(SoundManager.Instance.click);
		
		var quality = !DataController.Quality;
		PrefData.Quality = quality;
		qualityOn.SetActive(quality);
		if(!quality)
		{
			QualitySettings.SetQualityLevel(0,false);
		}
		else
		{
			QualitySettings.SetQualityLevel(_indexQualityGame, true);
		}
	}

	
	public void ChangeVibrate()
	{
		//Sound 
		SoundManager.Instance.PlayShot(SoundManager.Instance.click);
		
		var vibrate = !DataController.Vibrate;
		PrefData.Vibrate = vibrate;
		if (vibrate)
		{
			SoundManager.Instance.PlayEmphasis(.5f, .7f);
		}
		vibrateOn.SetActive(vibrate);
		//vibrateOff.SetActive(!vibrate);
	}
	
	private void OpenPrivacy()
	{
		Application.OpenURL("https://myroargame.com/privacy-policy");
	}


}
