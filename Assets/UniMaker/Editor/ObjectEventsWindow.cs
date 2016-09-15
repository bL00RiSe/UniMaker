﻿using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UniMaker.Actions;

namespace UniMaker
{
    public class ObjectEventsWindow : EditorWindow
	{
		private const int itemSize = 20;
		private const int actionItemSize = 24;
		private const float doubleClickTime = 0.3f;

		private Vector2 scrollValue = Vector2.zero;
		private List<UniEvent> eventList;
		private int? lastSelectedIndex = null;
		private double lastClickTime = 0;

		private ReorderableList list;

        private enum ParsingMode { USINGS, CLASS, ENDCLASS, EVENT, ENDEVENT };

        [MenuItem("UniMaker/Events Inspector")]
		static void Init()
		{
			ObjectEventsWindow wnd = EditorWindow.GetWindow<ObjectEventsWindow>("Events");
			wnd.OnSelectionChange();
			wnd.Show();
		}

		void OnSelectionChange()
		{
            if (Selection.activeObject is MonoScript)
            {
                eventList = new List<UniEvent>();

                MonoScript sel = (Selection.activeObject as MonoScript);

                StringReader strReader = new StringReader(sel.text);
                string currentLine = null;
                string eventContent = "";
                string eventOptions = null;
                ParsingMode mode = ParsingMode.USINGS;

                while((currentLine = strReader.ReadLine()) != null)
                {
                    currentLine = currentLine.TrimStart(new char[] { ' ', '\t' });

                    if (currentLine.StartsWith("//USINGS")) { mode = ParsingMode.USINGS; continue; }
                    if (currentLine.StartsWith("//CLASS")) { mode = ParsingMode.CLASS; continue; }
                    if (currentLine.StartsWith("//ENDCLASS")) { mode = ParsingMode.ENDCLASS; continue; }
                    if (currentLine.StartsWith("//EVENT"))
                    {
                        mode = ParsingMode.EVENT;
                        eventOptions = currentLine.Substring(currentLine.IndexOf('%') + 1);
                        continue;
                    }
                    if (currentLine.StartsWith("//ENDEVENT")) { mode = ParsingMode.ENDEVENT; continue; }

                    switch (mode)
                    {
                        case ParsingMode.USINGS:
                            //Add usings
                            break;
                        case ParsingMode.CLASS:
                            //save class content mb? idk
                            break;
                        case ParsingMode.EVENT:
                            eventContent += currentLine + "\n";
                            break;
                        case ParsingMode.ENDEVENT:
                            eventList.Add(new UniEvent(eventOptions, eventContent));
                            eventContent = "";
                            break;
                    }
                }
                strReader.Close();
            }
            
			Repaint();
		}
		
		void OnGUI()
		{
            /*
			if (selectedObject == null)
			{
				DrawLabelInCenter("Select any GMakerObject");
				return;
			}

			EditorGUILayout.BeginHorizontal();

			EditorGUILayout.BeginVertical(GUILayout.Width(150));
			EditorGUILayout.LabelField("Events:");
			scrollValue = EditorGUILayout.BeginScrollView(scrollValue, GUI.skin.box);
			if (selectedObject.Events.Count == 0)
			{
				DrawLabelInCenter("No events here!\nAdd any?");
			}
			for (int i = 0; i < selectedObject.Events.Count; i++)
			{
				DrawEvent(IconCacher.GetIcon<EventTypes>(selectedObject.Events[i].Type), selectedObject.Events[i].Type.ToString(), i);
			}
			EditorGUILayout.EndScrollView();
			if (GUILayout.Button("Add event"))
			{
				selectedObject.Events.Add(new GMakerObject.EventInstance(EventTypes.EventMouse));
				SelectEvent(selectedObject.Events.Count - 1);
				SetObjectDirty();
			}
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Delete"))
			{
				selectedObject.Events.RemoveAt(selectedObject.SelectedEventIndex);
				if (selectedObject.SelectedEventIndex > 0) { selectedObject.SelectedEventIndex--; }
				if (selectedObject.Events.Count > 0)
				{
					SelectEvent(selectedObject.SelectedEventIndex);
				}
				SetObjectDirty();
			}
			GUILayout.Button("Change");
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();

			Rect dropArea = EditorGUILayout.BeginVertical();
			EditorGUILayout.LabelField("Actions:");

			if (dropArea.Contains(Event.current.mousePosition))
			{
				object data = null;
				switch(Event.current.type)
				{
					case EventType.DragUpdated:
						data = DragAndDrop.GetGenericData("ActionTypes");
	                    if (data is ActionTypes)
						{
							DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
						}
						break;

					case EventType.DragPerform:
						data = DragAndDrop.GetGenericData("ActionTypes");
						if (data is ActionTypes && (selectedObject.Events.Count > 0))
						{
							DragAndDrop.AcceptDrag();
							selectedObject.SelectedEvent.Actions.Add(GetActionInstanceByType((ActionTypes)data));
							DragAndDrop.SetGenericData("ActionTypes", null);
							SetObjectDirty();
						}
						break;
				}
			}
			
			if (selectedObject.Events.Count == 0)
			{
				DrawLabelInCenter("No event selected");
			}
			else
			{
				if (list == null)
				{
					SelectEvent(selectedObject.SelectedEventIndex);
				}
				list.DoLayoutList();
			}
			EditorGUILayout.EndVertical();

			EditorGUILayout.EndHorizontal();
            */
		}

