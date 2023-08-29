using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;
using UniRx;
using Cysharp.Threading.Tasks;


namespace radiants.SimpleSoundSuite
{
	public class SoundPlayer : MonoBehaviour
	{
		[SerializeField]
		private Jukebox[] DefaultJukeboxes;


		private List<Jukebox> AdditionalJukeBoxes { get; } = new List<Jukebox>();

		private CompositeDisposable Disposables = new CompositeDisposable();

		private SoundObjectPool SoundPool { get; set; }
		private SoundObjectPool MusicPool { get; set; }


		public void AddJukeBox(Jukebox jukebox)
		{
			jukebox.MakeIndex();
			AdditionalJukeBoxes.Add(jukebox);
		}

		private void Start()
		{
			//各Poolを生成
			GameObject SoundParent = new GameObject("Sounds");
			GameObject MusicParent = new GameObject("Music");
			SoundParent.transform.SetParent(transform, false);
			MusicParent.transform.SetParent(transform, false);

			SoundPool = new SoundObjectPool(SoundParent.transform);
			MusicPool = new SoundObjectPool(MusicParent.transform, 3);

			//Jukeboxのindex作成を済ませておく
			for (int i = 0; i < DefaultJukeboxes.Length; i++)
			{
				DefaultJukeboxes[i].MakeIndex();
			}
		}



		/// <summary>
		/// 名前を指定してサウンドを再生する(効果音用)
		/// </summary>
		/// <param name="soundName">SoundElementのName</param>
		/// <param name="loop">ループ再生</param>
		/// <param name="outputGroup">出力先のAudioMixerGroup</param>
		/// <returns>playerID(中断用)</returns>
		public int Play(string soundName, bool loop = false, AudioMixerGroup outputGroup = null)
		{
			var element = GetElement(soundName);
			if (element == null) return -1;
			var single = element.GetNext();
			if (single == null) return -1;

			SoundPool.CheckPolyphonyAndStop(element);
			var audio = SoundPool.Get();
			_ = audio.Play(single, element.ID, 0f, loop, outputGroup);

			return audio.GetInstanceID();
		}

		/// <summary>
		/// 名前を指定してサウンドを再生する(効果音用)
		/// 音が終了するまでawaitで待機可能
		/// </summary>
		/// <param name="soundName">SoundElementのName</param>
		/// <param name="loop">ループ再生</param>
		/// <param name="outputGroup">出力先のAudioMixerGroup</param>
		/// <returns>playerID(中断用)</returns>
		public async UniTask PlayAwaitable(string soundName, bool loop = false, AudioMixerGroup outputGroup = null)
		{
			var element = GetElement(soundName);
			if (element == null) return;
			var single = element.GetNext();
			if (single == null) return;

			SoundPool.CheckPolyphonyAndStop(element);
			var audio = SoundPool.Get();
			await audio.Play(single, element.ID, 0f, loop, outputGroup);
		}

		/// <summary>
		/// playerIDを指定してサウンドを中断する(効果音用)
		/// </summary>
		/// <param name="playerID"></param>
		public void Stop(int playerID)
		{
			SoundPool.Stop(playerID);
		}

		private AudioMixerGroup MusicMixerGroup = null;
		private SoundObject CurrentMusicObject = null;

		/// <summary>
		/// BGM用のAudioMixerGroupを設定する
		/// </summary>
		/// <param name="group"></param>
		public void SetMusicMixerGroup(AudioMixerGroup group)
		{
			MusicMixerGroup = group;
		}

		/// <summary>
		/// 名前を指定してBGMを再生する。
		/// 既に他のBGMが再生中の場合、fadeInSecondsでクロスフェードして切り替わる。
		/// 同じBGMが既に再生中の場合は何もしない(forceReplay=trueで最初から再生しなおす)
		/// </summary>
		/// <param name="soundName"></param>
		/// <param name="fadeInSeconds"></param>
		/// <param name="forceReplay"></param>
		public void PlayMusic(string soundName, float fadeInSeconds = 0f, bool forceReplay = false)
		{
			var element = GetElement(soundName);
			if (element == null) return;


			if (CurrentMusicObject != null)
			{
				if(CurrentMusicObject.NowPlayingSound == element.ID && !forceReplay)
				{
					return;
				}

				//既存のBGMを止める
				float fadeoutSeconds = fadeInSeconds;
				if (fadeoutSeconds < 0.1f) fadeoutSeconds = 0.1f;
				CurrentMusicObject.Stop(fadeoutSeconds);
			}

			var single = element.GetNext();
			if (single == null) return;

			CurrentMusicObject = MusicPool.Get();
			_ = CurrentMusicObject.Play(single, element.ID, fadeInSeconds, true, MusicMixerGroup);
		}
		/// <summary>
		/// 現在流れているBGMを止める
		/// </summary>
		/// <param name="fadeoutSeconds">フェードアウトにかける時間</param>
		public void StopMusic(float fadeoutSeconds = 0.1f)
		{
			if (CurrentMusicObject == null) return;
			CurrentMusicObject.Stop(fadeoutSeconds);
		}

		#region Search AudioElement From Jukeboxes

		private SoundElement GetElement(string soundName)
		{
			//Additional優先
			for (int i = 0; i < AdditionalJukeBoxes.Count; i++)
			{
				var element = AdditionalJukeBoxes[i].GetElementByName(soundName);
				if (element != null) return element;
			}

			//無かったらDefault
			for (int i = 0; i < DefaultJukeboxes.Length; i++)
			{
				var element = DefaultJukeboxes[i].GetElementByName(soundName);
				if (element != null) return element;
			}
			return null;
		}

		#endregion



	}

}