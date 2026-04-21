using UnityEngine;
using TMPro;

public class TipManager : MonoBehaviour
{
    public static TipManager instance;
    public static TipManager Instance => instance;
    public GameObject tipPrefab;

    void Awake()
    {
        instance = this;
    }

    public void ShowTip() {
        ShowTip("hello world");
    }

    // 全局调用：每次调用都会实例化一个新的提示框
    public void ShowTip(string message)
    {
        GameObject tip = Instantiate(tipPrefab, transform);
        tip.transform.position = new Vector3(300, -58, 0);
        tip.GetComponentInChildren<TextMeshProUGUI>().text = message;
    }
}