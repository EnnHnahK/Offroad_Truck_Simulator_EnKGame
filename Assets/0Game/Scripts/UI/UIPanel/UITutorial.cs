using DG.Tweening;
using UnityEngine;

public class UITutorial : MonoBehaviour
{
	public static UITutorial Instance;
	
	public ButtonEffectLogic btnSkip, btnPlay;

	[SerializeField] private GameObject skipPop, finger, playTutorial;
	[SerializeField] private GameObject[] tutorialUpgrade;
	[SerializeField] private ButtonEffectLogic btnOk, btnCancel;
	
	[SerializeField] private ShowEffect _skipPop;

	private void Awake()
	{
		Instance = this;
		
		btnSkip.onClick.AddListener(ShowPopSkip);
		btnOk.onClick.AddListener(SkipTutorial);
		btnCancel.onClick.AddListener(ClosePopSkip);
		btnPlay.onClick.AddListener(PlayTutorial);
	}

	private void Start()
	{
		SetStep();
	}

	public void NextStep()
	{
		PrefData.StepTutorial++;
		SetStep();
	}

	public void SetStep()
	{
		switch (DataController.StepTutorial)
		{
			case 0: //Touch to play
				TouchToPlay();
				break;
			case 1:	//Touch SteeringWheel
				
				break;
			case 2: //Next Level
				
				break;
			case 3: //Upgrade Fuel
				UpgradeTutorial(0);
				break;
			case 4: //Upgrade Grip
				UpgradeTutorial(1);
				break;
			case 5: //Upgrade Bonus
				UpgradeTutorial(2);
				break;
			case 6: //Play to game (Tutorial Completed)
				TutorialCompleted();
				break;
		}
	}


	#region Tutorial Step

	public void TouchToPlay()
	{
		playTutorial.SetActive(true);
		SetFingerPos(new Vector3(0, -200, 0));
	}

	private void UpgradeTutorial(int type)
	{
		switch (type)
		{
			case 0:
				tutorialUpgrade[0].SetActive(true);
				break;
			case 1:
				tutorialUpgrade[0].SetActive(false);
				tutorialUpgrade[1].SetActive(true);
				break;
			case 2:
				tutorialUpgrade[1].SetActive(false);
				tutorialUpgrade[2].SetActive(true);
				break;
		}
		
		finger.transform.SetParent(tutorialUpgrade[type].transform.parent);
		SetFingerPos(tutorialUpgrade[type].transform.localPosition, 0.3f);
	}

	private void TutorialCompleted()
	{
		tutorialUpgrade[2].SetActive(false);
		PrefData.IncompleteTutorial = false;
		PrefData.StepTutorial++;
		finger.gameObject.SetActive(false);
		finger.transform.SetParent(transform);
		gameObject.SetActive(false);
	}
	
	
	#endregion

	private void ShowPopSkip()
	{
		skipPop.SetActive(true);
		_skipPop.Effect();
	}

	private void ClosePopSkip()
	{
		_skipPop.Hide();
		DOVirtual.DelayedCall(0.25f, () => skipPop.SetActive(false));
	}

	private void SkipTutorial()
	{
		
	}

	private void PlayTutorial()
	{
		if(finger.activeSelf) finger.SetActive(false);
		NextStep();
		playTutorial.SetActive(false);
		RaceManager.Instance.StartRace();
		UIController.Instance.uIGamePlay.Show();
	}

	public void SetFingerPos(Vector3 pos, float delay = 0f)
	{
		DOVirtual.DelayedCall(delay, () =>
			{
				if (!finger.activeSelf) finger.SetActive(true);
			}
		);
		finger.transform.localPosition = pos;
	}

	public void SetFingerPos(Vector3 pos, Transform parentT)
	{
		if(!finger.activeSelf) finger.SetActive(true);
		finger.transform.SetParent(parentT);
		finger.transform.localPosition = pos;
	}

	public void HideFinger()
	{
		if(finger.activeSelf) finger.SetActive(false);
	}
	
}
