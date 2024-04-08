using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
namespace radiants.SimpleSoundSuite
{

	public enum ElementOrder
	{
		Sequential,
		Random,
		RandomNotRepeat,
	}

	[System.Serializable]
	public class SoundElement
	{
		public string Name = "";

		public long ID = 0;

		//同時発音数 0:無制限
		public int Polyphony = 0;

		public List<SoundSingleElement> SingleElements = new List<SoundSingleElement>();

		public ElementOrder Order = ElementOrder.RandomNotRepeat;

#if UNITY_EDITOR
		public bool FoldShown { get; set; } = false;
#endif


		private int lastUsed = -1;
		public SoundSingleElement? GetNext()
		{
			if (SingleElements.Count == 0) return null;
			switch (Order)
			{
				case ElementOrder.Sequential:
					return GetSequential();
				case ElementOrder.Random:
					return GetRandom();
				case ElementOrder.RandomNotRepeat:
				default:
					return GetRandomNotRepeat();
			}
		}

		private SoundSingleElement? GetSequential()
		{
			if (SingleElements.Count == 0) return null;

			lastUsed++;
			while (lastUsed >= SingleElements.Count)
				lastUsed -= SingleElements.Count;
			return SingleElements[lastUsed];
		}

		private SoundSingleElement? GetRandom()
		{
			if (SingleElements.Count == 0) return null;

			lastUsed = GetRandomValue(SingleElements.Count);
			return SingleElements[lastUsed];
		}
		private SoundSingleElement? GetRandomNotRepeat()
		{
			if (SingleElements.Count == 0) return null;
			if (SingleElements.Count == 1) return SingleElements[0];

			int index;
			do
			{
				index = GetRandomValue(SingleElements.Count);
			}
			while (index == lastUsed);
			lastUsed = index;
			return SingleElements[index];
		}

		#region Random Generator(Injectable)

		private static int GetRandomValue()
		{
			return RandGen.GetValue();
		}
		private static int GetRandomValue(int maxNotIncluded)
		{
			return RandGen.GetValue(maxNotIncluded);
		}

		private static RandomGenerator RandGen;

		static SoundElement()
		{
			RandGen = new DefaultRandomGenerator();
		}
		public static void InjectRandomGenerator(RandomGenerator generator)
		{
			RandGen = generator;
		}
		#endregion
	}
	#region Random Generator(Injectable)

	public interface RandomGenerator
	{
		int GetValue();
		int GetValue(int maxNotIncluded);
	}
	internal class DefaultRandomGenerator : RandomGenerator
	{
		private System.Random rand = new System.Random();

		int RandomGenerator.GetValue()
		{
			return rand.Next();
		}
		int RandomGenerator.GetValue(int max)
		{
			return rand.Next(max);

		}
	}

	#endregion


	[System.Serializable]
	public class SoundSingleElement
	{
		public AudioClip? Clip;

		public float Volume = 1.0f;

		public bool ChangePitch = false;
		public Vector2 PitchRange = Vector2.one;

		public bool ChangePan = false;
		public Vector2 PanRange = Vector2.zero;
	}


}