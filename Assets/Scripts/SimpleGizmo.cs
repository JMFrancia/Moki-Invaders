using System;
using UnityEngine;

/*
 * Simple utility to draw a standard gizmo; great for debugging or keeping track of 
 * important location-specific game objects that have no renderers attached
 */
public class SimpleGizmo : MonoBehaviour
{
    [Serializable]
    enum GizmoShape
    {
        Sphere,
        WireSphere,
        Cube,
        WireCube,
        Ray
    }
    [Tooltip("Show gizmo in scene")]
    [SerializeField] bool _show = true;
    [Tooltip("Gizmo color in scene")]
    [SerializeField] Color _color = Color.green;
    [Tooltip("Gizmo size in scene")]
    [SerializeField] float _size = 1f;
    [Tooltip("Gizmo shape in scene")]
    [SerializeField] GizmoShape _shape = GizmoShape.WireSphere;

    private void OnDrawGizmos()
    {
        if (!_show)
            return;
        Gizmos.color = _color;
        switch (_shape) {
            case GizmoShape.WireCube:
                Gizmos.DrawWireCube(transform.position, Vector3.one * _size);
                break;
            case GizmoShape.Cube:
                Gizmos.DrawCube(transform.position, Vector3.one * _size);
                break;
            case GizmoShape.WireSphere:
                Gizmos.DrawWireSphere(transform.position, _size);
                break;
            case GizmoShape.Sphere:
                Gizmos.DrawSphere(transform.position, _size);
                break;
            case GizmoShape.Ray:
                Gizmos.DrawRay(transform.position, Vector3.up * _size);
                Gizmos.DrawRay(transform.position, Vector3.down * _size);
                break;
        }
    }
}
