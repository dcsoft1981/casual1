using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEditor;
using System;
using static UnityEngine.GraphicsBuffer;
using static DG.Tweening.DOTweenAnimation;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
	public bool isGameOver = false;
	public TargetCircle targetCircle; // 대상 TargetCircle
	//private LevelData levelData;
	private LevelDBEntity levelData;

	[SerializeField] private TextMeshProUGUI textGoal;
    [SerializeField] private TextMeshProUGUI textLevel;

	private int hp;
	private int targetId;
	private int targetScale;
	private Color targetColor;
	private float pinScale;


	[SerializeField] private GameObject btnRetry;
	[SerializeField] private GameObject labelClear;
	[SerializeField] private GameObject labelFailure;
	[SerializeField] private LevelDB levelDB;

	[SerializeField] private Color green;
	[SerializeField] private Color red;

	// Start is called once before the first execution of Update after the MonoBehaviour is created

	private void Awake()
	{
        if (instance == null)
        {
            instance = this;
        }
	}

	void Start()
    {
		int level = LocalDataManager.instance.GetCurLevel();
		//levelData = gameDataManager.GetLevelData(level);
		levelData = levelDB.levels[level-1];
		hp = levelData.hp;

		// Target SIZE 지정
		targetScale = levelData.target % 100;
		if (targetScale == 0)
			targetScale = 100;
		targetId = levelData.target - targetScale;
		float curTargetScale = Define.TARGET_BASE_SCALE * targetScale / 100f;
		targetCircle.transform.localScale = new Vector3(curTargetScale, curTargetScale, curTargetScale);

		// Target 색상 지정
		targetColor = GetTargetColor(targetId);
		SpriteRenderer objectRenderer = targetCircle.GetComponent<SpriteRenderer>();
		objectRenderer.color = targetColor;

		// Pin SIZE 지정
		pinScale = 1f;


		CreateAndPlayAnimation();
		Debug.Log("levelData : " + level + " - " + levelData.id + " - " + levelData.hp);
		Vector3 screenPosition = Camera.main.WorldToScreenPoint(targetCircle.transform.position);
		textGoal.transform.position = screenPosition;

		SetGoalText();
        SetLevelText();
		AudioManager.instance.OffEffectBgm();
		AudioManager.instance.PlayBgm();
	}

    void SetGoalText()
    {
		textGoal.SetText(hp.ToString());
	}

	void SetLevelText()
	{
        int level = LocalDataManager.instance.GetCurLevel();
		textLevel.SetText(level.ToString());
	}

	public void DecreaseGoal()
    {
        hp--;
        SetGoalText();

        if(hp <= 0)
        {
            SetGameOver(true);
        }
	}

    public void SetGameOver(bool success)
    {
        if(isGameOver == false)
        {
            isGameOver = true;
            if(success)
            {
                Camera.main.backgroundColor = green;
				AudioManager.instance.OnEffectBgm();
				Invoke("StageClear", 0.3f);
			}
            else
            {
				Camera.main.backgroundColor = red;
				Invoke("StageFailure", 0.7f);
			}
		}
    }

	private  void StageClear()
    {
		int level = LocalDataManager.instance.GetCurLevel();
        int nextLevel = level + 1;
		LocalDataManager.instance.SetCurLevel(nextLevel);
		TextMeshProUGUI buttonText = btnRetry.GetComponentInChildren<TextMeshProUGUI>();
		if(buttonText != null)
			buttonText.text = Define.LOCALE_NEXT;
		btnRetry.SetActive(true);
        labelClear.SetActive(true);
		AudioManager.instance.PlaySfx(AudioManager.Sfx.clear);
	}

	private void StageFailure()
	{
		/*
		TextMeshProUGUI buttonText = btnRetry.GetComponentInChildren<TextMeshProUGUI>();
		if (buttonText != null)
			buttonText.text = Define.LOCALE_RETRY;
		btnRetry.SetActive(true);
        labelFailure.SetActive(true);
		*/
		SceneManager.LoadScene("LobbyScene");
	}

	public void Retry()
    {
        SceneManager.LoadScene("SampleScene");
    }

	public void CreateAndPlayAnimation()
	{
		if (targetCircle == null || levelData == null)
		{
			Debug.LogError("TargetCircle or LevelData is not assigned.");
			return;
		}

		// AnimationClip 생성
		AnimationClip clip = new AnimationClip
		{
			legacy = false // Mecanim 호환을 위해 false로 설정
		};

		// AnimationCurve 생성
		/*
		Keyframe[] keyframes = new Keyframe[levelData.datas.Count];
		for (int i = 0; i < Define.MAX_LEVEL_KEYFRAME_COUNT; i++)
		{
			var data = levelData.datas[i];
			keyframes[i] = new Keyframe(data.time, data.rotate);
		}
		*/
		Keyframe[] keyframes = new Keyframe[1]; // 이후 회전 값 복수개 사용시 증가 가능
		keyframes[0] = new Keyframe(Define.ROTATE_SEC, levelData.rotation);
		//keyframes[1] = new Keyframe(levelData.time1, levelData.rotation1);
		//keyframes[2] = new Keyframe(levelData.time2, levelData.rotation2);
		//keyframes[3] = new Keyframe(levelData.time3, levelData.rotation3);
		//keyframes[4] = new Keyframe(levelData.time4, levelData.rotation4);
		// 모든 키프레임의 보간 방식을 선형으로 설정
		for (int i = 0; i < keyframes.Length; i++)
		{
			keyframes[i].inTangent = 0;
			keyframes[i].outTangent = 0;
		}

		AnimationCurve curve = new AnimationCurve(keyframes);
		curve.postWrapMode = WrapMode.Loop; // 애니메이션 곡선 반복 설정

		AnimationCurve speedCurve = new AnimationCurve(
			new Keyframe(0, 180),   // 0초에 180도/초
			new Keyframe(2, 360),  // 2초에 360도/초
			new Keyframe(3, 90)    // 3초에 90도/초
		);
		speedCurve.postWrapMode = WrapMode.Loop;

		targetCircle.StartAnimation(speedCurve);
		targetCircle.SetLevelDB(levelData);
		/*
		try 
		{
			clip.SetCurve("", typeof(TargetCircle), "m_RotateSpeed", curve);
			//clip.SetCurve("", typeof(Transform), "localEulerAngles.z", curve);
		} 
		catch ( Exception e )
		{
			Debug.LogError(e.ToString());
			Debug.LogError(e.Message);
		}
		
		clip.wrapMode = WrapMode.Loop;
		EnableLooping(clip);

		// Animator에 AnimationClip 할당 및 실행
		Animator animator = targetCircle.GetComponent<Animator>();
		if (animator == null)
		{
			animator = targetCircle.gameObject.AddComponent<Animator>();
		}

		RuntimeAnimatorController controller = new AnimatorOverrideController
		{
			runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("DefaultController")
		};
		animator.runtimeAnimatorController = controller;

		AnimatorOverrideController overrideController = (AnimatorOverrideController)animator.runtimeAnimatorController;
		overrideController["Rotation"] = clip;

		animator.Play("Rotation");
		*/
	}

	void EnableLooping(AnimationClip clip)
	{
#if UNITY_EDITOR
		if (clip != null)
		{
			// AnimationClipSettings를 사용해 루프 설정
			AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);
			settings.loopTime = true;
			AnimationUtility.SetAnimationClipSettings(clip, settings);
		}
#endif
	}

	public Color GetTargetColor(int targetId)
	{
		if (targetId == (int)Define.TargetType.BLACK)
			return Color.black;
		else if (targetId == (int)Define.TargetType.BLUE)
			return Color.blue;
		else if (targetId == (int)Define.TargetType.RED)
			return Color.red;
		else
			return Color.black;
	}
}
