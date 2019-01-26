using UnityEngine;

public class SimpleNode : NodeGraphElement
{
	Vector2 position;
	float width;
	float height;
	public string title;
	public string otherContent;

	public SimpleNode(Vector2 pos, float w, float h)
	{
		position.x = pos.x;
		position.y = pos.y;
		width = w;
		height = h;
		title = $"{position.x}, {position.y}, {width}, {height}";
	}

	public override void Drag(Vector2 delta, float zoomLevel)
	{
		position += (delta * (1.0f / zoomLevel));
		title = $"{position.x}, {position.y}, {width}, {height}";
	}

	public override void Draw(Vector2 zoomCoordsOrigin)
	{
		GUI.Box(new Rect(position.x - zoomCoordsOrigin.x, position.y - zoomCoordsOrigin.y, width, height), title);
		GUI.Label(new Rect(position.x - zoomCoordsOrigin.x, position.y - zoomCoordsOrigin.y + 20.0f, width, 20.0f), otherContent);
	}

	public override bool ProcessEvents(Event e, Vector2 zoomAreaPosition, float zoomLevel, Vector2 zoomCoordsOrigin)
	{
		// Instead of figuring out the scaled position and bounds of the rectangle, we could probably use
		// ConvertScreenCoordsToZoomCoords to convert the mousePosition into zoom coords and check it against the raw
		// Rect.
		// That's worth investigating as a way to make this code cleaner and less confusing.
		float scaledWidth = width * zoomLevel;
		float scaledHeight = height * zoomLevel;
		Vector2 scaledPosition = position * zoomLevel;

		Rect scaledRect = new Rect(scaledPosition.x, scaledPosition.y, scaledWidth, scaledHeight).Translate(zoomAreaPosition).Translate(-zoomCoordsOrigin * zoomLevel);

		otherContent = $"{scaledRect.position.x}, {scaledRect.position.y}, {scaledRect.width}, {scaledRect.height}";

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
						Drag(e.delta, zoomLevel);
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
