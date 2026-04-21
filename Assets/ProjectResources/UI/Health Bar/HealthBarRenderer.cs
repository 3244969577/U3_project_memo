using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class HealthBarRenderer : MonoBehaviour
{
    public GameObject healthBarPrefab;
    public Damageable target;
    public Canvas canvas;
    private Slider slider;

    [Header("平滑渐变设置")]
    public float smoothSpeed = 5f; // 平滑过渡速度

    private GameObject healthBar;
    private float targetFillAmount;

    private void Start()
    {
        this.healthBar = Instantiate(healthBarPrefab, target.transform.position+Vector3.up*1.75f, target.transform.rotation);
        this.healthBar.transform.SetParent(canvas.transform);
        this.healthBar.transform.localScale = new Vector3(1.5f, 1, 1);

        slider = healthBar.GetComponent<Slider>();
        this.slider.minValue = 0;
        this.slider.maxValue = target.GetMaxHealth();
        this.slider.value = target.GetHealth();
        targetFillAmount = target.GetHealth();
    }

    private void Update()
    {
        float currentHealth = target.GetHealth();
        targetFillAmount = Math.Max(currentHealth, this.slider.minValue);
        
        // 平滑过渡血条填充量
        if (slider.value != targetFillAmount)
        {
            slider.value = Mathf.Lerp(slider.value, targetFillAmount, smoothSpeed * Time.deltaTime);
        }
        
        this.healthBar.transform.position = this.target.transform.position + Vector3.up * 1.75f;
    }

}
