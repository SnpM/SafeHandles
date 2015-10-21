# SafeHandles
Unity Handles that can be used from anywhere.

Example Usage:

```
//In Assets
public class VisualizeAttribute {}
public class Test : MonoBehaviour () {
    [Visualize]
    public Vector3 inSceneVector3; //Editable in scene
}

//In Editor
using UnityEditor;
using UnityEngine;
using SafeHandles;
[CustomPropertyDrawer (typeof (VisualizeAttribute))]
public class EditorVisualize : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        switch (property.propertyType) {
            case SerializedPropertyType.Vector3:
                property.vector3Value = EditorGUILayout.Vector3Field(label, property.vector3Value);
                property.vector3Value = HandlesHelper.PositionHandle (property.propertyPath, property.vector3Value, Quaternion.identity);
                property.serializedObject.ApplyModifiedProperties();
                break;
            default:
                throw new System.ArgumentException(string.Format("The visualization behavior of Type {0} is not implemented", property.propertyType));
                break;
        }
    }
}
```

Note: Under development.
