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
	}

	public void Drag(Vector2 delta)
	{
		rect.position += delta;
	}

	public override void Draw()
	{
		GUI.Box(rect, title, style);
	}

	public override bool ProcessEvents(Event e)
	{
		return false;
	}
}
