using UnityEngine;

public class SimpleNode : NodeGraphElement
{
	Vector2 position;
	float width;
	float height;
	public string title;

	public GUIStyle style;

	public SimpleNode(Vector2 pos, float w, float h, Vector2 offset, float scaleFactor, GUIStyle nodeStyle)
	{
		position.x = pos.x;
		position.y = pos.y;
		width = w;
		height = h;
		style = nodeStyle;
		ScaleFactor = scaleFactor;
		Offset = offset;
	}

	public override void Drag(Vector2 delta)
	{
		position += (delta * (1.0f / ScaleFactor));
	}

	public override void Draw()
	{
		float scaledWidth = width * ScaleFactor;
		float scaledHeight = height * ScaleFactor;
		Vector2 draggedPosition = (position * ScaleFactor) + Offset;

		Rect scaledRect = new Rect(draggedPosition.x, draggedPosition.y, scaledWidth, scaledHeight);
		GUI.Box(scaledRect, title, style);
	}

	public override bool ProcessEvents(Event e)
	{
		float scaledWidth = width * ScaleFactor;
		float scaledHeight = height * ScaleFactor;
		Vector2 draggedPosition = (position * ScaleFactor) + Offset;

		Rect scaledRect = new Rect(draggedPosition.x, draggedPosition.y, scaledWidth, scaledHeight);

		switch (e.type)
		{
			case EventType.MouseDown:
				{
					if (e.button == 0)
					{
						if (scaledRect.Contains(e.mousePosition))
						{
							IsDragged = true;
							GUI.changed = true;
						}
						else
						{
							GUI.changed = true;
						}
					}
					break;
				}
			case EventType.MouseUp:
				{
					IsDragged = false;
					break;
				}
			case EventType.MouseDrag:
				{
					if(e.button == 0 && IsDragged)
					{
						Drag(e.delta);
						e.Use();
						return true;
					}
					break;
				}
			default:
				break;
		}

		return false;
	}
}
