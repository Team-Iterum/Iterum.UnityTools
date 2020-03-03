#if UNITY_EDITOR
using UnityEditor;

namespace EasyButtons
{
    /// <summary>
    /// Custom inspector for Object including derived classes.
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UnityEngine.Object), true)]
    public class ObjectEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            this.DrawEasyButtons();
            
            if (target == null) return;
            
            // Draw the rest of the inspector as usual
            DrawDefaultInspector();
        }
    }
}

#endif