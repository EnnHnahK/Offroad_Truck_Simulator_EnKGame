using UnityEngine;
using UnityEngine.UI;

public class LanguageItem : MonoBehaviour
{
	[SerializeField] private Button selectLang;

	[SerializeField] private Text txtName;

	[SerializeField] private GameObject langOn;

	private int _indexLang;

	private void Awake()
	{
		selectLang.onClick.AddListener(ChooseLang);
	}

	public void Init(int index, string langName)
	{
		txtName.text = langName;
		_indexLang = index;
	}

	public void ChooseLang()
	{
		langOn.SetActive(true);
		UIController.Instance.uISetting.ChangeLanguage(_indexLang);
		
		//Sound 
		SoundManager.Instance.PlayShot(SoundManager.Instance.click);
	}

	public void UnSelected()
	{
		langOn.SetActive(false);
	}

	public void LocalizationUpdate(string langName)
	{
		txtName.text = langName;
	}
	
}
