using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

namespace radiants.SimpleSoundSuite
{
	public class JukeboxEditor : EditorWindow
	{
		[MenuItem("Window/SimpleSoundSuite/Open Jukebox Editor")]
		private static void OpenWindow()
		{
			var window = GetWindow<JukeboxEditor>();
			window.titleContent = new GUIContent("Jukebox Editor");
		}



		private Jukebox CurrentTarget;
		private Vector2 ScrollPosition;

		//UIElementsは面倒そうなので不使用
		private void OnGUI()
		{
			CurrentTarget = EditorGUILayout.ObjectField("Target", CurrentTarget, typeof(Jukebox), false) as Jukebox;

			if (CurrentTarget == null) return;

			//区切り線
			GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(5));

			ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition);


			EditorGUILayout.LabelField("Jukebox Editor");

			List<System.Action> OnChangedActions = new List<System.Action>();
			for (int i = 0; i < CurrentTarget.Elements.Count; i++)
			{
				EditorGUILayout.BeginVertical("box");
				var action = DrawAudioElement(CurrentTarget.Elements[i], () => CurrentTarget.Elements.RemoveAt(i));
				if(action != null)
					OnChangedActions.Add(action);
				EditorGUILayout.EndVertical();
			}

			if (GUI.changed)
			{
				Undo.RecordObject(CurrentTarget, "Change Jukebox Parameter");
				OnChangedActions.ForEach(a => a.Invoke());
				EditorUtility.SetDirty(CurrentTarget);
			}

