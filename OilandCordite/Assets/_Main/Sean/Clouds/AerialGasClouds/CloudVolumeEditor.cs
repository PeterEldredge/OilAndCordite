using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(CloudVolumeController))]
public class CloudVolumeEditor : Editor
{
    SerializedProperty drawSimulationSpace;
    SerializedProperty drawActivationRadius;

    bool toggle = false;

    void OnEnable()
    {
        drawSimulationSpace = serializedObject.FindProperty("drawSimulationSpace");
        drawActivationRadius = serializedObject.FindProperty("drawActivationRadius");
    }

    public override void OnInspectorGUI() 
    {
        GUI.backgroundColor = Color.red;
        if(GUILayout.Button("Toggle Cloud Volume"))
        {
            serializedObject.Update();
            drawSimulationSpace.boolValue = !drawSimulationSpace.boolValue;
            serializedObject.ApplyModifiedProperties();
        }

        GUI.backgroundColor = Color.green;
        if(GUILayout.Button("Toggle Activation Radius"))
        {
            serializedObject.Update();
            drawActivationRadius.boolValue = !drawActivationRadius.boolValue;
            serializedObject.ApplyModifiedProperties();
        }

        GUI.backgroundColor = new Color(0.8f,0.5f,0.5f,1.0f);
        if(GUILayout.Button("Toggle All Gas Cloud Gizmos"))
        {
            serializedObject.Update();
            CloudVolumeController[] objs = FindObjectsOfType<CloudVolumeController>();
            for(int i = 0; i < objs.Length; i++) 
            {
                if(toggle)
                    objs[i].DisableGizmos();
                else 
                    objs[i].EnableGizmos();
            }
            toggle = !toggle;
            serializedObject.ApplyModifiedProperties();
        }
    }
}
