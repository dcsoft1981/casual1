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

	public enum Sfx { clear , shoot_failure , shoot_good , buff }

	private void Awake()
	{
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
	}

	public void PlaySfx(Sfx sfx)
	{
		if (LocalDataManager.instance.GetSoundOff())
			return;
		for( int index = 0; index < channels; ++index) {
			int loopIndex = (index + channelIndex) % sfxPlayers.Length;

			if (sfxPlayers[loopIndex].isPlaying)
				continue;
			channelIndex = loopIndex;
			sfxPlayers[loopIndex].clip = sfxClips[(int)sfx];
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
}
