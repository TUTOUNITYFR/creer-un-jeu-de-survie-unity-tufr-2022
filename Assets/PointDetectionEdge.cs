using UnityEngine;

public class PointDetectionEdge : MonoBehaviour
{
    public bool connected;
    public float radius = 0.6f;

    public Collider[] hitColliders;

    private void OnDisable()
    {
        connected = false;
    }

    public void CheckOverlap()
    {
        connected = false;

        hitColliders = Physics.OverlapSphere(transform.position, radius);

        if(hitColliders.Length > 0)
        {
            foreach (var collider in hitColliders)
            {
                if(collider.CompareTag("Point") || collider.CompareTag("Terrain"))
                {
                    connected = true;
                    return;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
