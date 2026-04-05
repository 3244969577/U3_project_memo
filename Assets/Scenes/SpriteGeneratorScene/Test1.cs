using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
public class Test1 : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TestReadJson()
    {
        Debug.Log("Test");
        Dictionary<string, Node> json = JsonReader.ReadWorkflowJson();
        string jsonString = JsonConvert.SerializeObject(json, Formatting.Indented);
        Debug.Log(jsonString);
        
    }
}
