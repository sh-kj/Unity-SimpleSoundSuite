using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UniRx;


namespace radiants.SimpleSoundSuite
{

	public class SoundObjectPool
	{
		private Transform PoolParent { get; }

		private ObjectPool<SoundObject> Pool { get; }
		private Dictionary<int, SoundObject> CurrentActiveObjects { get; }


		private CompositeDisposable Disposables = new CompositeDisposable();

		public SoundObjectPool(Transform poolParent, int defaultCapacity = 10, int maxSize = 10000)
		{
			PoolParent = poolParent;
			CurrentActiveObjects = new Dictionary<int, SoundObject>();

			Pool = new ObjectPool<SoundObject>(CreateFunc, OnGetAction, OnReleaseAction, OnDestroyAction
				, true, defaultCapacity, maxSize);
		}

		~SoundObjectPool()
		{
			Disposables.Dispose();
		}

		private SoundObject CreateFunc()
		{
			var go = new GameObject("SoundObject");
			go.transform.SetParent(PoolParent, false);
			var ao = go.AddComponent<SoundObject>();
			ao.OnFinished.Subscribe(_ => Pool.Release(ao))
				.AddTo(Disposables);

			return ao;
		}
		private void OnDestroyAction(SoundObject obj)
		{
			UnityEngine.Object.Destroy(obj.gameObject);
		}

		private void OnGetAction(SoundObject obj)
		{
			//obj.gameObject.SetActive(true);
			CurrentActiveObjects.Add(obj.PlayerID, obj);
		}

		private void OnReleaseAction(SoundObject obj)
		{
			//obj.gameObject.SetActive(false);
			CurrentActiveObjects.Remove(obj.PlayerID);
		}

		

		public SoundObject Get()
		{
			return Pool.Get();
		}

		public void CheckPolyphonyAndStop(SoundElement element)
		{
			if (element.Polyphony == 0) return;

			int firstKey = -1;
			int nowPlaying = 0;
			foreach (var active in CurrentActiveObjects)
			{
				if (active.Value.NowPlayingSound != element.ID) continue;

				//àÍî‘å√Ç¢Ç‡ÇÃÇãLò^ÇµÇƒÇ®Ç≠
				if (firstKey == -1) firstKey = active.Key;
				nowPlaying++;

				//Debug.Log("playing " + nowPlaying + ":" + active.Key);

				if (nowPlaying >= element.Polyphony)
				{
					//àÍî‘å√Ç¢Ç‡ÇÃÇé~ÇﬂÇÈÅB1å¬é~ÇﬂÇÍÇŒÇ¢Ç¢ÇÕÇ∏
					//Debug.Log("Stopped:" + firstKey);
					StopImmediately(firstKey);
					return;
				}
			}
		}

		public void Stop(int playerID, float fadeOutSeconds = 0.1f)
		{
			if (!CurrentActiveObjects.ContainsKey(playerID)) return;

			var obj = CurrentActiveObjects[playerID];
			obj.Stop(fadeOutSeconds);
		}

		private void StopImmediately(int playerID)
		{
			if (!CurrentActiveObjects.ContainsKey(playerID)) return;

			var obj = CurrentActiveObjects[playerID];
			obj.StopImmediately();
		}
	}
}