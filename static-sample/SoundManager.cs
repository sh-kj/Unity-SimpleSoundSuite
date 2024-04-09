public interface ISoundPlayer
{
	int Play(long soundID, bool loop, UnityEngine.Audio.AudioMixerGroup outputGroup);
	int Play(string soundName, bool loop, UnityEngine.Audio.AudioMixerGroup outputGroup);

	Cysharp.Threading.Tasks.UniTask PlayAwaitable(long soundID, bool loop, UnityEngine.Audio.AudioMixerGroup outputGroup);
	Cysharp.Threading.Tasks.UniTask PlayAwaitable(string soundName, bool loop, UnityEngine.Audio.AudioMixerGroup outputGroup);

	void Stop(int playerID);


	void SetMusicMixerGroup(UnityEngine.Audio.AudioMixerGroup group);

	void PlayMusic(long soundID, float fadeInSeconds, bool forceReplay);
	void PlayMusic(string soundName, float fadeInSeconds, bool forceReplay);
	void StopMusic(float fadeoutSeconds);
}


public static class SoundManager
{
	private static ISoundPlayer Player;

	public static void Inject(ISoundPlayer soundPlayer)
	{
		Player = soundPlayer;
	}

	public static int Play(string soundName, bool loop = false, UnityEngine.Audio.AudioMixerGroup outputGroup = null)
	{
		if (Player == null) return -1;
		return Player.Play(soundName, loop, outputGroup);
	}
	public static int Play(long soundID, bool loop = false, UnityEngine.Audio.AudioMixerGroup outputGroup = null)
	{
		if (Player == null) return -1;
		return Player.Play(soundID, loop, outputGroup);
	}

	public static async Cysharp.Threading.Tasks.UniTask PlayAwaitable(long soundID, bool loop = false, UnityEngine.Audio.AudioMixerGroup outputGroup = null)
	{
		if (Player == null) return;
		await Player.PlayAwaitable(soundID, loop, outputGroup);
	}
	public static async Cysharp.Threading.Tasks.UniTask PlayAwaitable(string soundName, bool loop = false, UnityEngine.Audio.AudioMixerGroup outputGroup = null)
	{
		if (Player == null) return;
		await Player.PlayAwaitable(soundName, loop, outputGroup);
	}

	public static void Stop(int playerID)
	{
		if (Player == null) return;
		Player.Stop(playerID);
	}


	public static void SetMusicMixerGroup(UnityEngine.Audio.AudioMixerGroup group)
	{
		if (Player == null) return;
		Player.SetMusicMixerGroup(group);
	}
	public static void PlayMusic(long soundID, float fadeInSeconds = 0f, bool forceReplay = false)
	{
		if (Player == null) return;
		Player.PlayMusic(soundID, fadeInSeconds, forceReplay);
	}
	public static void PlayMusic(string soundName, float fadeInSeconds = 0f, bool forceReplay = false)
	{
		if (Player == null) return;
		Player.PlayMusic(soundName, fadeInSeconds, forceReplay);
	}
	public static void StopMusic(float fadeoutSeconds)
	{
		if (Player == null) return;
		Player.StopMusic(fadeoutSeconds);
	}

}