		private void DrawEvent(Texture icon, string eventName, int index)
		{
            /*
			bool active = index == selectedObject.SelectedEventIndex;

			Color prevColor = GUI.backgroundColor;
			GUI.backgroundColor = active ? Color.blue : prevColor;

			Rect area = EditorGUILayout.BeginHorizontal(GUILayout.MaxHeight(itemSize));
			if (EditorGUI.Toggle(area, active, GUI.skin.box) && !active)
			{
				SelectEvent(index);
			}
			EditorGUILayout.Space();
			GUILayout.Label(icon, GUILayout.Width(itemSize), GUILayout.Height(itemSize));
			EditorGUILayout.BeginVertical(GUILayout.MaxHeight(itemSize));
			GUILayout.FlexibleSpace();
			EditorGUILayout.LabelField(eventName, GUILayout.MinWidth(130));
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndVertical();
			GUILayout.FlexibleSpace();
	        EditorGUILayout.EndHorizontal();

			GUI.backgroundColor = prevColor;
            */
		}

		private void SelectEvent(int selectIndex)
		{
            /*
			selectedObject.SelectedEventIndex = selectIndex;

			list = new ReorderableList(selectedObject.SelectedEvent.Actions, typeof(ActionBase));
			list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
			{
				ActionBase element = selectedObject.SelectedEvent.Actions[index];
				rect.y -= 1;
				EditorGUI.LabelField(new Rect(rect.x, rect.y, actionItemSize, actionItemSize), new GUIContent(IconCacher.GetIcon<ActionTypes>(element.Type)));
				rect.y += 3;
				EditorGUI.LabelField(new Rect(rect.x + actionItemSize, rect.y, rect.width - actionItemSize, actionItemSize), element.TextInList);
			};
			list.onAddCallback = (l) =>
			{
				ActionsSelectWindow wnd = EditorWindow.GetWindow<ActionsSelectWindow>("Actions");
				wnd.Show();
			};
			list.drawHeaderCallback = (Rect rect) =>
			{
				GUI.Label(rect, "Drag actions here");
			};
			list.onSelectCallback = (x) =>
			{
				if (lastSelectedIndex == x.index)
				{
					if ((EditorApplication.timeSinceStartup - lastClickTime) < doubleClickTime)
					{
						SetPropertiesWindow.Open(selectedObject.SelectedEvent.Actions[x.index]);
					}
				}

				lastSelectedIndex = x.index;
				lastClickTime = EditorApplication.timeSinceStartup;
			};
			list.onChangedCallback = (l) => { SetObjectDirty(); };
            */
		}

		internal void SetObjectDirty()
		{
            /*
			if (selectedObject != null)
			{
				selectedObject.SaveDataToJSON();
				EditorUtility.SetDirty(selectedObject);
			}
            */
		}

		private static void DrawLabelInCenter(string text)
		{
			EditorGUILayout.BeginVertical();
			GUILayout.FlexibleSpace();
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			EditorGUILayout.LabelField(text, GUI.skin.box);
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndVertical();
		}

		public static ActionBase GetActionInstanceByType(ActionTypes type)
		{
			return (ActionBase)Activator.CreateInstance("Assembly-CSharp", "UniMaker.Action.Action" + type.ToString()).Unwrap();
			/*ScriptableObject instanceToReturn = null;
			if (AssetDatabase.FindAssets("Action" + type.ToString()).Length > 0)
			{
				instanceToReturn = ScriptableObject.CreateInstance("Action" + type.ToString());
			}
			if (instanceToReturn != null && instanceToReturn is ActionBase)
			{
				return (ActionBase)instanceToReturn;
			}
			instanceToReturn = ScriptableObject.CreateInstance<ActionBase>();
			((ActionBase)instanceToReturn).Type = ActionTypes.None;
			((ActionBase)instanceToReturn).TextInList = "NOT DEFINED";
			return (ActionBase)instanceToReturn;*/
		}
	}
}