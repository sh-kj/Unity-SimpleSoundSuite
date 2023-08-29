using System.Collections;
using System.Collections.Generic;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.Audio;
using radiants.ReactiveTween;

using Cysharp.Threading.Tasks;


namespace radiants.SimpleSoundSuite
{

	[RequireComponent(typeof(AudioSource))]
	internal class SoundObject : MonoBehaviour
	{
		private AudioSource MySource { get; set; }

		private Subject<Unit> OnFinishedSubject = new Subject<Unit>();
		public IObservable<Unit> OnFinished { get { return OnFinishedSubject; } }

		private float ElementVolume { get; set; } = 1.0f;

		public long NowPlayingSoundID { get; private set; }
		public string NowPlayingSoundName { get; private set; }

		//instanceIDはマイナス値なので、逆転させてプラスにしたものをPlayerIDとする
		public int PlayerID { get { return -GetInstanceID(); } }

		private void Awake()
		{
			MySource = GetComponent<AudioSource>();
			MySource.playOnAwake = false;
		}

		public async UniTask Play(SoundSingleElement element, long id, string name,
			float fadeInSeconds = 0f, bool loop = false, AudioMixerGroup output = null)
		{
			MySource.loop = loop;
			MySource.clip = element.Clip;
			MySource.outputAudioMixerGroup = output;
			ElementVolume = element.Volume;
			NowPlayingSoundID = id;
			NowPlayingSoundName = name;

			if (element.ChangePan)
				MySource.panStereo = UnityEngine.Random.Range(element.PanRange.x, element.PanRange.y);
			else
				MySource.panStereo = 0f;

			if (element.ChangePitch)
				MySource.pitch = UnityEngine.Random.Range(element.PitchRange.x, element.PitchRange.y);
			else
				MySource.pitch = 1f;

			//fade in
			if (fadeInSeconds > 0f)
				MySource.volume = 0f;
			else
				MySource.volume = 1f;

			MySource.Play();

			//fade in
			if(fadeInSeconds > 0f)
				await Fader.Fade(fadeInSeconds, f => MySource.volume = ElementVolume * f, null, true);

			//(async) wait for playing
			await UniTask.WaitWhile(() =>
			{
				if (MySource == null) return false;
				return MySource.isPlaying;
			});
			OnFinishedSubject.OnNext(Unit.Default);
		}


		public async void Stop(float fadeOutSeconds = 0.1f)
		{
			//ElementVolumeを基準にフェードアウトする
			await Fader.Fade(fadeOutSeconds, f => 
			{
				MySource.volume = ElementVolume * (1.0f - f);
			}, () => 
			{
				//OnFinishedは自動で呼ばれるはず
				MySource.Stop();
			}, true);
		}

		public void StopImmediately()
		{
			MySource.Stop();
		}
	}
}