using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;

public class DecisionTreeTool : EditorWindow
{
	const float ZoomRate = (1.0f / 30.0f);
	const float MinZoom = 0.5f;
	const float MaxZoom = 2.0f;

	const float DefaultSimpleNodeWidth = 200.0f;
	const float DefaultSimpleNodeHeight = 50.0f;

	List<NodeGraphElement> elements;
	GUIStyle defaultStyle;

	Vector2 offset;
	Vector2 drag;
	float zoomLevel;

	[MenuItem("Window/Decision Tree Tool")]
	static void OpenWindow()
	{
		DecisionTreeTool dtt = GetWindow<DecisionTreeTool>();
		dtt.titleContent = new UnityEngine.GUIContent("Decision Tree Tool");
	}

	private void OnEnable()
	{
		zoomLevel = 1.0f;
		defaultStyle = new GUIStyle();
		defaultStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
		defaultStyle.border = new RectOffset(12, 12, 12, 12);
	}

	void OnGUI()
	{
		GUI.Label(new Rect(0.0f, 0.0f, 150.0f, 20.0f), "Zoom Level: " + (zoomLevel * 100.0f) + "%");
		GUI.Label(new Rect(0.0f, 15.0f, 150.0f, 20.0f), "drag: " + drag.ToString("F4"));
		GUI.Label(new Rect(0.0f, 30.0f, 200.0f, 20.0f), "offset: " + offset.ToString("F4"));
		GUI.Label(new Rect(0.0f, 45.0f, 200.0f, 20.0f), "mousePos: " + Event.current.mousePosition.ToString("F4"));

		DrawGrid(20f, 0.2f, Color.gray);
		DrawGrid(100f, 0.4f, Color.gray);

		Draw();
		ProcessElementEvents(Event.current);
		ProcessEvents(Event.current);

		if (GUI.changed)
		{
			Repaint();
		}
	}

	void DrawGrid(float gridSpacing, float gridOpacity, Color gridColour)
	{
		gridSpacing *= zoomLevel;

		int numAcross = Mathf.CeilToInt(position.width / gridSpacing);
		int numDown = Mathf.CeilToInt(position.width / gridSpacing);

		Handles.BeginGUI();
		Handles.color = new Color(gridColour.r, gridColour.g, gridColour.b, gridOpacity);

		// We're not accounting for zoom properly in this logic.
		// It always keeps the grid coordinate at 0,0 steady as we zoom in and out.
		// Ideally we'd keep either the centre of our current window view steady, or we'd
		// keep the coordinate under our mouse cursor steady.
		// TODO: Fix the zoom logic so it zooms in and out smoothly.
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
		Rect scaledRect = new Rect(0, 0, 10, 10);
		scaledRect.width *= zoomLevel;
		scaledRect.height *= zoomLevel;
		scaledRect.position = offset;
		GUI.Box(scaledRect, "");

		if (elements != null)
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
				{
					if (e.button == 1)
					{
						ProcessContextMenu(e.mousePosition);
					}
					break;
				}
			case EventType.MouseDrag:
				{
					if(e.button == 0)
					{
						OnDrag(e.delta);
					}
					break;
				}
			case EventType.ScrollWheel:
				{
					OnZoom(e.delta);

					break;
				}
			default:
				break;
		}
	}

	void ProcessElementEvents(Event e)
	{
		if(elements != null)
		{
			for(int i = elements.Count - 1; i >= 0; --i)
			{
				bool guiChanged = elements[i].ProcessEvents(e);

				if(guiChanged)
				{
					GUI.changed = true;
				}
			}
		}
	}

	void OnDrag(Vector2 delta)
	{
		drag = delta;

		if(elements != null)
		{
			for(int i = 0; i < elements.Count; ++i)
			{
				elements[i].Drag(delta);
			}
		}

		GUI.changed = true;
	}

	void OnZoom(Vector2 delta)
	{
		zoomLevel = Mathf.Clamp(zoomLevel - (ZoomRate * delta.y), MinZoom, MaxZoom);

		if (elements != null)
		{
			for (int i = 0; i < elements.Count; ++i)
			{
				elements[i].ScaleFactor = zoomLevel;
			}
		}

		GUI.changed = true;
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

		elements.Add(new SimpleNode(mousePosition, DefaultSimpleNodeWidth, DefaultSimpleNodeHeight, defaultStyle));
	}
}
