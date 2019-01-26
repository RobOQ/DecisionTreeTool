using UnityEngine;

public class NodeGraphElement
{
	public bool IsDragged { get; set; }

	public virtual void Drag(Vector2 delta, float zoomLevel)
	{
	}

	public virtual void Draw(Vector2 zoomCoordsOrigin)
	{
	}

	public virtual bool ProcessEvents(Event e, Vector2 zoomAreaPosition, float zoomLevel, Vector2 zoomCoordsOrigin)
	{
		return false;
	}
}
