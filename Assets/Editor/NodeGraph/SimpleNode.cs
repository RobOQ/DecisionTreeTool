using UnityEngine;

public class SimpleNode : NodeGraphElement
{
	public Rect rect;
	public string title;

	public GUIStyle style;

	public SimpleNode(Vector2 position, float width, float height, GUIStyle nodeStyle)
	{
		rect = new Rect(position.x, position.y, width, height);
		style = nodeStyle;
		ScaleFactor = 1.0f;
	}

	public override void Drag(Vector2 delta)
	{
		// Changing the position "raw" like this will make zooming tricky.
		// Maybe better to store a "logical" position and a screen position?
		rect.position += delta;
	}

	public override void Draw()
	{
		// This simple scaling is not *quite* right.
		// It scales the size correctly, but not the position.
		// A more correct zoom will keep the rect's position steady relative to the "background".
		// In other words, some rectangles should (be able to) disappear off the screen when zooming in,
		// and should reappear when zooming out.
		// TODO: Fix this logic so zooming in and out works correctly.
		Rect scaledRect = new Rect(rect);
		scaledRect.width *= ScaleFactor;
		scaledRect.height *= ScaleFactor;
		GUI.Box(scaledRect, title, style);
	}

	public override bool ProcessEvents(Event e)
	{
		Rect scaledRect = new Rect(rect);
		scaledRect.width *= ScaleFactor;
		scaledRect.height *= ScaleFactor;

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
