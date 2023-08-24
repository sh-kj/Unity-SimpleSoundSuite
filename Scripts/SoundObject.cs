using System.Collections;
using System.Collections.Generic;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.Audio;


namespace radiants.SimpleSoundSuite
{

	[RequireComponent(typeof(AudioSource))]
	public class SoundObject : MonoBehaviour
	{
		private AudioSource MySource { get; set; }

		private Subject<Unit> OnFinishedSubject = new Subject<Unit>();
		public IObservable<Unit> OnFinished { get { return OnFinishedSubject; } }

		private Coroutine WaitAndFinishCoroutine;


		private void Awake()
		{
			MySource = GetComponent<AudioSource>();
			MySource.playOnAwake = false;
		}

		public void Play(SoundSingleElement element, AudioMixerGroup output, bool loop = false)
		{
			MySource.loop = loop;
			MySource.clip = element.Clip;
			MySource.outputAudioMixerGroup = output;
			MySource.volume = element.Volume;

			if (element.ChangePan)
				MySource.panStereo = UnityEngine.Random.Range(element.PanRange.x, element.PanRange.y);
			else
				MySource.panStereo = 0f;

			if (element.ChangePitch)
				MySource.pitch = UnityEngine.Random.Range(element.PitchRange.x, element.PitchRange.y);
			else
				MySource.pitch = 1f;

			MySource.Play();

			if(!loop)
				WaitAndFinishCoroutine = StartCoroutine(WaitAndFinish(element.Clip.length));
		}

		private IEnumerator WaitAndFinish(float seconds)
		{
			yield return new WaitForSeconds(seconds);
			OnFinishedSubject.OnNext(Unit.Default);
		}


		public void Stop()
		{
			MySource.Stop();

			//手動で止まった場合は自動でFinishするコルーチンを止める
			if (WaitAndFinishCoroutine != null) StopCoroutine(WaitAndFinishCoroutine);
			WaitAndFinishCoroutine = null;

			OnFinishedSubject.OnNext(Unit.Default);
		}
	}
}