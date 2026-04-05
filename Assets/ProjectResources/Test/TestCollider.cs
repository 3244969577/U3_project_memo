using UnityEngine;

public class TestCollider : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("TestCollider Start");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 3D
    private void OnTriggerEnter(Collider other) {
        Debug.Log($"Trigger 3D 进入: {other.gameObject.name}, Tag: {other.tag}");
    }

    // 2D
    private void OnTriggerEnter2D(Collider2D other) {
        Debug.Log($"Trigger 2D 进入: {other.gameObject.name}, Tag: {other.tag}");
    }
}
