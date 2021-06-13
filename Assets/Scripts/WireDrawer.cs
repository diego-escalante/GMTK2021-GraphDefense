using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WireDrawer : MonoBehaviour {

    [SerializeField]
    private Material wireMaterial;

    private Dictionary<List<BaseStructure>, LineRenderer> connections = new Dictionary<List<BaseStructure>, LineRenderer>();

    public void AddConnection(BaseStructure a, BaseStructure b) {
        if (AreConnected(a,b)) {
            return;
        }

        LineRenderer lineRenderer = new GameObject(string.Format("Connection {0}-{1}", a.gameObject.name, b.gameObject.name)).AddComponent<LineRenderer>();
        lineRenderer.transform.SetParent(transform);
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.startColor = Color.yellow;
        lineRenderer.endColor = Color.yellow;
        lineRenderer.positionCount = 2;
        lineRenderer.material = wireMaterial;
        lineRenderer.SetPositions(new Vector3[]{a.transform.position + new Vector3(0,0,0.1f), b.transform.position + new Vector3(0,0,0.1f)});

        connections.Add(new List<BaseStructure>{a, b}, lineRenderer);
    }

    public void RemoveConnection(BaseStructure a, BaseStructure b) {
        List<BaseStructure> connection = GetConnection(a, b);
        if (connection == null) {
            return;
        }

        Destroy(connections[connection].gameObject);
        connections.Remove(connection);
    }

    private bool AreConnected(BaseStructure a, BaseStructure b) {
        return connections.Any(pair => pair.Key.Contains(a) && pair.Key.Contains(b));
    }

    private List<BaseStructure> GetConnection(BaseStructure a, BaseStructure b) {
        List<List<BaseStructure>> c = connections.Keys.Where(key => key.Contains(a) && key.Contains(b)).ToList();

        if (c.Count == 0) {
            return null;
        }

        return c.First();
    }
}
