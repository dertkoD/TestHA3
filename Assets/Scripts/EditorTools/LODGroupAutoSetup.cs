using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class LODGroupAutoSetup : EditorWindow
{
    private Transform targetParent;
    private float largeCullPercent = 0.01f;
    private float mediumCullPercent = 0.02f;
    private float smallCullPercent = 0.05f;
    private float largeThreshold = 5f;
    private float smallThreshold = 1.5f;

    [MenuItem("Tools/Optimization/Add LOD Groups to Children")]
    static void ShowWindow()
    {
        GetWindow<LODGroupAutoSetup>("LOD Group Setup");
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("LOD Group Auto-Setup", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        targetParent = (Transform)EditorGUILayout.ObjectField(
            "Parent Object", targetParent, typeof(Transform), true);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Cull Screen Percentages", EditorStyles.boldLabel);

        largeCullPercent = EditorGUILayout.Slider(
            "Large Objects (walls/floors)", largeCullPercent, 0.001f, 0.1f);
        mediumCullPercent = EditorGUILayout.Slider(
            "Medium Objects (beds/barrels)", mediumCullPercent, 0.005f, 0.15f);
        smallCullPercent = EditorGUILayout.Slider(
            "Small Objects (boxes/decor)", smallCullPercent, 0.01f, 0.2f);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Size Thresholds (bounds extent)", EditorStyles.boldLabel);

        largeThreshold = EditorGUILayout.FloatField("Large > ", largeThreshold);
        smallThreshold = EditorGUILayout.FloatField("Small < ", smallThreshold);

        EditorGUILayout.Space();

        if (targetParent == null)
        {
            EditorGUILayout.HelpBox(
                "Drag the Castle (or any parent) from the Hierarchy into the field above.",
                MessageType.Info);
        }

        EditorGUI.BeginDisabledGroup(targetParent == null);

        if (GUILayout.Button("Add LOD Groups", GUILayout.Height(40)))
        {
            AddLODGroups();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Remove All LOD Groups from Children", GUILayout.Height(30)))
        {
            RemoveLODGroups();
        }

        EditorGUI.EndDisabledGroup();
    }

    void AddLODGroups()
    {
        if (targetParent == null) return;

        int added = 0;
        int skipped = 0;

        Undo.SetCurrentGroupName("Add LOD Groups");
        int undoGroup = Undo.GetCurrentGroup();

        for (int i = 0; i < targetParent.childCount; i++)
        {
            Transform child = targetParent.GetChild(i);

            if (child.GetComponent<LODGroup>() != null)
            {
                skipped++;
                continue;
            }

            Renderer[] renderers = child.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0)
            {
                skipped++;
                continue;
            }

            Bounds combinedBounds = renderers[0].bounds;
            for (int r = 1; r < renderers.Length; r++)
                combinedBounds.Encapsulate(renderers[r].bounds);

            float maxExtent = combinedBounds.extents.magnitude;

            float cullPercent;
            if (maxExtent >= largeThreshold)
                cullPercent = largeCullPercent;
            else if (maxExtent <= smallThreshold)
                cullPercent = smallCullPercent;
            else
                cullPercent = mediumCullPercent;

            LODGroup lodGroup = Undo.AddComponent<LODGroup>(child.gameObject);

            LOD[] lods = new LOD[1];
            lods[0] = new LOD(cullPercent, renderers);
            lodGroup.SetLODs(lods);
            lodGroup.RecalculateBounds();

            added++;
        }

        Undo.CollapseUndoOperations(undoGroup);

        Debug.Log($"[LOD Setup] Added LODGroups to {added} objects, skipped {skipped} " +
                  $"(already had LODGroup or no renderers)");

        EditorUtility.DisplayDialog("LOD Group Setup Complete",
            $"Added LOD Groups: {added}\nSkipped: {skipped}",
            "OK");
    }

    void RemoveLODGroups()
    {
        if (targetParent == null) return;

        int removed = 0;
        Undo.SetCurrentGroupName("Remove LOD Groups");
        int undoGroup = Undo.GetCurrentGroup();

        for (int i = 0; i < targetParent.childCount; i++)
        {
            LODGroup lodGroup = targetParent.GetChild(i).GetComponent<LODGroup>();
            if (lodGroup != null)
            {
                Undo.DestroyObjectImmediate(lodGroup);
                removed++;
            }
        }

        Undo.CollapseUndoOperations(undoGroup);

        Debug.Log($"[LOD Setup] Removed {removed} LODGroups");
        EditorUtility.DisplayDialog("LOD Groups Removed", $"Removed: {removed}", "OK");
    }
}
