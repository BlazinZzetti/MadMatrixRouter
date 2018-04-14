using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TimeToFramesTool : EditorWindow
{
    int minA;
    int secA;
    int milA;

    int minB;
    int secB;
    int milB;

    float frames;

    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/TimeToFrames")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        TimeToFramesTool window = (TimeToFramesTool)EditorWindow.GetWindow(typeof(TimeToFramesTool));
        window.Show();
    }

    private void Update()
    {
        var totalMilliseconsA = (60000 * minA) + (1000 * secA) + (10 * milA);
        var totalMilliseconsB = (60000 * minB) + (1000 * secB) + (10 * milB);

        if (totalMilliseconsA > totalMilliseconsB)
        {
            var totalDiff = totalMilliseconsA - totalMilliseconsB;
            frames = (totalDiff / 1000f) * 60;
        }
        else //May also be equals in which case the result would always be 0.
        {
            var totalDiff = totalMilliseconsB - totalMilliseconsA;
            frames = (totalDiff / 1000f) * 60;
        }
    }

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Time A");
        minA = EditorGUILayout.DelayedIntField(minA);
        secA = EditorGUILayout.DelayedIntField(secA);
        milA = EditorGUILayout.DelayedIntField(milA);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Time B");
        minB = EditorGUILayout.DelayedIntField(minB);
        secB = EditorGUILayout.DelayedIntField(secB);
        milB = EditorGUILayout.DelayedIntField(milB);
        EditorGUILayout.EndHorizontal();
     
        EditorGUILayout.FloatField("Frames", frames);
    }
}
