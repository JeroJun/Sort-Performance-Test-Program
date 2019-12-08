using UnityEngine;
using UnityEditor; //유니티 에디터를 사용합니다.

[CustomEditor(typeof(Sort))] //여기에서 커스텀 에디터를 붙이기 위한 스크립트를 지정합니다.
public class InspectorTest : Editor
{    // 모노가 아니라 Editor입니다.

    public override void OnInspectorGUI()   //OnInspectorGUI 에 오버라이드 해 줍니다.
    {
        base.OnInspectorGUI();

        Sort instance = target as Sort;

        if (GUILayout.Button("Reset"))
        {
            instance.UpdateGraph();
        }
    }
}