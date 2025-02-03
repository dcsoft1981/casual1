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
using Unity.Burst.Intrinsics;
using System.Collections;
using Febucci.UI;
using DG.Tweening.Core.Easing;
using VibrationUtility;
using static UnityEngine.EventSystems.EventTrigger;
using System.Text;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using static System.Net.Mime.MediaTypeNames;
using System.Linq.Expressions;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
	public bool isGameOver = false;
	public TargetCircle targetCircle; // ��� TargetCircle
	public PinLauncher pinLauncher; // ��� PinLauncher
									 
	private LevelDBEntity levelData;

	[SerializeField] private TextMeshProUGUI textHP;
	[SerializeField] private TextMeshProUGUI textTapTok;
	[SerializeField] private TextMeshProUGUI textLevel;
	[SerializeField] private TextMeshProUGUI textShot;
	[SerializeField] private TextMeshProUGUI textCombo;
	[SerializeField] private TextMeshProUGUI textSkill;
	[SerializeField] private TextMeshProUGUI textStageLevel;
	[SerializeField] private TextMeshProUGUI textLevelPlayData;

	private int maxHp = 100;
	private int hp;
	private int targetId;
	private int targetScale;	
	private int shot;
	private int cheatRotation = 0;
	private int rotationBuff = 0;
	private int combo = 0;

	private List<GameObject> listGimmick1;
	private List<GameObject> listGimmick2;
	private List<GameObject> listGimmick3;
	private List<GameObject> listGimmick4;
	private List<GameObject> listGimmick5;
	private List<GameObject> listGimmick6;
	private List<GameObject> listGimmick7;

	[SerializeField] private Color failureColor;

	[SerializeField] private GameObject gimmickObject;

	private bool inShield = false;
	private GameObject curHitGimmick = null;
	private List<GameObject> listPinnedShot = null;

	private Dictionary<int, int> dic_doubleDamage;
	private Dictionary<int, int> dic_noDamage;
	private bool shotAddDamage = false;
	private Pin currPin = null;
	private bool checkFinalShot = false;
	private int skillPosY = 0;
	private IngameType ingameType;

	[SerializeField] private GameObject popupResult;

	[SerializeField] private float pinPositionY100;
	[SerializeField] private float pinPositionY80;
	[SerializeField] private float pinPositionY50;
	[SerializeField] private float hpPositionY100;
	[SerializeField] private float hpPositionY80;
	[SerializeField] private float hpPositionY50;

	[SerializeField] private GameObject ComboEffect;
	[SerializeField] private GameObject ComboEffects;

	private int nextPinId = 0;
	private Dictionary<int, List<GameObject>> dic_AngleGimmicks;
	private Dictionary<GameObject, GameObject> dic_PairGimmick;
	private int lastGrade = 0;
	[SerializeField] private GameObject gradeUp;
	[SerializeField] private TextMeshProUGUI clearText;
	[SerializeField] private ParticleSystem effectPrab;
	[SerializeField] private Transform effectGroup;

	[SerializeField] private GameObject buttonTab;
	private TextMeshProUGUI buttonTabText;
	public bool tutorialButtonTab = false;

	// Start is called once before the first execution of Update after the MonoBehaviour is created

	private void Awake()
	{
        if (instance == null)
        {
            instance = this;
			listGimmick1 = new List<GameObject>();
			listGimmick2 = new List<GameObject>();
			listGimmick3 = new List<GameObject>();
			listGimmick4 = new List<GameObject>();
			listGimmick5 = new List<GameObject>();
			listGimmick6 = new List<GameObject>();
			listGimmick7 = new List<GameObject>();

			listPinnedShot = new List<GameObject>();
			dic_AngleGimmicks = new Dictionary<int, List<GameObject>>();
			dic_PairGimmick = new Dictionary<GameObject, GameObject>();
			dic_doubleDamage = new Dictionary<int, int>();
			dic_noDamage = new Dictionary<int, int>();
			VibrationUtil.Init();
		}
	}

	void Start()
    {
		int level = 0;
		LocalDataManager.instance.CheckInfinityStage();
		ingameType = LocalDataManager.instance.GetIngameType();
		lastGrade = LocalDataManager.instance.GetCurGrade();
		switch (ingameType)
		{
			case IngameType.NORMAL:
				{
					level = LocalDataManager.instance.GetCurLevel();
					if(level < 4)
					{
						tutorialButtonTab = true;
					}
				}
				break;
			case IngameType.INFINITY:
				{
					level = LocalDataManager.instance.GetInfinityStageLevel();
					SetStageText();
				}
				break;
		}

		levelData = LocalDataManager.instance.GetLevelDBEntity(level);
		combo = 0;
		SetOrthographicSize();

		// HP ����
		maxHp = levelData.hp;
		hp = maxHp;

		// Target SIZE ����
		targetScale = levelData.target % 100;
		if (targetScale == 0)
			targetScale = 100;
		float targetScaleRate = targetScale / 100f;
		targetId = levelData.target - targetScale;
		//float curTargetScale = Define.TARGET_BASE_SCALE;
		//targetCircle.transform.localScale = new Vector3(curTargetScale, curTargetScale, curTargetScale);

		// Target ����
		targetCircle.SetSprite(targetScaleRate, targetId);
		// ǥ�� �׸���
		targetCircle.DrawExpression(maxHp, hp);
		CircleCollider2D collider2D = targetCircle.GetComponent<CircleCollider2D>();
		collider2D.radius = collider2D.radius * targetScaleRate;

		// Shot ����
		shot = levelData.shot;

		CreateAndPlayAnimation();
		Debug.Log("levelData:" + level + " - " + levelData.id + " - " + levelData.hp);
		Vector3 targetScreenPosition = Camera.main.WorldToScreenPoint(targetCircle.transform.position);
		textHP.transform.position = Camera.main.WorldToScreenPoint(new Vector3(0f, GetTargetHPPosition(), 0f));
		textTapTok.transform.position = targetScreenPosition;

		Vector3 texpHPPostion = pinLauncher.transform.position + new Vector3(0.6f, 0f, 0f);
		Vector3 pinLauncherScreenPosition = Camera.main.WorldToScreenPoint(texpHPPostion);
		textShot.transform.position = pinLauncherScreenPosition;

		SetHPText();
        SetLevelText();
		SetShotText();
		PrepareGimmickAll();

		AudioManager.instance.TickTockStop();
		AudioManager.instance.OffEffectBgm();
		AudioManager.instance.PlayBgm();

		// �÷��� ��� ����
		LocalDataManager.instance.StartLevelPlayData();
		InvokeRepeating("CheckStageFailure", 0f, 0.3f);

		if(tutorialButtonTab)
		{
			buttonTab.SetActive(true);
			buttonTabText = buttonTab.GetComponentInChildren<TextMeshProUGUI>();
			buttonTabText.DOFade(0f, 1.3f).SetLoops(-1, LoopType.Yoyo);
		}
	}

    void SetHPText()
    {
		textHP.SetText(hp.ToString());
	}

	void SetLevelText()
	{
		int value = 0;
		switch(ingameType)
		{
			case IngameType.NORMAL:
				{
					value = LocalDataManager.instance.GetCurLevel();
				}
				break;
			case IngameType.INFINITY:
				{
					value = LocalDataManager.instance.GetInfinityStage();
				}
				break;
		}
		textLevel.SetText(value.ToString());
	}

	void SetStageText()
	{
		textStageLevel.SetText("Stage");
	}

	void SetShotText()
	{
		textShot.SetText("x" + shot.ToString());
	}

	void SetComboText()
	{
		textCombo.SetText(combo.ToString()+ "/5");
	}

	public void IncreaseHP(int amount)
	{
		hp += amount;
		SetHPText();
	}

	public void DecreaseHP(int damage)
    {
		if(hp > 0)
		{
			hp -= damage;
			targetCircle.GetColorByTargetHp(maxHp, hp);
			if (damage == 2)
				Vibrate2();
			else
				Vibrate1();
			StartCoroutine(Shake(targetCircle.gameObject, ShakeType.NORMAL));
		}
		if (hp < 0)
			hp = 0;
		SetHPText();

        if(hp <= 0)
        {
            SetGameOver(true);
        }
	}

	public void DecreaseShot()
	{
		shot--;
		if(shot < 0)
			shot = 0;
		SetShotText();
		CheckFinalShot();
	}

	public int GetShotCount()
	{
		return shot;
	}

	public void SetGameOver(bool success)
    {
        if(isGameOver == false)
        {
			ClearGuideLines();
			isGameOver = true;
            if(success)
            {
                Camera.main.backgroundColor = LocalDataManager.instance.GetCurColor();
				AudioManager.instance.OnEffectBgm();
				Invoke("StageClear", 0.3f);
			}
            else
            {
				Camera.main.backgroundColor = failureColor;
				Invoke("StageFailure", 0.7f);
			}
		}
    }

	public void ClearGuideLines()
	{
		LineRenderer[] array = targetCircle.gameObject.GetComponentsInChildren<LineRenderer>();
		for (int i = 0; i < array.Length; i++)
		{
			//if(array[i].positionCount < 10)
			array[i].enabled = false;
		}
	}

	private  void StageClear()
    {
		textHP.gameObject.SetActive(false);
		textShot.gameObject.SetActive(false);
		targetCircle.ClearExpressionLines();
		DestroyAllGimmicks();
		DestroyAllShots();
		Invoke("TargetEffect", 0.5f);
		//targetCircle.GRADIENT_ON();
		switch (ingameType)
		{
			case IngameType.NORMAL:
				{
					int level = LocalDataManager.instance.GetCurLevel();
					if(level == 10)
					{
						// ����� Ȱ��ȭ
						LocalDataManager.instance.SetReddotPlayer(true);
						LocalDataManager.instance.SetReddotMenu(true);
					}
					int nextLevel = level + 1;
					LocalDataManager.instance.SetCurLevel(nextLevel);
				}
				break;
			case IngameType.INFINITY:
				{
					// ���ѽ��������� ����
					int stage = LocalDataManager.instance.GetInfinityStage();
					int nextStage = stage + 1;
					LocalDataManager.instance.SetInfinityStage(nextStage);
				}
				break;
		}


		/*
		TextMeshProUGUI buttonText = btnRetry.GetComponentInChildren<TextMeshProUGUI>();
		if(buttonText != null)
			buttonText.text = Define.LOCALE_NEXT;
		btnRetry.SetActive(true);
        labelClear.SetActive(true);
		*/

		if(lastGrade != LocalDataManager.instance.GetCurGrade())
		{
			clearText.text = "New Tier achieved!";
			// ��� ����
			textLevelPlayData.text = "";
			GameObject gradeGameObject = popupResult.transform.Find("Grade").gameObject;
			GradeDBEntity entity = LocalDataManager.instance.GetCurGradeDBEntity();
			GameObject gradeInfoGameObject = LocalDataManager.instance.CreateGradeInfo(entity, gradeUp, gradeGameObject.transform);
			gradeInfoGameObject.transform.localPosition = Vector3.one;
			// Home ��ư Ȱ��ȭ
			popupResult.transform.Find("ButtonHome").gameObject.SetActive(true);
		}
		else
		{
			clearText.text = "Success!";
			// ��� ����
			textLevelPlayData.text = LocalDataManager.instance.GetLevelPlayDataText();
			// Next ��ư Ȱ��ȭ
			popupResult.transform.Find("ButtonNext").gameObject.SetActive(true);
		}
		AudioManager.instance.PlaySfx(AudioManager.Sfx.clear);
		Vibrate3();
		Invoke("PopupClearResult", 1f);
	}

	public void StageFailure()
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

		// AnimationClip ����
		AnimationClip clip = new AnimationClip
		{
			legacy = false // Mecanim ȣȯ�� ���� false�� ����
		};

		// AnimationCurve ����
		/*
		Keyframe[] keyframes = new Keyframe[levelData.datas.Count];
		for (int i = 0; i < Define.MAX_LEVEL_KEYFRAME_COUNT; i++)
		{
			var data = levelData.datas[i];
			keyframes[i] = new Keyframe(data.time, data.rotate);
		}
		*/
		
		Keyframe[] keyframes = new Keyframe[1]; // ���� ȸ�� �� ������ ���� ���� ����
		keyframes[0] = new Keyframe(Define.ROTATE_SEC, levelData.rotation);
		//keyframes[1] = new Keyframe(levelData.time1, levelData.rotation1);
		//keyframes[2] = new Keyframe(levelData.time2, levelData.rotation2);
		//keyframes[3] = new Keyframe(levelData.time3, levelData.rotation3);
		//keyframes[4] = new Keyframe(levelData.time4, levelData.rotation4);
		// ��� Ű�������� ���� ����� �������� ����
		for (int i = 0; i < keyframes.Length; i++)
		{
			keyframes[i].inTangent = 0;
			keyframes[i].outTangent = 0;
		}

		AnimationCurve curve = new AnimationCurve(keyframes);
		curve.postWrapMode = WrapMode.Loop; // �ִϸ��̼� � �ݺ� ����

		AnimationCurve speedCurve = new AnimationCurve(
			new Keyframe(0, 180),   // 0�ʿ� 180��/��
			new Keyframe(2, 360),  // 2�ʿ� 360��/��
			new Keyframe(3, 90)    // 3�ʿ� 90��/��
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

		// Animator�� AnimationClip �Ҵ� �� ����
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
			// AnimationClipSettings�� ����� ���� ����
			AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);
			settings.loopTime = true;
			AnimationUtility.SetAnimationClipSettings(clip, settings);
		}
