using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Globalization;

public class FlashText : MonoBehaviour
{
    public TextMeshProUGUI pressText;

    void Update()
    {
        // 文字闪烁
        // pressText.enabled = Mathf.Repeat(Time.time, 1f) > 0.5f;
        pressText.gameObject.SetActive(Mathf.Repeat(Time.time, 1f) > 0.5f);
    }
}
