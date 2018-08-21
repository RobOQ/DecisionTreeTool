using UnityEditor;

public class DecisionTreeTool : EditorWindow  {

	[MenuItem("Window/Decision Tree Tool")]
	private static void OpenWindow()
	{
		DecisionTreeTool dtt = GetWindow<DecisionTreeTool>();
		dtt.titleContent = new UnityEngine.GUIContent("Decision Tree Tool");
	}
}