			//D&DでAudioElementを新規追加できる
			EditorGUILayout.BeginHorizontal();
			if(GUILayout.Button("+", GUILayout.Height(60f)))
			{
				Undo.RecordObject(CurrentTarget, "Add Jukebox Element");

				SoundElement elem = new SoundElement();
				elem.ID = (long)Random.Range(0, int.MaxValue) * (long)Random.Range(0, int.MaxValue);
				//todo idかぶり対策
				elem.Name = elem.ID.ToString();
				CurrentTarget.Elements.Add(elem);
				EditorUtility.SetDirty(CurrentTarget);
			}
			var rect = GUILayoutUtility.GetRect(60f, 60f, GUILayout.ExpandWidth(true));
			DrawFileDragArea<AudioClip>(rect, "Drop AudioClip(s)\nto Make new SoundElement.", clips => 
			{
				Undo.RecordObject(CurrentTarget, "Add Jukebox Element");

				SoundElement elem = new SoundElement();
				elem.ID = (long)Random.Range(0, int.MaxValue) * (long)Random.Range(0, int.MaxValue);
				elem.Name = clips[0].name;
				elem.Order = ElementOrder.RandomNotRepeat;
				//todo 名前かぶり対策
				elem.SingleElements = new List<SoundSingleElement>();
				for (int i = 0; i < clips.Length; i++)
				{
					SoundSingleElement singleElem = new SoundSingleElement();
					singleElem.Volume = 1.0f;
					singleElem.Clip = clips[i];
					elem.SingleElements.Add(singleElem);
				}
				CurrentTarget.Elements.Add(elem);
				EditorUtility.SetDirty(CurrentTarget);
			});
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.EndScrollView();
		}

		private System.Action DrawAudioElement(SoundElement TargetElement, System.Action deleteAction)
		{
			//開閉
			TargetElement.FoldShown = EditorGUILayout.BeginFoldoutHeaderGroup(TargetElement.FoldShown, TargetElement.Name);

			if (!TargetElement.FoldShown)
			{
				EditorGUILayout.EndFoldoutHeaderGroup();
				return null;
			}

			string tmpName = TargetElement.Name;
			long tmpID = TargetElement.ID;
			ElementOrder tmpOrder = TargetElement.Order;

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.BeginVertical();

			tmpName = EditorGUILayout.TextField("Name", tmpName);
			tmpID = EditorGUILayout.LongField("ID", tmpID);

			tmpOrder = (ElementOrder)EditorGUILayout.EnumPopup("Order", tmpOrder);

			List<System.Action> changedActions = new List<System.Action>();
			for (int i = 0; i < TargetElement.SingleElements.Count; i++)
			{
				EditorGUILayout.BeginVertical("box");
				changedActions.Add(DrawAudioSingleElement(TargetElement.SingleElements[i], () => TargetElement.SingleElements.RemoveAt(i)));
				EditorGUILayout.EndVertical();
			}

			//追加ボタン
			EditorGUILayout.BeginHorizontal();
			if(GUILayout.Button("+", GUILayout.Height(40f)))
			{
				Undo.RecordObject(CurrentTarget, "Add Jukebox Clip");
				TargetElement.SingleElements.Add(new SoundSingleElement());
				EditorUtility.SetDirty(CurrentTarget);
			}
			var rect = GUILayoutUtility.GetRect(20f, 40f);
			//D&Dで追加
			DrawFileDragArea<AudioClip>(rect, "Drop AudioClip\nto add Clip", clip =>
			{
				Undo.RecordObject(CurrentTarget, "Add Jukebox Clip");
				SoundSingleElement elem = new SoundSingleElement();
				elem.Clip = clip[0];
				TargetElement.SingleElements.Add(elem);
				EditorUtility.SetDirty(CurrentTarget);
			});
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.EndHorizontal();
			//削除ボタン
			if(GUILayout.Button("Delete\nElement"))
			{
				Undo.RecordObject(CurrentTarget, "Delete Jukebox Element");
				deleteAction.Invoke();
				EditorUtility.SetDirty(CurrentTarget);
			}
			EditorGUILayout.EndVertical();

			EditorGUILayout.EndFoldoutHeaderGroup();

			return () =>
			{
				TargetElement.Name = tmpName;
				TargetElement.ID = tmpID;
				TargetElement.Order = tmpOrder;
				changedActions.ForEach(a => a.Invoke());
			};
		}
		private System.Action DrawAudioSingleElement(SoundSingleElement TargetSE, System.Action deleteAction)
		{
			float tmpVolume = TargetSE.Volume;
			AudioClip tmpClip = TargetSE.Clip;
			bool tmpChangePitch = TargetSE.ChangePitch;
			float pitchMin = TargetSE.PitchRange.x;
			float pitchMax = TargetSE.PitchRange.y;
			bool tmpChangePan = TargetSE.ChangePan;
			float panMin = TargetSE.PanRange.x;
			float panMax = TargetSE.PanRange.y;

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.BeginVertical();
			tmpClip = EditorGUILayout.ObjectField("Clip", tmpClip, typeof(AudioClip), false) as AudioClip;
			tmpVolume = EditorGUILayout.Slider("Volume", tmpVolume, 0f, 1f);

			#region Pitch
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Change Pitch", GUILayout.Width(100f));
			tmpChangePitch = EditorGUILayout.Toggle(tmpChangePitch, GUILayout.Width(46f));
			if(tmpChangePitch)
			{
				EditorGUILayout.MinMaxSlider(ref pitchMin, ref pitchMax, 0.1f, 3.0f);
				pitchMin = EditorGUILayout.FloatField(pitchMin, GUILayout.Width(60f));
				pitchMax = EditorGUILayout.FloatField(pitchMax, GUILayout.Width(60f));
			}
			EditorGUILayout.EndHorizontal();
			#endregion

			#region Pan
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Change Pan", GUILayout.Width(100f));
			tmpChangePan = EditorGUILayout.Toggle(tmpChangePan, GUILayout.Width(46f));
			if(tmpChangePan)
			{
				EditorGUILayout.MinMaxSlider(ref panMin, ref panMax, -1f, 1f);
				panMin = EditorGUILayout.FloatField(panMin, GUILayout.Width(60f));
				panMax = EditorGUILayout.FloatField(panMax, GUILayout.Width(60f));
			}
			EditorGUILayout.EndHorizontal();
			#endregion

			EditorGUILayout.EndVertical();

			if (GUILayout.Button("Delete\nClip", GUILayout.Width(60f)))
			{
				Undo.RecordObject(CurrentTarget, "Delete Jukebox Clip");
				deleteAction.Invoke();
				EditorUtility.SetDirty(CurrentTarget);
			}
			EditorGUILayout.EndHorizontal();

			return () =>
			{
				TargetSE.Volume = tmpVolume;
				TargetSE.Clip = tmpClip;
				TargetSE.ChangePitch = tmpChangePitch;
				TargetSE.PitchRange = new Vector2(pitchMin, pitchMax);
				TargetSE.ChangePan = tmpChangePan;
				TargetSE.PanRange = new Vector2(panMin, panMax);
			};
		}


		#region Drag and Drop

		private void DrawFileDragArea<T>(
			Rect dropArea,
			string dropAreaMessage,
			System.Action<T[]> dropCallback,
			DragAndDropVisualMode visualMode = DragAndDropVisualMode.Generic)
			where T : UnityEngine.Object
		{
			Event evt = Event.current;
			GUI.Box(dropArea, dropAreaMessage);

			switch (evt.type)
			{
				// ドラッグ中.
				case EventType.DragUpdated:
					if (!dropArea.Contains(evt.mousePosition)) break;

					bool convertible = CheckConVertible<T>(DragAndDrop.objectReferences);
					if (convertible)
						DragAndDrop.visualMode = visualMode;
					else
						DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
					break;


				// ドラッグされた
				case EventType.DragPerform:
					if (!dropArea.Contains(evt.mousePosition)) break;
					if (!CheckConVertible<T>(DragAndDrop.objectReferences)) break;

					// オブジェクトを受け入れる.
					DragAndDrop.AcceptDrag();
					var objects = ConvertObjectsArray<T>(DragAndDrop.objectReferences);
					dropCallback.Invoke(objects);

					DragAndDrop.activeControlID = 0;
					Event.current.Use();
					break;
			}
		}
		private bool CheckConVertible<T>(UnityEngine.Object[] origin)
		{
			for (int i = 0; i < origin.Length; i++)
			{
				if (origin[i] is T)
					return true;
			}
			return false;
		}
		private T[] ConvertObjectsArray<T>(UnityEngine.Object[] origin)
			where T : UnityEngine.Object
		{
			List<T> ret = new List<T>();
			for (int i = 0; i < origin.Length; i++)
			{
				var obj = origin[i];
				if (obj is T)
					ret.Add(obj as T);
			}
			return ret.ToArray();
		}

		#endregion
	}
}