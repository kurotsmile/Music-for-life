// Created by Carlos Arturo Rodriguez Silva (Legend) https://twitter.com/xLegendx97 or https://www.facebook.com/legendxh
// Thread: http://forum.unity3d.com/threads/rhythm-visualizator.423168/ 
// Video: https://www.youtube.com/watch?v=i5uRU45fi8U

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RhythmVisualizator))]
public class RhythmVisualizatorEditor : Editor
{
	public override void OnInspectorGUI()
	{
		var rhythmVisualizator = (RhythmVisualizator)target;

		if (GUILayout.Button ("Update BPM / Off-set")) {
			rhythmVisualizator.MSDelay ();
		}
		if (GUILayout.Button ("Tap BPM (2 sec to reset)")) {
			rhythmVisualizator.TapBPM ();
		}

		if (EditorApplication.isPlaying) {
			if (DrawDefaultInspector ()) {
				rhythmVisualizator.UpdateScript ();
			}

		} else {
			DrawDefaultInspector ();
		}
	}
}