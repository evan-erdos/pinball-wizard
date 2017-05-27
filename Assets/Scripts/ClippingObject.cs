using UnityEngine;
using System.Collections;
using System.Linq;

[ExecuteInEditMode] public class ClippingObject : MonoBehaviour {
    [SerializeField] float planeSize = 5;
    [SerializeField] [Range(0,3)] int clipPlanes = 0;
    public Vector3 plane1Position = Vector3.zero, plane1Rotation = new Vector3(0,0,0);
    public Vector3 plane2Position = Vector3.zero, plane2Rotation = new Vector3(0,90,90);
    public Vector3 plane3Position = Vector3.zero, plane3Rotation = new Vector3(0,0,90);

    public void Start() { }
    public void OnEnable() => GetComponent<MeshRenderer>().sharedMaterial =
        new Material(Shader.Find("Custom/StandardClippable")) {
            hideFlags = HideFlags.HideAndDontSave };

    void DrawPlane(Vector3 position, Vector3 euler) {
        var forward = Quaternion.Euler(euler) * Vector3.forward;
        var left = Quaternion.Euler(euler) * Vector3.left;
        var forwardLeft = position + forward * planeSize * 0.5f + left * planeSize * 0.5f;
        var forwardRight = forwardLeft - left * planeSize;
        var backRight = forwardRight - forward * planeSize;
        var backLeft = forwardLeft - forward * planeSize;

        Gizmos.DrawLine(position, forwardLeft);
        Gizmos.DrawLine(position, forwardRight);
        Gizmos.DrawLine(position, backRight);
        Gizmos.DrawLine(position, backLeft);

        Gizmos.DrawLine(forwardLeft, forwardRight);
        Gizmos.DrawLine(forwardRight, backRight);
        Gizmos.DrawLine(backRight, backLeft);
        Gizmos.DrawLine(backLeft, forwardLeft);
    }

    void OnDrawGizmosSelected() {
        if (clipPlanes >= 1) DrawPlane(plane1Position, plane1Rotation);
        if (clipPlanes >= 2) DrawPlane(plane2Position, plane2Rotation);
        if (clipPlanes >= 3) DrawPlane(plane3Position, plane3Rotation);
    }

    void Update() {
        var sharedMaterial = GetComponent<MeshRenderer>().sharedMaterial;
        switch (clipPlanes) {
            case 0:
                sharedMaterial.DisableKeyword("CLIP_ONE");
                sharedMaterial.DisableKeyword("CLIP_TWO");
                sharedMaterial.DisableKeyword("CLIP_THREE");
                break;
            case 1:
                sharedMaterial.EnableKeyword("CLIP_ONE");
                sharedMaterial.DisableKeyword("CLIP_TWO");
                sharedMaterial.DisableKeyword("CLIP_THREE");
                break;
            case 2:
                sharedMaterial.DisableKeyword("CLIP_ONE");
                sharedMaterial.EnableKeyword("CLIP_TWO");
                sharedMaterial.DisableKeyword("CLIP_THREE");
                break;
            case 3:
                sharedMaterial.DisableKeyword("CLIP_ONE");
                sharedMaterial.DisableKeyword("CLIP_TWO");
                sharedMaterial.EnableKeyword("CLIP_THREE");
                break;
        }

        if (clipPlanes >= 1) {
            sharedMaterial.SetVector("_planePos", plane1Position);
            sharedMaterial.SetVector("_planeNorm",
                Quaternion.Euler(plane1Rotation) * Vector3.up);
        }

        if (clipPlanes >= 2) {
            sharedMaterial.SetVector("_planePos2", plane2Position);
            sharedMaterial.SetVector("_planeNorm2",
                Quaternion.Euler(plane2Rotation) * Vector3.up);
        }

        if (clipPlanes >= 3) {
            sharedMaterial.SetVector("_planePos3", plane3Position);
            sharedMaterial.SetVector("_planeNorm3",
                Quaternion.Euler(plane3Rotation) * Vector3.up);
        }
    }
}
