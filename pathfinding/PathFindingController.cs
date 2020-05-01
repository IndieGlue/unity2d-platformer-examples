using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFindingController : MonoBehaviour
{
    public NodeController startNode;
    public NodeController endNode;

    private List<NodeController> openlist = new List<NodeController>();
    private List<NodeController> closedlist = new List<NodeController>();

    private IEnumerator pathFindRoutine;

    void Start()
    {
        pathFindRoutine = PathFindRoutine();
        StartCoroutine(pathFindRoutine);
    }   
    
    IEnumerator PathFindRoutine()
    {
        openlist.Add(startNode);
        var currentNode = openlist.First();

        while(openlist.Count > 0)
        {
            var shortestPath = float.MaxValue;

            foreach(var item in openlist)
            {
                var distance = Vector3.Distance(currentNode.transform.position, item.transform.position);

                if(distance < shortestPath)
                {
                    shortestPath = distance;
                    currentNode = item;
                }
            }

            openlist.Remove(currentNode);
            closedlist.Add(currentNode);

            currentNode.color = new Color(0, 0, 0);

            if(currentNode == endNode)
            {
                var node = endNode;
                while (node.parent != null)
                {
                    node.color = new Color(255, 0, 0);
                    node = node.parent;
                }

                node.color = new Color(255, 0, 0);
                // Traverse back and get shortest path
                StopCoroutine(pathFindRoutine);
            }

            foreach(var child in currentNode.nodes)
            {
                if (closedlist.Contains(child))
                    continue;

                child.parent = currentNode;

                child.g = currentNode.g + Vector3.Distance(currentNode.transform.position, child.transform.position);
                child.h = Vector3.Distance(currentNode.transform.position, endNode.transform.position);
                child.f = child.g + child.h;

                foreach (var openNode in openlist)
                    if (child == openNode && child.g > openNode.g)
                        continue;

                openlist.Add(child);
            }

            yield return new WaitForSeconds(0.5f);
        }
    }
}
