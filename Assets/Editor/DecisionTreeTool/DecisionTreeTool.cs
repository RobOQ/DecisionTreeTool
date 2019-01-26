using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class DecisionTreeTool : EditorWindow
{
	class MouseButtons
	{
		// As used in mouse events in Unity GUI code.
		public static int Left { get; } = 0;
		public static int Right { get; } = 1;
		public static int Middle { get; } = 2;
	}

	static Vector2 MinWindowSize { get; } = new Vector2(600.0f, 300.0f);

	float ZoomRate { get; } = (1.0f / 30.0f);
	float MinZoom { get; } = 0.8f;
	float MaxZoom { get; } = 2.0f;
	float DefaultSimpleNodeWidth { get; } = 200.0f;
	float DefaultSimpleNodeHeight { get; } = 50.0f;
	float SidebarWidth { get; } = 300.0f;

	List<NodeGraphElement> elements;
	float zoomLevel = 1.0f;
	Rect zoomArea;
	Vector2 zoomCoordsOrigin = Vector2.zero;

	[MenuItem("Window/Decision Tree Tool")]
	static void OpenWindow()
	{
		DecisionTreeTool dtt = GetWindow<DecisionTreeTool>();
		dtt.minSize = MinWindowSize;
		dtt.titleContent = new UnityEngine.GUIContent("Decision Tree Tool");
	}

	void OnEnable()
	{
		zoomLevel = 1.0f;
		zoomArea = new Rect(SidebarWidth, 0.0f, position.width - SidebarWidth, position.height);
	}

	Vector2 ConvertScreenCoordsToZoomCoords(Vector2 screenCoords)
	{
		return (screenCoords - zoomArea.TopLeft()) / zoomLevel + zoomCoordsOrigin;
	}

	void DrawGraphCanvas()
	{
		// Within the zoom area all coordinates are relative to the top left corner of the zoom area
		// with the width and height being scaled versions of the original/unzoomed area's width and height.
		EditorZoomArea.Begin(zoomLevel, zoomArea);

		// Draw grid?
		// DrawGrid(20f, 0.2f, Color.gray);
		// DrawGrid(100f, 0.4f, Color.gray);

		DrawElements();

		EditorZoomArea.End();
	}

	void DrawSidebar()
	{
		GUI.Box(new Rect(0.0f, 0.0f, SidebarWidth, position.height), "Node Info");
	}

	void OnGUI()
	{
		zoomArea.width = position.width - 300.0f;
		zoomArea.height = position.height;

		ProcessElementEvents(Event.current);
		ProcessEvents(Event.current);

		DrawGraphCanvas();
		DrawSidebar();

		GUI.Label(new Rect(0.0f, 30.0f, 300.0f, 20.0f), "ZoomArea: " + zoomArea.x + ", " + zoomArea.y + ", " + zoomArea.width + ", " + zoomArea.height);
		GUI.Label(new Rect(0.0f, 50.0f, 300.0f, 20.0f), "MousePos: " + Event.current.mousePosition.x + ", " + Event.current.mousePosition.y);
		GUI.Label(new Rect(0.0f, 70.0f, 300.0f, 20.0f), "ZoomLevel: " + zoomLevel);
		GUI.Label(new Rect(0.0f, 90.0f, 300.0f, 20.0f), "ZoomCoordsOrigin: " + zoomCoordsOrigin);

		if (GUI.changed)
		{
			Repaint();
		}
	}

	// void DrawGrid(float gridSpacing, float gridOpacity, Color gridColour)
	// {
	// 	gridSpacing *= zoomLevel;

	// 	int numAcross = Mathf.CeilToInt(position.width / gridSpacing) + 1;
	// 	int numDown = Mathf.CeilToInt(position.height / gridSpacing) + 1;

	// 	Handles.BeginGUI();
	// 	Handles.color = new Color(gridColour.r, gridColour.g, gridColour.b, gridOpacity);

	// 	offset += drag * 0.5f;
	// 	Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);

	// 	for(int i = 0; i < numAcross; ++i)
	// 	{
	// 		Vector3 startPoint = new Vector3(gridSpacing * i, -gridSpacing, 0);
	// 		Vector3 offsetStartPoint = startPoint + newOffset;
	// 		Vector3 endPoint = new Vector3(gridSpacing * i, position.height + gridSpacing, 0f);
	// 		Vector3 offsetEndPoint = endPoint + newOffset;

	// 		Handles.DrawLine(offsetStartPoint, offsetEndPoint);
	// 	}

	// 	for(int i = 0; i < numDown; ++i)
	// 	{
	// 		Vector3 startPoint = new Vector3(-gridSpacing, gridSpacing * i, 0);
	// 		Vector3 offsetStartPoint = startPoint + newOffset;
	// 		Vector3 endPoint = new Vector3(position.width + gridSpacing, gridSpacing * i, 0f);
	// 		Vector3 offsetEndPoint = endPoint + newOffset;

	// 		Handles.DrawLine(offsetStartPoint, offsetEndPoint);
	// 	}

	// 	Handles.color = Color.white;
	// 	Handles.EndGUI();
	// }

	void DrawElements()
	{
		if (elements != null)
		{
			foreach (var element in elements)
			{
				element.Draw(zoomCoordsOrigin);
			}
		}
	}

	void ProcessEvents(Event e)
	{
		switch (e.type)
		{
			case EventType.MouseDown:
				{
					if (e.button == MouseButtons.Right)
					{
						// Should this maybe be on MouseUp instead?
						ProcessContextMenu(e.mousePosition);
					}
					break;
				}
			case EventType.MouseDrag:
				{
					if (e.button == MouseButtons.Middle)
					{
						OnDrag(e.delta);
						e.Use();
					}
					break;
				}
			case EventType.ScrollWheel:
				{
					OnZoom(e);
					e.Use();
					break;
				}

		}
	}

	void ProcessElementEvents(Event e)
	{
		if (elements != null)
		{
			for (int i = elements.Count - 1; i >= 0; --i)
			{
				bool guiChanged = elements[i].ProcessEvents(e, zoomArea.position, zoomLevel, zoomCoordsOrigin);

				if (guiChanged)
				{
					GUI.changed = true;
				}
			}
		}
	}

	void OnDrag(Vector2 delta)
	{
		delta *= -1.0f;
		delta /= zoomLevel;
		zoomCoordsOrigin += delta;

		GUI.changed = true;
	}

	void OnZoom(Event e)
	{
		Vector2 screenCoordsMousePos = e.mousePosition;
		Vector2 zoomCoordsMousePos = ConvertScreenCoordsToZoomCoords(screenCoordsMousePos);
		Vector2 delta = e.delta;
		float oldZoom = zoomLevel;
		zoomLevel = Mathf.Clamp(zoomLevel - (ZoomRate * delta.y), MinZoom, MaxZoom);
		zoomCoordsOrigin += (zoomCoordsMousePos - zoomCoordsOrigin) - (oldZoom / zoomLevel) * (zoomCoordsMousePos - zoomCoordsOrigin);

		GUI.changed = true;
	}

	void ProcessContextMenu(Vector2 mousePosition)
	{
		GenericMenu genericMenu = new GenericMenu();
		genericMenu.AddItem(new GUIContent("Add Simple Node"), false, () => OnClickAddNode(mousePosition));
		genericMenu.AddItem(new GUIContent("Add Zero Node"), false, () => OnClickAddZeroNode());
		genericMenu.ShowAsContext();
	}

	void OnClickAddNode(Vector2 mousePosition)
	{
		if (elements == null)
		{
			elements = new List<NodeGraphElement>();
		}

		elements.Add(new SimpleNode(ConvertScreenCoordsToZoomCoords(mousePosition), DefaultSimpleNodeWidth, DefaultSimpleNodeHeight));
	}

	void OnClickAddZeroNode()
	{
		if (elements == null)
		{
			elements = new List<NodeGraphElement>();
		}

		elements.Add(new SimpleNode(Vector2.zero, DefaultSimpleNodeWidth, DefaultSimpleNodeHeight));
	}
}
