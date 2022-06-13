using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace WizardsCode.AI
{
    [CustomEditor(typeof(EnemySpawner))]
    [CanEditMultipleObjects]
    public class EnemySpawnerInspector : Editor
    {
        const string EditorScriptsPath = "Assets/_BlazeNeo/Editor";

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement myInspector = new VisualElement();

            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{EditorScriptsPath}/AI/Spawner/SpawnerInspector.uxml");
            visualTree.CloneTree(myInspector);

            VisualElement inspectorFoldout = myInspector.Q("DefaultInspector");
            InspectorElement.FillDefaultInspector(inspectorFoldout, serializedObject, this);

            return myInspector;
        }
    }
}
