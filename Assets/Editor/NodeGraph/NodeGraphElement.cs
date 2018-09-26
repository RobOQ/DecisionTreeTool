using UnityEngine;

public class NodeGraphElement
{
	public float ScaleFactor { get; set; }
	public Vector2 Offset { get; set; }
	public bool IsDragged { get; set; }

	public virtual void Drag(Vector2 delta)
	{
	}

	public virtual void OffsetCanvas(Vector2 delta)
	{
		Offset += delta;
	}

	public virtual void Draw()
	{
	}

	public virtual bool ProcessEvents(Event e)
	{
		return false;
	}
}
