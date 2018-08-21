using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;

public class DecisionTreeTool : EditorWindow
{
	List<NodeGraphElement> elements;
	GUIStyle defaultStyle;

	Vector2 offset;
	Vector2 drag;

	[MenuItem("Window/Decision Tree Tool")]
	static void OpenWindow()
	{
		DecisionTreeTool dtt = GetWindow<DecisionTreeTool>();
		dtt.titleContent = new UnityEngine.GUIContent("Decision Tree Tool");
	}

	private void OnEnable()
	{
		defaultStyle = new GUIStyle();
		defaultStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
		defaultStyle.border = new RectOffset(12, 12, 12, 12);
	}

	void OnGUI()
	{
		DrawGrid(20f, 0.2f, Color.gray);
		DrawGrid(100f, 0.4f, Color.gray);

		Draw();
		ProcessEvents(Event.current);

		if (GUI.changed)
		{
			Repaint();
		}
	}

	void DrawGrid(float gridSpacing, float gridOpacity, Color gridColour)
	{
		int numAcross = Mathf.CeilToInt(position.width / gridSpacing);
		int numDown = Mathf.CeilToInt(position.width / gridSpacing);

		Handles.BeginGUI();
		Handles.color = new Color(gridColour.r, gridColour.g, gridColour.b, gridOpacity);

		offset += drag * 0.5f;
		Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);

		for(int i = 0; i < numAcross; ++i)
		{
			Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
		}

		for(int i = 0; i < numDown; ++i)
		{
			Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * i, 0) + newOffset, new Vector3(position.width, gridSpacing * i, 0f) + newOffset);
		}

		Handles.color = Color.white;
		Handles.EndGUI();
	}

	void Draw()
	{
		if(elements != null)
		{
			foreach(var element in elements)
			{
				element.Draw();
			}
		}
	}

	void ProcessEvents(Event e)
	{
		drag = Vector2.zero;

		switch(e.type)
		{
			case EventType.MouseDown:
				if(e.button == 1)
				{
					ProcessContextMenu(e.mousePosition);
				}
				break;
			default:
				break;
		}
	}

	void ProcessContextMenu(Vector2 mousePosition)
	{
		GenericMenu genericMenu = new GenericMenu();
		genericMenu.AddItem(new GUIContent("Add Simple Node"), false, () => OnClickAddNode(mousePosition));
		genericMenu.ShowAsContext();
	}

	void OnClickAddNode(Vector2 mousePosition)
	{
		if(elements == null)
		{
			elements = new List<NodeGraphElement>();
		}

		elements.Add(new SimpleNode(mousePosition, 200, 50, defaultStyle));
	}
}