#endif
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
		// ��� ������ ���� ��� ����
		PrepareGimmick(levelData.gimmick1, listGimmick1);
		PrepareGimmick(levelData.gimmick2, listGimmick2);
		PrepareGimmick(levelData.gimmick3, listGimmick3);
		PrepareGimmick(levelData.gimmick4, listGimmick4);
		PrepareGimmick(levelData.gimmick5, listGimmick5);
		PrepareGimmick(levelData.gimmick6, listGimmick6);
		PrepareGimmick(levelData.gimmick7, listGimmick7);
		// ����� ����
		SetPairGimmick();
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
		GimmickDBEntity gimmickInfo = LocalDataManager.instance.GetGimmickInfo(gimmickType);

		if(gimmickType == GimmickType.DAMAGE_AREA)
		{
			// ������ ���� ����
			if(numInfo[2] >= 0)
			{
				// 1 line
				for( int i = numInfo[2]; i <= numInfo[3]; i++)
				{
					dic_doubleDamage.Add(i,1); ;
				}
			}
			else
			{
				// 2 line
				for (int i = 0 ; i <= numInfo[3]; i++)
				{
					dic_doubleDamage.Add(i, 1); ;
				}
				for (int i = (360+numInfo[2]); i <= 359; i++)
				{
					dic_doubleDamage.Add(i, 1); ;
				}
			}
			Debug.Log($"DAMAGE_AREA :  {targetScale}, {numInfo[2]}, {numInfo[3]}");
			targetCircle.DrawDamageLine(numInfo[2], numInfo[3]);
		}
		else if (gimmickType == GimmickType.NODAMAGE_AREA)
		{
			// �뵥���� ���� ����
			if (numInfo[2] >= 0)
			{
				// 1 line
				for (int i = numInfo[2]; i <= numInfo[3]; i++)
				{
					dic_noDamage.Add(i, 1); ;
				}
			}
			else
			{
				// 2 line
				for (int i = 0; i <= numInfo[3]; i++)
				{
					dic_noDamage.Add(i, 1); ;
				}
				for (int i = (360 + numInfo[2]); i <= 359; i++)
				{
					dic_noDamage.Add(i, 1); ;
				}
			}
			Debug.Log($"NODAMAGE_AREA :  {targetScale}, {numInfo[2]}, {numInfo[3]}");
			targetCircle.DrawNoDamageLine(numInfo[2], numInfo[3]);
		}
		else
		{
			// ��� ����
			for (int i = 1; i < numInfo.Length; i++)
			{
				int angle = numInfo[i];
				Vector3 gimmickPos = GetGimmickPos(angle, gimmickInfo.distance);
				CreateGimmick(gimmickType, hp, gimmickPos, angle, listGimmick);
			}
		}
	}

	public Color GetGimmickColor(Gimmick gameObjectGimmick)
	{
		return LocalDataManager.instance.GetGimmickColor(gameObjectGimmick.gimmickType, gameObjectGimmick.hp, gameObjectGimmick.GetChecked());
	}

	public Vector3 GetGimmickPos(int angle, float gimmickDistance)
	{
		float radians = angle * Mathf.Deg2Rad;
		float curTargetScale = Define.TARGET_BASE_SCALE * targetScale / 100f;
		float distance = curTargetScale/2f + gimmickDistance;  // Ÿ�����κ��� ������ �Ÿ�

		// �� ��ġ ���
		Vector3 targetPosition = targetCircle.transform.position; // Ÿ�� ��ġ
		Vector3 newPosition = new Vector3(
			targetPosition.x + distance * Mathf.Cos(radians),
			targetPosition.y + distance * Mathf.Sin(radians),
			targetPosition.z // 2D �����̹Ƿ� Z���� ����
		);
		return newPosition;
	}

	public void CreateGimmick(GimmickType gimmickType, int hp, Vector3 gimmickPos, int angle, List<GameObject> listGimmick)
	{
		// ��� ����
		GameObject gimmickGameObject = Instantiate(gimmickObject, gimmickPos, Quaternion.identity);
		gimmickGameObject.transform.SetParent(targetCircle.transform);
		Gimmick gameObjectGimmick = gimmickGameObject.GetComponent<Gimmick>();
		SetTargetStateByGimmickInit(gimmickType);
		bool isChecked = GetGimmickInitChecked(gimmickType, listGimmick);
		Color color = LocalDataManager.instance.GetGimmickColor(gimmickType, hp, isChecked);
		gameObjectGimmick.SetGimmick(gimmickType, hp, color, angle, listGimmick, isChecked, targetCircle.gameObject);
		gameObjectGimmick.effect = Instantiate(effectPrab, effectGroup);// ����Ʈ ����
		listGimmick.Add(gimmickGameObject);
		AddAngleGimmick(angle, gimmickGameObject);
	}

	public void SetTargetStateByGimmickInit(GimmickType gimmickType)
	{
		switch (gimmickType)
		{
			case GimmickType.KEY_CHAIN:
				{
					inShield = true;
					targetCircle.ShieldColorON();
				}
				break;
		}
	}

	public bool GetGimmickInitChecked(GimmickType gimmickType, List<GameObject> listGimmick)
	{
		bool result = false;
		switch (gimmickType)
		{
			// ����
			case GimmickType.SEQUENCE:
				{
					if (listGimmick != null && listGimmick.Count == 0) // ���� ����� ù ���
						result = true;
				}
				break;
			// ONOFF_ON
			case GimmickType.ONOFF_ON: // ONOFF ��Ʈ���� ����
				{
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
		GimmickCathegory gimmickCathegory = Define.GetGimmickCathegory(gameObjectGimmick.gimmickType);
		gameObjectGimmick.hp--;
		gameObjectGimmick.SetColor(GetGimmickColor(gameObjectGimmick));
		if (gimmickCathegory == GimmickCathegory.GimmickHit)
		{
			if(gameObjectGimmick.gimmickType == GimmickType.REMOVE_SHOT)
			{
				Vibrate4();
			}
			else
			{
				Vibrate1();
			}
		}
		StartCoroutine(Shake(gameObject, ShakeType.NORMAL));
		if (gameObjectGimmick.hp <= 0)
		{
			DestroyGimmick(gameObject, gameObjectGimmick, false);
		}
	}

	private void DestroyShot(GameObject gameObject, Pin pin)
	{
		if (gameObject.IsDestroyed())
			return;

		// ��� ����
		gameObject.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
		{
			pin.EffectPlay();
		});
	}

	private void DestroyGimmick(GameObject gameObject, Gimmick gameObjectGimmick, bool afterClear)
	{
		if (gameObject.IsDestroyed())
			return;

		if(gameObjectGimmick != null)
		{
			if (afterClear)
			{
				// ����Ʈ ���
				gameObjectGimmick.EffectPlay();
			}
			else
			{
				// ���̵���� ����
				gameObjectGimmick.DisableGuideLine();
			}
		}
		
		// ��� ����
		gameObject.transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.InBack).OnComplete(() =>
		{
			if(!afterClear)
				Destroy(gameObject);
		});
	}

	private bool GimmickCheckWork(GameObject gameObject, Gimmick gameObjectGimmick)
	{
		bool noDamage = false;
		switch (gameObjectGimmick.gimmickType)
		{
			case GimmickType.CONTINUE_HIT:
				{
					if (gameObjectGimmick.GetChecked())
					{
						// ������ �߰�
						GimmickHpMinusWork(gameObject, gameObjectGimmick);
					}
					else
					{
						// üũ ���·� ����
						gameObjectGimmick.SetChecked();
						gameObjectGimmick.SetGimmickSprite(2);
						noDamage = true;
					}
				}
				break;

			case GimmickType.ONOFF_ON:
			case GimmickType.ONOFF_OFF:
				{
					if (gameObjectGimmick.GetChecked())
					{
						// ������ �߰�
						GimmickHpMinusWork(gameObject, gameObjectGimmick);
					}
					else
					{
						noDamage = true;
					}
				}
				break;
		}
		return noDamage;
	}

	private void GimmickListWork(GameObject gameObject, Gimmick gameObjectGimmick)
	{
		List<GameObject> gimmickList = gameObjectGimmick.GetListGimmick();
		if (gimmickList == null)
			return;

		switch (gameObjectGimmick.gimmickType)
		{
			// ����
			case GimmickType.SEQUENCE:
				{
					// Ÿ�� üũ �ȵǾ� ������ �� ����
					if (!gameObjectGimmick.GetChecked())
						return;
					// ��������Ʈ ��ü
					gameObjectGimmick.SetUnChecked();
					gameObjectGimmick.SetColor(GetGimmickColor(gameObjectGimmick));
					gameObjectGimmick.SetGimmickSprite(2);
					
					Gimmick nextGameObjectGimmick = GetNextGameObject(gimmickList, gameObject);
					if(nextGameObjectGimmick != null)
					{
						// ���� ������Ʈ Ȱ��ȭ
						nextGameObjectGimmick.SetChecked();
						nextGameObjectGimmick.SetColor(GetGimmickColor(nextGameObjectGimmick));
					}
					else
					{
						// �ش� ����Ʈ�� ��͵� ��� üũ ����
						RemoveAllGimmicks(gimmickList);
					}


				}
				break;

			// ��
			case GimmickType.KEY_CHAIN:
				{
					// üũ �Ǿ������� ����
					if (gameObjectGimmick.GetChecked())
						return;
					// üũ ó��
					gameObjectGimmick.SetChecked();
					gameObjectGimmick.SetColor(GetGimmickColor(gameObjectGimmick));
					if (CheckAllChecked(gimmickList))
					{
						AudioManager.instance.PlaySfx(AudioManager.Sfx.shot_special);
						RemoveAllGimmicks(gimmickList);
						inShield = false;
						targetCircle.ShieldColorOFF();
					}
				}
				break;
		}
	}

	public ShotGimmickHitResult GimmickHitWork(GameObject gameObject)
	{
		bool reflectPin = true;
		Gimmick gameObjectGimmick = gameObject.GetComponent<Gimmick>();
		GimmickDBEntity gimmickInfo = LocalDataManager.instance.GetGimmickInfo(gameObjectGimmick.gimmickType);
		bool ironGimmick = false;

		switch (gameObjectGimmick.gimmickType)
		{
			// ��� ��Ʈ��
			case GimmickType.SHIELD:
				{
					GimmickHpMinusWork(gameObject, gameObjectGimmick);
				}
				break;
			case GimmickType.SUPER_SHIELD:
				{
					//AudioManager.instance.PlaySfx(AudioManager.Sfx.shoot_failure);
					ironGimmick = true;
				}
				break;

			case GimmickType.TARGET_RECOVER:
				{
					IncreaseHP(gimmickInfo.value1);
					GimmickHpMinusWork(gameObject, gameObjectGimmick);
				}
				break;
			case GimmickType.DAMAGE_N:
				{
					AudioManager.instance.PlaySfx(AudioManager.Sfx.shot_special);
					DecreaseHP(gimmickInfo.value1);
					GimmickHpMinusWork(gameObject, gameObjectGimmick);
					if (hp <= 0)
					{
						SetGameOver(true);
					}
				}
				break;
			case GimmickType.SEQUENCE:
				{
					GimmickListWork(gameObject, gameObjectGimmick);
				}
				break;
			case GimmickType.KEY_CHAIN:
				{
					GimmickListWork(gameObject, gameObjectGimmick);
				}
				break;
			case GimmickType.CONTINUE_HIT:
				{
					this.curHitGimmick = gameObject;
					GimmickCheckWork(gameObject, gameObjectGimmick);
				}
				break;
			case GimmickType.REMOVE_SHOT:
				{
					RemoveAllShot();
					GimmickHpMinusWork(gameObject, gameObjectGimmick);
					AudioManager.instance.PlaySfx(AudioManager.Sfx.shot_special);
				}
				break;
			case GimmickType.ONOFF_ON:
			case GimmickType.ONOFF_OFF:
				{
					bool noDamage = GimmickCheckWork(gameObject, gameObjectGimmick);
					if(noDamage)
					{
						ironGimmick = true;
					}
				}
				break;

			// Ÿ�� ��Ʈ�� reflectPin = false;
			case GimmickType.ROTATION_UP:
				{
					reflectPin = false;
					rotationBuff++;
					GimmickHpMinusWork(gameObject, gameObjectGimmick);
				}
				break;
			case GimmickType.ROTATION_DOWN:
				{
					reflectPin = false;
					rotationBuff--;
					GimmickHpMinusWork(gameObject, gameObjectGimmick);
				}
				break;
			case GimmickType.ADD_SHOT:
				{
					reflectPin = false;
					shot += gimmickInfo.value1;
					SetShotText();
					GimmickHpMinusWork(gameObject, gameObjectGimmick);
				}
				break;
		}
		if (reflectPin)
		{
			if (ironGimmick)
				return ShotGimmickHitResult.HIT_IRON_REFLECT;
			else
				return ShotGimmickHitResult.HIT_REFLECT;
		}
		else
		{
			return ShotGimmickHitResult.HIT_THROUTH;
		}
	}

	public float GetRotationValue(float rotation)
	{
		if(rotationBuff == 0)
		{
			return rotation;
		}

		float addValue = LocalDataManager.instance.GetGimmickInfo(GimmickType.ROTATION_UP).value1 * rotationBuff;
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

	public void DestroyAllGimmicks()
	{
		Gimmick[] gimmicks = Object.FindObjectsByType<Gimmick>(FindObjectsSortMode.None);
		for (int i = 0; i < gimmicks.Length; i++)
		{
			DestroyGimmick(gimmicks[i].gameObject, gimmicks[i], true);
		}
	}

	public void DestroyAllShots()
	{
		Pin[] pins = Object.FindObjectsByType<Pin>(FindObjectsSortMode.None);
		for (int i = 0; i < pins.Length; i++)
		{
			DestroyShot(pins[i].gameObject, pins[i]);
		}
	}

	public void RemoveAllGimmicks(List<GameObject> list)
	{
		foreach(GameObject go in list)
		{
			GimmickHpMinusWork(go);
		}
	}

	public bool IsInShield()
	{
		return inShield;
	}

	public bool CheckAllChecked(List<GameObject> list)
	{
		foreach (GameObject go in list)
		{
			if (go.IsDestroyed())
				continue;
			Gimmick gameObjectGimmick = go.GetComponent<Gimmick>();
			if (gameObjectGimmick == null)
				return false;
			if(!gameObjectGimmick.GetChecked())
				return false;
		}

		return true;
	}

	public void ResetHitGimmick()
	{
		Debug.Log("���� ResetHitGimmick");
		curHitGimmick = null;
	}

	public void CheckListGimmickStatus()
	{
		CheckListGimmickStatus(listGimmick1);
		CheckListGimmickStatus(listGimmick2);
		CheckListGimmickStatus(listGimmick3);
		CheckListGimmickStatus(listGimmick4);
		CheckListGimmickStatus(listGimmick5);
		CheckListGimmickStatus(listGimmick6);
		CheckListGimmickStatus(listGimmick7);
	}

	private void CheckListGimmickStatus(List<GameObject> list)
	{
		if (list == null || list.Count == 0)
			return;
		foreach (GameObject go in list)
		{
			if(go.IsDestroyed())
				continue;
			Gimmick gameObjectGimmick = go.GetComponent<Gimmick>();
			if(gameObjectGimmick != null)
			{
				switch(gameObjectGimmick.gimmickType)
				{
					case GimmickType.CONTINUE_HIT:
						{
							Debug.Log($"���� üũ GimmickType.CONTINUE_HIT: {curHitGimmick}");
							if (go == curHitGimmick)
							{
								// ���� ��ŵ
							}
							else
							{
								if (gameObjectGimmick.GetChecked())
								{
									// ���� ����
									gameObjectGimmick.SetUnChecked();
									gameObjectGimmick.SetGimmickSprite(1);
								}
							}
						}
						break;

					case GimmickType.ONOFF_ON:
					case GimmickType.ONOFF_OFF:
						{
							if (gameObjectGimmick.GetChecked())
							{
								gameObjectGimmick.SetUnChecked();
								gameObjectGimmick.SetGimmickSprite(1);
							}
							else
							{
								gameObjectGimmick.SetChecked();
								gameObjectGimmick.SetGimmickSprite(2);
							}
						}
						break;
				}
			}
		}
	}

	public void AddPinnedShot(GameObject go)
	{
		listPinnedShot.Add(go);
	}

	private void RemoveAllShot()
	{
		// Ÿ�� ����ŷ
		StartCoroutine(Shake(targetCircle.gameObject, ShakeType.LONG));
		foreach (GameObject go in listPinnedShot)
		{
			if(!go.IsDestroyed())
			{
				go.transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.InBack).OnComplete(() => Destroy(go));
			}
		}
	}

	public int GetHpAmountByTargetAngle(int angle, bool upgradeShot)
	{
		int baseDamage = 1;
		int shotUpgradeDamage = 0;
		int areaBonusDamage = 0;
		
		if (upgradeShot)
			shotUpgradeDamage = 1;
		if (dic_doubleDamage.ContainsKey(angle))
			areaBonusDamage = 1;
		else if (dic_noDamage.ContainsKey(angle))
			return 0;

		return (baseDamage + shotUpgradeDamage + areaBonusDamage);
	}

	public AudioManager.Sfx GetComboSFX(int combo)
	{
		return AudioManager.Sfx.shot_good;
	}

	public void AddCombo(Vector3 position)
	{
		combo++;
		float pitch = 1.0f + ((combo-1) * 0.1f);
		AudioManager.instance.PlaySfx(GetComboSFX(combo), pitch);
		SetComboText();
		ShowComboLabel(position, combo);
		if (combo >= Define.MAX_COMBO)
		{
			shotAddDamage = true;
			Invoke("ResetCombo", 0.1f);
		}
	}

	public void ResetCombo()
	{
		combo = 0;
		SetComboText();
	}

	public void SetCreatedPin(Pin _currPin)
	{
		currPin = _currPin;
	}

	public void CheckUpgradePin(bool forced)
	{
		if (forced)
			shotAddDamage = true;

		if (currPin != null && currPin.isAbleUpgrade() && shotAddDamage)
		{
			currPin.Upgrade();
			shotAddDamage = false;
			currPin.ScaleShake();
			//StartCoroutine(Shake(currPin.gameObject, ShakeType.IMPACK));
		}
	}

	private IEnumerator Shake(GameObject gameObject, Define.ShakeType shakeType)
	{
		float shakeDuration = 0.06f; // ��鸲 ���� �ð�
		float shakeMagnitude = 0.05f; // ��鸲 ����
		float elapsed = 0.0f;

		if (shakeType == ShakeType.LONG)
		{
			shakeDuration = 0.3f;
		}
		else if (shakeType == ShakeType.IMPACK)
		{
			shakeMagnitude = 0.3f;
		}

		bool expressionScaleUp = false;
		TargetCircle targetCircle = gameObject.GetComponent<TargetCircle>();
		if (targetCircle != null)
		{
			expressionScaleUp = true;
			targetCircle.SetExpressionLineScaleUp();
		}

		Transform childTransform = gameObject.transform.Find(Define.CHILD_SPRITE_OBJECT);
		SpriteRenderer spriteRenderer = childTransform.gameObject.GetComponent<SpriteRenderer>();
		Color originColor = spriteRenderer.color;
		spriteRenderer.color = new Color(0.2f, 0.2f, 0.2f, 1f);

		while (elapsed < shakeDuration)
		{
			// ������ ��ġ ����
			float offsetX = UnityEngine.Random.Range(-1f, 1f) * shakeMagnitude;
			float offsetY = UnityEngine.Random.Range(-1f, 1f) * shakeMagnitude;

			// ������Ʈ�� Sprite ��ġ ����
			if (!childTransform.IsDestroyed())
			{
				childTransform.localPosition = new Vector3(offsetX, offsetY, 0);
			}

			// ��� �ð� ����
			elapsed += Time.deltaTime;

			// ���� �����ӱ��� ���
			yield return null;
		}

		if(expressionScaleUp)
		{
			if(!targetCircle.IsDestroyed())
				targetCircle.SetExpressionLineScaleNormal();
		}

		// ��鸲 ���� �� ���� ��ġ�� ����
		if (!childTransform.IsDestroyed() && !childTransform.gameObject.IsDestroyed())
		{
			childTransform.localPosition = Vector3.zero;
		}
		if(!spriteRenderer.IsDestroyed())
			spriteRenderer.color = originColor;
	}

	public int Random100()
	{
		return UnityEngine.Random.Range(1, 101);
	}

	public bool TriggerSkill(PassiveType passiveType)
	{
		GradeDBEntity entity = LocalDataManager.instance.GetCurGradeDBEntity();
		if (entity == null)
			return false;

		int curRate = 0;
		switch(passiveType)
		{
			case PassiveType.SHOT_DOUBLE_DAMAGE:
				curRate = entity.doubleDamage;
				if (checkFinalShot) // �߰� �߻�ü �����ÿ��� ��ų �ߵ� ���� �ʵ���
					curRate = 0;
				break;

			case PassiveType.FAILURE_BONUS_SHOT:
				curRate = entity.bonusShot;
				break;
		}

		int randValue = Random100();
		if (randValue <= curRate)
			return true;

		return false;
	}

	public bool CheckTriggerSkill(PassiveType passiveType)
	{
		if (!TriggerSkill(passiveType)) // ��ų�� ���� ������ , ��ų�� ����
			return false;

		SkillWork(passiveType); // ��ų�� ���� �߰� ��� ����
		ShowPopupSkill(passiveType);
		return true;
	}

	private void SkillWork(PassiveType passiveType)
	{
		switch(passiveType)
		{
			case PassiveType.SHOT_DOUBLE_DAMAGE:
				{
					CheckUpgradePin(true);
					textSkill.SetText(Define.SKILL_DOUBLE_DAMAGE);
				}
				break;
			case PassiveType.FAILURE_BONUS_SHOT:
				{
					this.shot = 1;
					SetShotText();
					textShot.color = Color.red;
					textSkill.SetText(Define.SKILL_FINAL_SHOT);
				}
				break;
		}
	}

	public void ShowPopupSkill(PassiveType passiveType)
	{
		// ���� �߻�
		AudioManager.instance.PlaySfx(AudioManager.Sfx.buff);
		// �ؽ�Ʈ�� ȭ�� �Ʒ��� ��ġ
		textSkill.rectTransform.anchoredPosition = new Vector2(-800, 600);

		// ������ ����
		DG.Tweening.Sequence sequence = DOTween.Sequence();

		// ȭ�� �ϴܿ��� ���� �̵� (��: y = 0) - 0.5�� ����
		sequence.Append(textSkill.rectTransform.DOAnchorPosX(0f, 0.03f).SetEase(Ease.OutQuad));

		// ���
		sequence.AppendInterval(0.85f);

		// �ٽ� �̵� - 0.5�� ����
		sequence.Append(textSkill.rectTransform.DOAnchorPosX(-800, 0.1f).SetEase(Ease.InQuad));

		// ������ ����
		sequence.Play();
	}

	public bool GetCheckFinalShot()
	{
		return checkFinalShot;
	}

	public void CheckFinalShot()
	{
		if(shot == 0)
		{
			if (checkFinalShot)
				return;

			CheckTriggerSkill(PassiveType.FAILURE_BONUS_SHOT);
			checkFinalShot = true;
		}
	}

	private void SetOrthographicSize()
	{
		int screenWidth = Screen.width;
		int screenHeight = Screen.height;
		float aspectRatio = (float)screenWidth / screenHeight;
		float orthographicSize = 5f;
		if(aspectRatio > 0.56f)
		{
			orthographicSize = 5f;
		}
		else if(aspectRatio < 0.36)
		{
			orthographicSize = 7f;
		}
		else
		{
			orthographicSize = (0.56f - aspectRatio)*10f + 5f;
		}
		Camera.main.orthographicSize = orthographicSize;

		if (aspectRatio > 0.65f)
		{
			skillPosY = 200;
		}
		else
		{
			skillPosY = 300;
		}
		
		Debug.Log($"SetOrthographicSize: {screenWidth}, {screenHeight}, {aspectRatio}, {skillPosY}");
	}

	public void PopupResultSuccess()
	{
		popupResult.SetActive(true);
	}

	public Vector3 GetTargetPinnedPosition()
	{
		float y = pinPositionY100;
		if (targetScale == 80)
			y = pinPositionY80;
		else if (targetScale == 50)
			y = pinPositionY50;

		return new Vector3(0f, y, 0f);
	}

	public float GetTargetHPPosition()
	{
		float value = 0f;
		if (targetScale == 100)
			value = hpPositionY100;
		else if (targetScale == 80)
			value = hpPositionY80;
		else if (targetScale == 50)
			value = hpPositionY50;
		return value;
	}

	public void ShowComboLabel(Vector3 position, int combo)
	{
		float magnitude = 0.15f;
		// ������ ��ġ ����
		float offsetX = UnityEngine.Random.Range(-0.6f, 1.4f) * magnitude;
		float offsetY = UnityEngine.Random.Range(5f, 7f) * magnitude;
		//ComboEffect
		Vector3 newPosition = new Vector3(position.x+offsetX, position.y+offsetY, 0f);
		Vector3 screenPos = Camera.main.WorldToScreenPoint(newPosition);
		GameObject comboEffectGameObject = Instantiate(ComboEffect, screenPos, Quaternion.identity);
		TextMeshProUGUI textObject = comboEffectGameObject.GetComponent<TextMeshProUGUI>();
		textObject.color = new Color(0.5f+0.1f*combo, 0f, 0f);
		textObject.text = combo + "Combo";
		comboEffectGameObject.transform.SetParent(ComboEffects.transform);
		float scale = 1f;
		if (combo == 3)
			scale = 1.2f;
		else if (combo == 4)
			scale = 1.4f;
		else if (combo == 5)
			scale = 2f;
		comboEffectGameObject.transform.localScale = new Vector3(scale, scale, scale);
		StartCoroutine(DisappearText(comboEffectGameObject));
	}

	private IEnumerator DisappearText(GameObject gameObject)
	{
		TypewriterByCharacter typewriter = gameObject.GetComponent<TypewriterByCharacter>();
		float duration = 0.2f; // �����ð�
		float elapsed = 0.0f;
		while (elapsed < duration)
		{
			// ��� �ð� ����
			elapsed += Time.deltaTime;

			// ���� �����ӱ��� ���
			yield return null;
		}

		typewriter.StartDisappearingText();
	}

	public int GetNextPinID()
	{
		nextPinId++;
		return nextPinId;
	}

	public bool CheckGimmickShotWorked(Pin _pin, GameObject _gimmickObject)
	{
		Gimmick gimmick = _gimmickObject.GetComponent<Gimmick>();
		if (!gimmick.PinWork(_pin.GetPinID()))
		{
			// �̹� ó���� ��
			Debug.Log("OnTriggerEnter2D Gimmick Aleady WORKED : " + _pin.GetPinID());
			return true;
		}
		else
		{
			return false;
		}
	}

	public ShotGimmickHitResult ShotGimmickHit(Pin _pin, GameObject _gimmickObject)
	{
		if(CheckGimmickShotWorked(_pin, _gimmickObject))
			return ShotGimmickHitResult.ALEADY_COMPLETED;
		ShotGimmickHitResult result = GimmickHitWork(_gimmickObject);
		GameObject pair = GetPairGimmick(_gimmickObject);
		if (pair != null && !pair.IsDestroyed())
		{
			if(!CheckGimmickShotWorked(_pin, pair))
			{
				// ����� ������ ó��
				GimmickHitWork(pair);
			}
			// ��� ó���� �׻� ƨ��
			return ShotGimmickHitResult.HIT_PAIR_REFLECT;
		}

		return result;
	}

	private void AddAngleGimmick(int angle, GameObject gimmickObject)
	{
		if (dic_AngleGimmicks.TryGetValue(angle, out List<GameObject> list))
		{
			
		}
		else
		{
			list = new List<GameObject>();
			dic_AngleGimmicks.Add(angle, list);
		}
		list.Add(gimmickObject);
	}

	private void SetPairGimmick()
	{
		// ������ 2���� ����� �ְ� �ϳ��� �����Ʈ��, �ϳ��� Ÿ����Ʈ���̸� ����ͼ���
		foreach(List<GameObject> list in dic_AngleGimmicks.Values)
		{
			if (list.Count == 2)
			{
				Gimmick gimmick1 = list[0].GetComponent<Gimmick>();
				Gimmick gimmick2 = list[1].GetComponent<Gimmick>();
				GimmickCathegory gimmickCathegory1 = Define.GetGimmickCathegory(gimmick1.gimmickType);
				GimmickCathegory gimmickCathegory2 = Define.GetGimmickCathegory(gimmick2.gimmickType);
				if((gimmickCathegory1 == GimmickCathegory.GimmickHit && gimmickCathegory2 == GimmickCathegory.TargetHit) ||
					(gimmickCathegory2 == GimmickCathegory.GimmickHit && gimmickCathegory1 == GimmickCathegory.TargetHit))
				{
					// �����
					dic_PairGimmick.Add(list[0], list[1]);
					dic_PairGimmick.Add(list[1], list[0]);
				}
			}
		}
	}

	public GameObject GetPairGimmick(GameObject go)
	{
		if (dic_PairGimmick.TryGetValue(go, out GameObject pair))
		{
			return pair;
		}
		else
		{
			return null; ;
		}
	}

	private void CheckStageFailure()
	{
		if (!isGameOver)
		{
			// shot == 0 , hp > 0 , ���� ����, ���̳��� üũ ������ �̸� ����
			if (shot == 0 && hp > 0 && (currPin != null && currPin.GetWorked()) && checkFinalShot)
				SetGameOver(false);
		}
	}

	private void Vibrate(VibrationType type)
	{
		if (LocalDataManager.instance.GetVibrateOff())
			return;
		VibrationUtil.Vibrate(type);
	}
	public void Vibrate1()
	{
		Vibrate(VibrationType.Nope);
	}

	public void Vibrate2()
	{
		Vibrate(VibrationType.Success);
	}

	public void Vibrate3()
	{
		Vibrate(VibrationType.Error);
	}

	public void Vibrate4()
	{
		Vibrate(VibrationType.Default);
	}

	public void TutorialButtonClick()
	{
		if(tutorialButtonTab)
		{
			buttonTab.SetActive(false);
		}
	}

	public void TargetEffect()
	{
		targetCircle.EffectPlay();
		textTapTok.gameObject.SetActive(true);
	}

	public void PopupClearResult()
	{
		PopupResultSuccess();
		LocalDataManager.instance.ClearLevelPlayData();
	}
}
