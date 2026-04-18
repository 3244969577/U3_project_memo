using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "SceneData")]
public class SceneData : ScriptableObject {
    public string startSceneName;
    public string mainGameSceneName;
}