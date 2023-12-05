using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodesHandler : MonoBehaviour
{
    [SerializeField]
    private List<Node> allNodes = new List<Node>();

    private void Start()
    {

        for (int i = 0; i < allNodes.Count; i++)
        {
            Node temp = allNodes[i];
            int randomIndex = Random.Range(i, allNodes.Count);
            allNodes[i] = allNodes[randomIndex];
            allNodes[randomIndex] = temp;
        }

        for (int i = 0; i < allNodes.Count; i++)
        {
            allNodes[i].unitCount = i + 2;
        }

    }
    public void DropAllNodes()
    {
        for (int i = 0; i < allNodes.Count; i++)
        {
            allNodes[i].Invoke("Drop",Random.Range(1.5f,2.5f));
        }
    }




}
