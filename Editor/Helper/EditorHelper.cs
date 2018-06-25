using MeshEditor.Editor.Abstract;
using UnityEditor;
using UnityEngine;

namespace MeshEditor.Editor.Helper
{
    public static class EditorHelper
    {
        public static void MakeDirty(Object _target, bool force = false)
        {
            if (_target != null && (force || GUI.changed))
            {
#if UNITY_5_3 || UNITY_5_4 || UNITY_5_3_OR_NEWER
                if (!Application.isPlaying && PrefabUtility.GetPrefabType(_target) != PrefabType.Prefab)
                {
                    if (_target is MonoBehaviour)
                    {
                        MonoBehaviour monoBehaviour = (MonoBehaviour)_target;
                        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(monoBehaviour.gameObject.scene);
                    }
                    else
                    {
                        UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
                    }
                }
#endif
                EditorUtility.SetDirty(_target);
            }
        }

        public static AbstractVectorListEditor activeListEditor;

        public static void LaunchRedoUndo()
        {
            activeListEditor.ThrowUndoRedo();
        }

    }
}