using UnityEditor;
using UnityEngine;
using ScriptableObjects;
using System.Collections.Generic;

public class BoatMiniGameEditorWindow : EditorWindow
{
    private BoatMiniGame miniGame;
    private MiniGameTrack selectedTrack;
    private Vector2 scrollPosition;

    private const float TrackHeight = 80f;
    private const float NoteSize = 10f;
    private const float pixelsPerSecond = 100f;

    private MiniGameNote _draggedNote = null;
    private Vector2 _dragOffset;

    [MenuItem("Tools/Boat MiniGame Editor")]
    public static void ShowWindow()
    {
        GetWindow<BoatMiniGameEditorWindow>("Boat MiniGame Editor");
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();
        miniGame = EditorGUILayout.ObjectField("MiniGame Asset", miniGame, typeof(BoatMiniGame), false) as BoatMiniGame;
        if (miniGame == null) return;

        if (miniGame.GetTracks().Count == 0)
        {
            if (GUILayout.Button("Add Track"))
            {
                Undo.RecordObject(miniGame, "Add MiniGame Track");
                miniGame.GetTracks().Add(new MiniGameTrack { notes = new List<MiniGameNote>(), trackDuration = 5f });
                EditorUtility.SetDirty(miniGame);
            }
            return;
        }

        int lineCount = System.Enum.GetValues(typeof(MiniGameButton)).Length;

        if (Application.isPlaying && miniGame.GetCurrentTrack() != null)
        {
            selectedTrack = miniGame.GetCurrentTrack();
            EditorGUILayout.HelpBox("Showing currently playing track.", MessageType.Info);
        }
        else
        {
            int selectedIndex = Mathf.Max(0, miniGame.GetTracks().IndexOf(selectedTrack));
            selectedIndex = EditorGUILayout.Popup("Track", selectedIndex,
                miniGame.GetTracks().ConvertAll(t => "Track " + miniGame.GetTracks().IndexOf(t)).ToArray());
            selectedTrack = miniGame.GetTracks()[selectedIndex];
        }

        selectedTrack.trackDuration = EditorGUILayout.FloatField("Track Duration", selectedTrack.trackDuration);
        selectedTrack.trackDuration = Mathf.Max(1f, selectedTrack.trackDuration);

        EditorGUILayout.Space();
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        Rect trackRect = GUILayoutUtility.GetRect(selectedTrack.trackDuration * pixelsPerSecond, TrackHeight * lineCount);
        GUI.Box(trackRect, GUIContent.none);

        for (int line = 0; line < lineCount; line++)
        {
            float y = trackRect.y + line * TrackHeight + TrackHeight / 2f;
            Handles.color = new Color(0.5f, 0.5f, 0.5f, 0.4f);
            Handles.DrawLine(new Vector2(trackRect.x, y), new Vector2(trackRect.x + selectedTrack.trackDuration * pixelsPerSecond, y));
        }

        int seconds = Mathf.CeilToInt(selectedTrack.trackDuration);
        for (int i = 0; i <= seconds; i++)
        {
            float x = trackRect.x + i * pixelsPerSecond;
            Handles.color = Color.gray;
            Handles.DrawLine(new Vector2(x, trackRect.y), new Vector2(x, trackRect.y + TrackHeight * lineCount));
            GUI.Label(new Rect(x + 2, trackRect.y, 30, 20), $"{i}s", EditorStyles.miniLabel);
        }

        if (Application.isPlaying && miniGame.GetStartTime() != 0 && Time.time - miniGame.GetStartTime() <= selectedTrack.trackDuration)
        {
            float playTime = Time.time - miniGame.GetStartTime();
            float playHeadX = playTime * pixelsPerSecond;
            Handles.color = Color.red;
            Handles.DrawLine(
                new Vector2(trackRect.x + playHeadX, trackRect.y),
                new Vector2(trackRect.x + playHeadX, trackRect.y + TrackHeight * lineCount)
            );
        }

        Event e = Event.current;
        foreach (MiniGameNote note in selectedTrack.notes)
        {
            int line = (int)note.button;
            float x = trackRect.x + note.timeStamp * pixelsPerSecond;
            float y = trackRect.y + line * TrackHeight + TrackHeight / 2f;

            Rect noteRect = new Rect(x - NoteSize / 2f, y - NoteSize / 2f, NoteSize, NoteSize);
            EditorGUI.DrawRect(noteRect, note.isHit ? Color.green : Color.yellow);

            if (e.type == EventType.MouseDown && e.button == 1 && noteRect.Contains(e.mousePosition))
            {
                selectedTrack.notes.Remove(note);
                GUI.changed = true;
                e.Use();
                break;
            }

            if (e.type == EventType.MouseDown && e.button == 0 && noteRect.Contains(e.mousePosition))
            {
                _draggedNote = note;
                _dragOffset = e.mousePosition - new Vector2(x, y);
                e.Use();
            }

            if (_draggedNote == note && e.type == EventType.MouseDrag)
            {
                float newTime = (e.mousePosition.x - trackRect.x - _dragOffset.x) / pixelsPerSecond;
                newTime = Mathf.Clamp(newTime, 0f, selectedTrack.trackDuration);
                note.timeStamp = newTime;
                Repaint();
            }
        }

        foreach (MiniGameNote note in miniGame.hitNotes)
        {
            int line = (int)note.button;
            float x = trackRect.x + note.timeStamp * pixelsPerSecond;
            float y = trackRect.y + line * TrackHeight + TrackHeight / 2f;

            Rect noteRect = new Rect(x - NoteSize / 2f, y - NoteSize / 2f, NoteSize, NoteSize);
            EditorGUI.DrawRect(noteRect, new Color(1f, 0f, 1f, 0.5f));
        }

        if (e.type == EventType.MouseDown && e.button == 0 && trackRect.Contains(e.mousePosition) && _draggedNote == null)
        {
            Vector2 clickPos = e.mousePosition;
            int lineClicked = Mathf.FloorToInt((clickPos.y - trackRect.y) / TrackHeight);
            float timeClicked = (clickPos.x - trackRect.x) / pixelsPerSecond;

            if (lineClicked >= 0 && lineClicked < lineCount)
            {
                selectedTrack.notes.Add(new MiniGameNote
                {
                    button = (MiniGameButton)lineClicked,
                    timeStamp = Mathf.Clamp(timeClicked, 0, selectedTrack.trackDuration),
                    isHit = false
                });

                e.Use();
                Repaint();
            }
        }

        if (e.type == EventType.MouseUp)
        {
            _draggedNote = null;
        }

        EditorGUILayout.EndScrollView();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(miniGame);
        }

        if (Application.isPlaying)
        {
            Repaint();
        }
    }
}
