using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEditor;
using System;
using static UnityEngine.GraphicsBuffer;
using static DG.Tweening.DOTweenAnimation;
using static Define;
using Unity.VisualScripting;
using System.Collections.Generic;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
	public bool isGameOver = false;
	public TargetCircle targetCircle; // 대상 TargetCircle
	public PinLauncher pinLauncher; // 대상 PinLauncher
									 
	private LevelDBEntity levelData;

	[SerializeField] private TextMeshProUGUI textHP;
    [SerializeField] private TextMeshProUGUI textLevel;
	[SerializeField] private TextMeshProUGUI textShot;

	private int hp;
	private int targetId;
	private int targetScale;
	private Color targetColor;
	private float pinScale;
	private int shot;
	private int cheatRotation = 0;
	private int rotationBuff = 0;

	private Dictionary<int, GimmickDBEntity> dic_gimmicks;
	private Dictionary<int, Sprite> dic_gimmickSprites;
	private Dictionary<int, Sprite> dic_gimmickSprites2;
	private Dictionary<int, Sprite> dic_gimmickSprites3;

	private List<GameObject> listGimmick1;
	private List<GameObject> listGimmick2;
	private List<GameObject> listGimmick3;
	private List<GameObject> listGimmick4;
	private List<GameObject> listGimmick5;

	[SerializeField] private GameObject btnRetry;
	[SerializeField] private GameObject labelClear;
	[SerializeField] private GameObject labelFailure;
	[SerializeField] private LevelDB levelDB;
	[SerializeField] private GimmickDB gimmickDB;

	[SerializeField] private Color green;
	[SerializeField] private Color red;

	[SerializeField] private GameObject gimmickObject;

	// Start is called once before the first execution of Update after the MonoBehaviour is created

	private void Awake()
	{
        if (instance == null)
        {
            instance = this;
			dic_gimmicks = new Dictionary<int, GimmickDBEntity>();
			dic_gimmickSprites = new Dictionary<int, Sprite>();
			dic_gimmickSprites2 = new Dictionary<int, Sprite>();
			dic_gimmickSprites3 = new Dictionary<int, Sprite>();
			foreach (GimmickDBEntity gimmickDbEntity in gimmickDB.gimmicks)
			{
				dic_gimmicks.Add(gimmickDbEntity.id, gimmickDbEntity);
				if(gimmickDbEntity.sprite.Length > 0)
					dic_gimmickSprites.Add(gimmickDbEntity.id, Resources.Load<Sprite>(gimmickDbEntity.sprite));
				if (gimmickDbEntity.sprite2.Length > 0)
					dic_gimmickSprites2.Add(gimmickDbEntity.id, Resources.Load<Sprite>(gimmickDbEntity.sprite2));
				if (gimmickDbEntity.sprite3.Length > 0)
					dic_gimmickSprites3.Add(gimmickDbEntity.id, Resources.Load<Sprite>(gimmickDbEntity.sprite3));
			}
			listGimmick1 = new List<GameObject>();
			listGimmick2 = new List<GameObject>();
			listGimmick3 = new List<GameObject>();
			listGimmick4 = new List<GameObject>();
			listGimmick5 = new List<GameObject>();
		}
	}

	void Start()
    {
		int level = LocalDataManager.instance.GetCurLevel();
		//levelData = gameDataManager.GetLevelData(level);
		levelData = levelDB.levels[level-1];

		// HP 지정
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

		// Shot 지정
		shot = levelData.shot;


		CreateAndPlayAnimation();
		Debug.Log("levelData : " + level + " - " + levelData.id + " - " + levelData.hp);
		Vector3 targetScreenPosition = Camera.main.WorldToScreenPoint(targetCircle.transform.position);
		textHP.transform.position = targetScreenPosition;

		Vector3 texpHPPostion = pinLauncher.transform.position + new Vector3(0.6f, 0f, 0f);
		Vector3 pinLauncherScreenPosition = Camera.main.WorldToScreenPoint(texpHPPostion);
		textShot.transform.position = pinLauncherScreenPosition;

		SetHPText();
        SetLevelText();
		SetShotText();
		PrepareGimmickAll();
		AudioManager.instance.OffEffectBgm();
		AudioManager.instance.PlayBgm();
	}

    void SetHPText()
    {
		textHP.SetText(hp.ToString());
	}

	void SetLevelText()
	{
        int level = LocalDataManager.instance.GetCurLevel();
		textLevel.SetText(level.ToString());
	}

	void SetShotText()
	{
		textShot.SetText("x" + shot.ToString());
	}

	public void DecreaseHP()
    {
        hp--;
		SetHPText();

        if(hp <= 0)
        {
            SetGameOver(true);
        }
	}

	public void DecreaseShot()
	{
		shot--;
		SetShotText();
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
        SceneManager.LoadScene("IngameScene");
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

	public void CheckFailure()
	{
		// shot == 0 이면 실패
		if (shot == 0)
			SetGameOver(false);
	}

	public void SetCheatRotation(int rotation)
	{
		cheatRotation = rotation;
	}

	public void AddShot(int changeShot)
	{
		shot += changeShot;
		SetShotText();
	}

	public int GetCheatRotation()
	{
		return cheatRotation;
	}

	void PrepareGimmickAll()
	{
		// 기믹 정보를 보며 기믹 생성
		PrepareGimmick(levelData.gimmick1, listGimmick1);
		PrepareGimmick(levelData.gimmick2, listGimmick2);
		PrepareGimmick(levelData.gimmick3, listGimmick3);
		PrepareGimmick(levelData.gimmick4, listGimmick4);
		PrepareGimmick(levelData.gimmick5, listGimmick5);
	}

	void PrepareGimmick(string gimmick, List<GameObject> listGimmick)
	{
		if (gimmick.Length == 0)
			return;

		string[] strInfo = gimmick.Split(":");
		int[] numInfo = new int[strInfo.Length];
		for (int i = 0; i < numInfo.Length; i++)
		{
			numInfo[i] = int.Parse(strInfo[i]);
		}

		int gimmickValue = numInfo[0];
		int hp = gimmickValue % 100;

		GimmickType gimmickType = (GimmickType)(gimmickValue - hp);
		GimmickDBEntity gimmickInfo = GetGimmickInfo(gimmickType);

		for (int i = 1; i < numInfo.Length; i++)
		{
			int angle = numInfo[i];
			Vector3 gimmickPos = GetGimmickPos(angle, gimmickInfo.distance);
			CreateGimmick(gimmickType, hp, gimmickPos, angle, listGimmick);
		}
	}

	public Color GetGimmickColor(Gimmick gameObjectGimmick)
	{
		return GetGimmickColor(gameObjectGimmick.gimmickType, gameObjectGimmick.hp, gameObjectGimmick.GetChecked());
	}

	public Color GetGimmickColor(GimmickType type, int hp, bool isChecked)
	{
		switch (type)
		{
			case GimmickType.SUPER_SHIELD:
				return Color.gray;
			case GimmickType.TARGET_RECOVER:
				return Color.green;
			case GimmickType.ROTATION_DOWN:
				return new Color(0f, 1f, 0f);
			case GimmickType.ROTATION_UP:
				return new Color(1f, 0f, 0f);
			case GimmickType.ADD_SHOT:
				return new Color(0.0f, 0f, 0.9f);
			case GimmickType.SEQUENCE:
				{
					if (!isChecked)
						return new Color(0f, 0f, 0f);
					else
						return Color.cyan;
				}
		}

		if (hp > 3)
			return new Color(0.9f, 0f, 0f);
		if (hp == 3)
			return new Color(0.7f, 0f, 0f);
		else if (hp == 2)
			return new Color(0.4f, 0f, 0f);
		else if (hp == 1)
			return new Color(0.2f, 0f, 0f);

		return Color.black;
	}

	public Vector3 GetGimmickPos(int angle, float gimmickDistance)
	{
		float radians = angle * Mathf.Deg2Rad;
		float curTargetScale = Define.TARGET_BASE_SCALE * targetScale / 100f;
		float distance = curTargetScale/2f + gimmickDistance;  // 타겟으로부터 떨어진 거리

		// 새 위치 계산
		Vector3 targetPosition = targetCircle.transform.position; // 타겟 위치
		Vector3 newPosition = new Vector3(
			targetPosition.x + distance * Mathf.Cos(radians),
			targetPosition.y + distance * Mathf.Sin(radians),
			targetPosition.z // 2D 게임이므로 Z값은 유지
		);
		return newPosition;
	}

	public void CreateGimmick(GimmickType gimmickType, int hp, Vector3 gimmickPos, int angle, List<GameObject> listGimmick)
	{
		GameObject gimmickGameObject = Instantiate(gimmickObject, gimmickPos, Quaternion.identity);
		gimmickGameObject.transform.SetParent(targetCircle.transform);
		Gimmick gameObjectGimmick = gimmickGameObject.GetComponent<Gimmick>();
		bool isChecked = GetGimmickInitChecked(gimmickType, listGimmick);
		Color color = GetGimmickColor(gimmickType, hp, isChecked);
		gameObjectGimmick.SetGimmick(gimmickType, hp, color, angle, listGimmick, isChecked);
		listGimmick.Add(gimmickGameObject);
	}

	public bool GetGimmickInitChecked(GimmickType gimmickType, List<GameObject> listGimmick)
	{
		bool result = false;
		switch (gimmickType)
		{
			// 순서
			case GimmickType.SEQUENCE:
				{
					if (listGimmick != null && listGimmick.Count == 0)
						result = true;
				}
				break;
		}
		return result;
	}

	private void GimmickHpMinusWork(GameObject gameObject)
	{
		GimmickHpMinusWork(gameObject, gameObject.GetComponent<Gimmick>());
	}

	private void GimmickHpMinusWork(GameObject gameObject, Gimmick gameObjectGimmick)
	{
		if (gameObjectGimmick.hp <= 0)
			return;
		gameObjectGimmick.hp--;
		gameObjectGimmick.SetColor(GetGimmickColor(gameObjectGimmick));
		if (gameObjectGimmick.hp <= 0)
		{
			// 기믹 제거
			gameObject.transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.InBack).OnComplete(() => Destroy(gameObject));
		}
	}

	private void GimmickListWork(GameObject gameObject, Gimmick gameObjectGimmick)
	{
		List<GameObject> gimmickList = gameObjectGimmick.GetListGimmick();
		if (gimmickList == null)
			return;

		switch (gameObjectGimmick.gimmickType)
		{
			// 순서
			case GimmickType.SEQUENCE:
				{
					// 타겟 체크 안되어 있으면 무 반응
					if (!gameObjectGimmick.GetChecked())
						return;
					// 스프라이트 교체
					gameObjectGimmick.SetUnChecked();
					gameObjectGimmick.SetColor(GetGimmickColor(gameObjectGimmick));
					gameObjectGimmick.SetGimmickSprite(2);
					
					Gimmick nextGameObjectGimmick = GetNextGameObject(gimmickList, gameObject);
					if(nextGameObjectGimmick != null)
					{
						// 다음 오브젝트 활성화
						nextGameObjectGimmick.SetChecked();
						nextGameObjectGimmick.SetColor(GetGimmickColor(nextGameObjectGimmick));
					}
					else
					{
						// 해당 리스트의 기믹들 모두 체크 제거
						RemoveAllGimmicks(gimmickList);
					}


				}
				break;
		}
	}

	public bool GimmickHitWork(GameObject gameObject)
	{
		bool destroyPin = true;
		Gimmick gameObjectGimmick = gameObject.GetComponent<Gimmick>();
		GimmickDBEntity gimmickInfo = GetGimmickInfo(gameObjectGimmick.gimmickType);

		switch (gameObjectGimmick.gimmickType)
		{
			// 기믹 히트형
			case GimmickType.SHIELD:
				{
					GimmickHpMinusWork(gameObject, gameObjectGimmick);
				}
				break;

			case GimmickType.TARGET_RECOVER:
				{
					hp += gimmickInfo.value1;
					SetHPText();
					GimmickHpMinusWork(gameObject, gameObjectGimmick);
				}
				break;
			case GimmickType.DAMAGE_N:
				{
					hp -= gimmickInfo.value1;
					SetHPText();
					GimmickHpMinusWork(gameObject, gameObjectGimmick);
				}
				break;
			case GimmickType.SEQUENCE:
				{
					GimmickListWork(gameObject, gameObjectGimmick);
				}
				break;

			// 타겟 히트형 destroyPin = false;
			case GimmickType.ROTATION_UP:
				{
					destroyPin = false;
					rotationBuff++;
					GimmickHpMinusWork(gameObject, gameObjectGimmick);
				}
				break;
			case GimmickType.ROTATION_DOWN:
				{
					destroyPin = false;
					rotationBuff--;
					GimmickHpMinusWork(gameObject, gameObjectGimmick);
				}
				break;
			case GimmickType.ADD_SHOT:
				{
					destroyPin = false;
					shot += gimmickInfo.value1;
					SetShotText();
					GimmickHpMinusWork(gameObject, gameObjectGimmick);
				}
				break;
		}
		return destroyPin;
	}

	public float GetRotationValue(float rotation)
	{
		if(rotationBuff == 0)
		{
			return rotation;
		}

		float addValue = GetGimmickInfo(GimmickType.ROTATION_UP).value1 * rotationBuff;
		if(rotation > 0)
		{
			float resultValue = rotation + addValue;
			if (resultValue < 10f)
				resultValue = 10f;
			return resultValue;
		}
		else
		{
			float resultValue = rotation - addValue;
			if (resultValue > -10f)
				resultValue = -10f;
			return resultValue;
		}
	}

	public GimmickDBEntity GetGimmickInfo(GimmickType type)
	{
		if (dic_gimmicks.TryGetValue((int)type, out GimmickDBEntity gimmick))
			return gimmick;
		return null;
	}

	public Sprite GetGimmickSprite(GimmickType type)
	{
		if (dic_gimmickSprites.TryGetValue((int)type, out Sprite sprite))
			return sprite;
		return null;
	}

	public Sprite GetGimmickSprite2(GimmickType type)
	{
		if (dic_gimmickSprites2.TryGetValue((int)type, out Sprite sprite))
			return sprite;
		return null;
	}

	public Sprite GetGimmickSprite3(GimmickType type)
	{
		if (dic_gimmickSprites3.TryGetValue((int)type, out Sprite sprite))
			return sprite;
		return null;
	}

	public Gimmick GetNextGameObject(List<GameObject> gimmickList, GameObject gameObject)
	{
		bool found = false;
		foreach(GameObject go in gimmickList)
		{
			if (found)
			{
				return go.GetComponent<Gimmick>();
			}

			if(go == gameObject) found = true;
		}
		return null;
	}

	public void RemoveAllGimmicks(List<GameObject> list)
	{
		foreach(GameObject go in list)
		{
			GimmickHpMinusWork(go);
		}
	}
}
