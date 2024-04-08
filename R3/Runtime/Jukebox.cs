using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace radiants.SimpleSoundSuite
{

	[CreateAssetMenu(menuName = "radiants/SimpleSoundSuite/Make Jukebox ScriptableObject")]

	public class Jukebox : ScriptableObject
	{
		[SerializeField]
		public List<SoundElement> Elements = new List<SoundElement>();


		private Dictionary<string, SoundElement> NameDict = null;
		private Dictionary<long, SoundElement> IDDict = null;
		//prepare index
		public void MakeIndex()
		{
			NameDict = new Dictionary<string, SoundElement>();
			IDDict = new Dictionary<long, SoundElement>();

			for (int i = 0; i < Elements.Count; i++)
			{
				var element = Elements[i];
				if (NameDict.ContainsKey(element.Name))
				{
					Debug.LogWarning("Duplicate Element Name:" + element.Name);
				}
				else
				{
					NameDict.Add(element.Name, element);
				}

				if (IDDict.ContainsKey(element.ID))
				{
					Debug.LogWarning("Duplicate Element ID:" + element.ID);
				}
				else
				{
					IDDict.Add(element.ID, element);
				}
			}
		}


		public SoundElement GetElementByName(string name)
		{
			if (NameDict == null)
				MakeIndex();

			return NameDict.GetValueOrDefault(name, null);
		}

		public SoundElement GetElementByID(long id)
		{
			if (IDDict == null)
				MakeIndex();
			return IDDict.GetValueOrDefault(id, null);
		}

	}
}