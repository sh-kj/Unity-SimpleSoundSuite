using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

public class SoundPlayerInjector : MonoBehaviour
{
	[SerializeField]
	private radiants.SimpleSoundSuite.SoundPlayer SoundPlayerInstance;

	private void Awake()
	{
		SoundPlayerWrapper wrapper = new SoundPlayerWrapper(SoundPlayerInstance);
		SoundManager.Inject(wrapper);
	}
}

public class SoundPlayerWrapper : ISoundPlayer
{
	private radiants.SimpleSoundSuite.SoundPlayer Player;

	public SoundPlayerWrapper(radiants.SimpleSoundSuite.SoundPlayer player)
	{
		Player = player;
	}

	int ISoundPlayer.Play(long soundID, bool loop, AudioMixerGroup outputGroup)
	{
		return Player.Play(soundID, loop, outputGroup);
	}

	int ISoundPlayer.Play(string soundName, bool loop, AudioMixerGroup outputGroup)
	{
		return Player.Play(soundName, loop, outputGroup);
	}

	UniTask ISoundPlayer.PlayAwaitable(long soundID, bool loop, AudioMixerGroup outputGroup)
	{
		return Player.PlayAwaitable(soundID, loop, outputGroup);
	}

	UniTask ISoundPlayer.PlayAwaitable(string soundName, bool loop, AudioMixerGroup outputGroup)
	{
		return Player.PlayAwaitable(soundName, loop, outputGroup);
	}

	void ISoundPlayer.PlayMusic(long soundID, float fadeInSeconds, bool forceReplay)
	{
		Player.PlayMusic(soundID, fadeInSeconds, forceReplay);
	}

	void ISoundPlayer.PlayMusic(string soundName, float fadeInSeconds, bool forceReplay)
	{
		Player.PlayMusic(soundName, fadeInSeconds, forceReplay);
	}

	void ISoundPlayer.SetMusicMixerGroup(AudioMixerGroup group)
	{
		Player.SetMusicMixerGroup(group);
	}

	void ISoundPlayer.Stop(int playerID)
	{
		Player.Stop(playerID);
	}

	void ISoundPlayer.StopMusic(float fadeoutSeconds)
	{
		Player.StopMusic(fadeoutSeconds);
	}
}