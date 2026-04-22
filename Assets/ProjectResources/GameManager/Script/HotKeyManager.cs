using UnityEngine;

public class HotKeyManager : Singleton<HotKeyManager>
{
    [SerializeField] private HotKeySetting hotKeySetting;
    public HotKeySetting HotKeySetting => hotKeySetting;
}
