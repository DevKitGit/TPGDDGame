using UnityEngine.UIElements;

public interface IMenuHandler
{
	UI UI { get; set; }
	VisualElement Element { get; }
	void BindControls();
	void OnEnter();
	void OnExit();
}
