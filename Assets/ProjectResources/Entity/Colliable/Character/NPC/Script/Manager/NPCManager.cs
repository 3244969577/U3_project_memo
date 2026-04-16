using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : ManagerBase
{
    public static NPCManager Instance => (NPCManager)instance;

    private Dictionary<string, GameObject> generatedNPCs = new Dictionary<string, GameObject>();

    public void RegisterNPC(string name, GameObject npc)
    {
        generatedNPCs.Add(name, npc);
    }
    public GameObject GetNPC(string name)
    {
        return generatedNPCs[name];
    }

    public bool IsNPCExist(string name)
    {
        return generatedNPCs.ContainsKey(name);
    }
}
