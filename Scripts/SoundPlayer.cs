using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;
using UniRx;


namespace radiants.SimpleSoundSuite
{
	public class SoundPlayer : MonoBehaviour
	{
		[SerializeField]
		private Jukebox[] DefaultJukeboxes;


		private List<Jukebox> AdditionalJukeBoxes { get; } = new List<Jukebox>();

		private CompositeDisposable Disposables = new CompositeDisposable();

		public void AddJukeBox(Jukebox jukebox)
		{
			jukebox.MakeIndex();
			AdditionalJukeBoxes.Add(jukebox);
		}

		private void Start()
		{
			InitPool();

			//index作成を済ませておく
			for (int i = 0; i < DefaultJukeboxes.Length; i++)
			{
				DefaultJukeboxes[i].MakeIndex();
			}
		}




		public int Play(string soundName, AudioMixerGroup outputGroup = null)
		{
			var element = GetElement(soundName);
			if (element == null) return -1;
			var single = element.GetNext();
			if (single == null) return -1;

			var audio = Pool.Get();
			audio.Play(single, outputGroup);

			return audio.GetInstanceID();
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

		#region AudioObject Pool

		private IObjectPool<SoundObject> Pool { get; set; }

		private void InitPool()
		{
			Pool = new ObjectPool<SoundObject>(CreatePoolItem, obj => obj.gameObject.SetActive(true), obj => obj.gameObject.SetActive(false),
				obj => Destroy(obj.gameObject), true, 10, 10000);
		}


		private SoundObject CreatePoolItem()
		{
			var go = new GameObject("audio object");
			go.transform.SetParent(this.transform, false);
			var ao = go.AddComponent<SoundObject>();
			ao.OnFinished.Subscribe(_ => Pool.Release(ao))
				.AddTo(Disposables);

			return ao;
		}

		private void OnDestroy()
		{
			Disposables.Dispose();
		}

		#endregion

	}

}