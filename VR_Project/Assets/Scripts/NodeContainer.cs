using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NodeData", menuName = "ScriptableObjects/NodeData", order = 1)]
public class NodeContainer : ScriptableObject
{
    [SerializeField] public Node[] NodeGraph = null;
}


