using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(CombatButton), true)]
[CanEditMultipleObjects]
public class TestButtonEditor : ButtonEditor
{
    SerializedProperty m_OnSelectProperty;
    SerializedProperty m_OnDeselectProperty;
    protected override void OnEnable()
    {
        base.OnEnable();
        m_OnSelectProperty = serializedObject.FindProperty("m_OnSelect");
        m_OnDeselectProperty = serializedObject.FindProperty("m_OnDeselect");

    }
 
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        serializedObject.Update();
        EditorGUILayout.PropertyField(m_OnSelectProperty);
        EditorGUILayout.PropertyField(m_OnDeselectProperty);
        serializedObject.ApplyModifiedProperties();
    }
}