using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;


[CreateAssetMenu(fileName = "New Upgrade", menuName = "New Upgrade")]
public class Upgrades : ScriptableObject
{

    public string Name;
    public string InvokeMethod;
    [TextArea]
    public string Description;
    public Sprite Artwork;
    public int Cost;
    public bool Acquired = false;
    public GameObject SpriteRenderedObject;

    public void IsAcquired()
    {
        Acquired = true;
    }

    public void Awake()
    {
#if UNITY_EDITOR
        EditorApplication.playModeStateChanged += ModeChanged;
#endif
    }

#if UNITY_EDITOR
    void ModeChanged(PlayModeStateChange playModeState)
    {
        if (playModeState == PlayModeStateChange.EnteredEditMode)
        {
            Acquired = false;
            Debug.Log("acquired false");
        }
    }
#endif
}
