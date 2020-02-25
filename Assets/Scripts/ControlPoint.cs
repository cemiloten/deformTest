using UnityEngine;

public class ControlPoint : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Transform trs = transform;
        Gizmos.color = Color.blue;
        Gizmos.matrix = trs.localToWorldMatrix;
        Gizmos.DrawSphere(Vector3.zero, 0.15f);
   }
}

