using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;


namespace radiants.SimpleSoundSuite
{
	public static class Fader
	{
		public static async UniTask Fade(float time,
			System.Action<float> onNext,
			System.Action onComplete = null,
			bool useUnscaledTime = false,
			PlayerLoopTiming timing = PlayerLoopTiming.Update,
			CancellationToken cancellationToken = default)
		{
			float elapsedTime = 0f;
			while (elapsedTime < time)
			{
				float scaledTime = elapsedTime / time;
				onNext(scaledTime);

				await UniTask.Yield();

				elapsedTime += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

				if(cancellationToken.IsCancellationRequested)
				{
					onComplete?.Invoke();
					return;
				}
			}
			onNext(1f);
			onComplete?.Invoke();
		}
	}
}