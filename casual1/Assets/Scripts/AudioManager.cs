using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

	[Header("#BGM")]
	public AudioClip bgmClip;
	public float bgmVolume;
	AudioSource bgmPlayer;
	AudioHighPassFilter bgmHighPassFilter;

	[Header("#SFX")]
	public AudioClip[] sfxClips;
	public float sfxVolume;
	public int channels;
	AudioSource[] sfxPlayers;
	int channelIndex;

	public AudioClip ticktockClip;
	AudioSource ticktockPlayer = null;

	public enum Sfx 
	{
		clear , 
		shot_failure , 
		shot_good , 
		buff, 
		shot_special,
		shot_gimmick,
		fullcombo,
		iron_hit
	}

	private void Awake()
	{
		/*
		// 기존 인스턴스가 있으면 새로운 것을 삭제
		if (instance != null && instance != this)
		{
			Destroy(gameObject);
			return;
		}
		DontDestroyOnLoad(gameObject);
		*/

		instance = this;
		Init();
	}

	private void Init()
	{
		GameObject bgmObject = new GameObject("BgmPlayer");
		bgmObject.transform.parent = transform;
		bgmPlayer = bgmObject.AddComponent<AudioSource>();
		bgmPlayer.playOnAwake = false;
		bgmPlayer.loop = true;
		bgmPlayer.volume = bgmVolume;
		bgmPlayer.clip = bgmClip;
		bgmHighPassFilter = Camera.main.GetComponent<AudioHighPassFilter>();

		GameObject sfxObject = new GameObject("SfxPlayer");
		sfxObject.transform.parent = transform;
		sfxPlayers = new AudioSource[channels];
		for (int index = 0; index < channels; index++)
		{
			sfxPlayers[index] = sfxObject.AddComponent<AudioSource>();
			sfxPlayers[index].playOnAwake = false ;
			sfxPlayers[index].bypassListenerEffects = true;
			sfxPlayers[index].volume = sfxVolume;
		}

		if(ticktockClip != null)
		{
			GameObject ticktockObject = new GameObject("TickTockPlayer");
			ticktockObject.transform.parent = transform;
			ticktockPlayer = ticktockObject.AddComponent<AudioSource>();
			ticktockPlayer.playOnAwake = true;
			ticktockPlayer.loop = true;
			ticktockPlayer.volume = sfxVolume;
			ticktockPlayer.clip = ticktockClip;
		}
	}

	public void PlaySfx(Sfx sfx, float pitch = 1f)
	{
		if (LocalDataManager.instance.GetSoundOff())
			return;
		for ( int index = 0; index < channels; ++index) {
			int loopIndex = (index + channelIndex) % sfxPlayers.Length;

			if (sfxPlayers[loopIndex].isPlaying)
				continue;
			channelIndex = loopIndex;
			sfxPlayers[loopIndex].clip = sfxClips[(int)sfx];
			sfxPlayers[loopIndex].pitch = pitch;
			sfxPlayers[loopIndex].Play();
			break;
		}
	}

	public void PlayBgm()
	{
		if (LocalDataManager.instance.GetSoundOff())
			return;
		bgmPlayer.Play();
	}

	public void StopBgm()
	{
		bgmPlayer.Stop();
	}

	public void OnEffectBgm()
	{
		bgmHighPassFilter.enabled = true;
	}

	public void OffEffectBgm()
	{
		bgmHighPassFilter.enabled = false;
	}

	public void Vibrate()
	{
		if (LocalDataManager.instance.GetVibrateOff())
			return;
		Handheld.Vibrate();
	}

	public void TickTockPlay()
	{
		if (LocalDataManager.instance.GetSoundOff())
			return;
		if(ticktockPlayer != null)
			ticktockPlayer.Play();
	}

	public void TickTockStop()
	{
		if (ticktockPlayer != null)
			ticktockPlayer.Stop();
	}
}
