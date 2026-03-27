using UnityEngine;
using System.Collections.Generic;
using Fungus;

public class DialogSceneTestCase : MonoBehaviour
{
    public GameObject NPC;
    private Flowchart targetFlowchart;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targetFlowchart = GameManager.Instance.flowchart;
        FungusDialogRenderer.Instance.RenderDialogLines(new List<string>{
            "1",
            "2",
            "3",
            "4"
        }, NPC.GetComponentInChildren<Fungus.Character>(), NPC.GetComponent<SpriteRenderer>().sprite);
       
        // FungusDialogRenderer.Instance.RenderDialogLines(new List<string>{
        //     "1",
        //     "2",
        //     "3",
        //     "4"
        // });
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FungusDialogRenderer.Instance.RenderDialogLines(new List<string>{
                "1",
                "2",
                "3",
                "4"
            });

        }
    }
}
