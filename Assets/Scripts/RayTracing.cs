using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayTracing : MonoBehaviour
{
    public float maxDistance = 10f;
    public LayerMask groundLayer;

    void Update()
    {
        float northDistance = RaycastDirection(Vector3.forward);
        float southDistance = RaycastDirection(Vector3.back);
        float eastDistance = RaycastDirection(Vector3.right);
        float westDistance = RaycastDirection(Vector3.left);

        // Use these distances as input to your neural network
        // For example:
        // neuralNetwork.SetInput(new float[] { northDistance, southDistance, eastDistance, westDistance });
    }

    public float RaycastDirection(Vector3 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, maxDistance, groundLayer))
        {
            if (hit.collider.CompareTag("Ground"))
            {
                return hit.distance;
            }
        }
        return maxDistance;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.forward * maxDistance);
        Gizmos.DrawRay(transform.position, Vector3.back * maxDistance);
        Gizmos.DrawRay(transform.position, Vector3.right * maxDistance);
        Gizmos.DrawRay(transform.position, Vector3.left * maxDistance);
    }
}
