using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FloatingTextManager : MonoBehaviour
{
    //public static FloatingTextManager instance;
    public GameObject textPrefab;
    public GameObject canvas;
    private List<FloatingText> floatingTexts = new List<FloatingText>();

    public FloatingText Show(string msg, int fontSize, Color color, Vector3 position, Vector3 motion, float duration)
    {
        Debug.Log($"floatingShow: {msg}");
        FloatingText floatingText = this.GetFloatingText(canvas.transform);
        floatingText.Set(msg, fontSize, color, position, motion, duration);
        floatingText.Show();
        return floatingText;
    }

    private FloatingText GetFloatingText(Transform parentTransform = null)
    {
        FloatingText txt = new FloatingText();
        if (parentTransform != null)
        {
            txt.go = Instantiate(this.textPrefab, parentTransform);
        }
        else
        {
            txt.go = Instantiate(this.textPrefab, this.transform);
        }
        txt.txt = txt.go.GetComponent<Text>();
        // 将文本x和y轴反转180度
        // txt.go.transform.localEulerAngles = new Vector3(180, 180, 0);
        // 设置缩放参数
        txt.go.transform.localScale = new Vector3(0.4f, 0.2f, 0.4f);
        floatingTexts.Add(txt);
        return txt;
    }

    private void Update()
    {
        foreach (FloatingText txt in this.floatingTexts)
        {
            txt.UpdateFloatingText();
        }
    }
}
