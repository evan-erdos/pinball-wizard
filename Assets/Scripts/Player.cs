using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    bool isJumping;
    float speed;
    [SerializeField] float force = 1000;
    [SerializeField] float torque = 1000;
    (float x, float y) movement = (0,0);
    new Rigidbody rigidbody;
    List<Transform> legs = new List<Transform>();
    void Awake() => rigidbody = GetComponent<Rigidbody>();
    void Start() {
        foreach (var child in GetComponentsInChildren<Collider>())
            if (child.name=="leg") legs.Add(child.transform); }

    void OnTriggerStay(Collider c) =>
        rigidbody.AddForce((transform.position-c.transform.localPosition).normalized, ForceMode.Impulse);

    void OnCollisionStay(Collision c) {
        if (c.collider.name!="leg") return;
        rigidbody.AddForce(c.contacts.FirstOrDefault().normal*force, ForceMode.VelocityChange); }

    void Update() => (isJumping, movement.x, movement.y) =
        (Input.GetButton("Jump"), Input.GetAxis("Roll"), Input.GetAxis("Pitch"));

    void FixedUpdate() {
        rigidbody.AddTorque(new Vector3(movement.x, movement.y, 0)*torque, ForceMode.VelocityChange);
        foreach (var leg in legs) {
            foreach (var collider in leg.GetComponents<Collider>()) collider.enabled = isJumping;
            leg.localPosition = new Vector3(
                y: 0, z: 0, x: Mathf.SmoothDamp(
                    current: leg.localPosition.x,
                    target: (isJumping)?0:0.25f,
                    currentVelocity: ref speed,
                    smoothTime: 0.01f,
                    maxSpeed: 1000));
        }
    }
}
