using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeController : MonoBehaviour
{
    public List<NodeController> nodes;
    public Color color;

    private SpriteRenderer renderer;

    [HideInInspector]
    public NodeController parent;
    [HideInInspector]
    public float g; // G is the distance between the current node and the start node.
    [HideInInspector]
    public float h; // H is the heuristic — estimated distance from the current node to the end node.
    [HideInInspector]
    public float f; // F = G + H

    // Start is called before the first frame update
    void Start()
    {
        color = new Color(1, 1, 1);
        renderer = GetComponent<SpriteRenderer>();
        renderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        renderer.color = color;
    }

    private void FixedUpdate()
    {
        if (nodes != null)
        {
            foreach (var node in nodes)
                Debug.DrawLine(transform.position, node.transform.position);
        }
    }

    public void OnDrawGizmosSelected()
    {
        foreach (var node in nodes)
            Gizmos.DrawLine(transform.position, node.transform.position);
    }
}
