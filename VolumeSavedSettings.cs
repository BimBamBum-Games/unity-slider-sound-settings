using System;
using UnityEngine;

[Serializable]
public class VolumeSavedSettings {
    [field: SerializeField] public float Volume { get; set; }
    [field: SerializeField] public bool IsOn { get; set; }
    private VolumeSavedSettings() { }
    public static VolumeSavedSettings Get() { return new VolumeSavedSettings(); }
}
