using UnityEngine;

public class NodeGraphElement
{
	public virtual void Draw()
	{
	}

	public virtual bool ProcessEvents(Event e)
	{
		return false;
	}
}
