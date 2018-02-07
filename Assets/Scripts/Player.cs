using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Player : MonoBehaviour {
    [SerializeField] protected float power = 10;
    [SerializeField] protected float delay = 0.4f;
    [SerializeField] protected float impulse = 20;
    [SerializeField] protected float torque = 1000;
    [SerializeField] protected float length = 0.25f;

    bool isGrounded, isJumping, isRolling;
    float jumpAngle = 0.5f;
    Vector3 touch, movement = new Vector3(0,0,0);
    new Rigidbody rigidbody;
    List<Transform> children = new List<Transform>();
    List<Collider> colliders = new List<Collider>();


    void ApplyLegForce(Vector3 point, Vector3 normal) {
        if (isRolling) rigidbody.AddForce(-normal*impulse);
        rigidbody.AddForceAtPosition(
            normal*Vector3.Dot(normal,transform.forward)*impulse,
            transform.position-(point-transform.position),
            ForceMode.VelocityChange);
    }


    void Awake() {
        rigidbody = GetComponent<Rigidbody>();
        foreach (var c in GetComponentsInChildren<Collider>())
            if (c.name=="leg") children.Add(c.transform);
        foreach (var leg in children)
            colliders.AddRange(leg.GetComponents<Collider>());
    }


    void Update() {
        isJumping = Input.GetButtonDown("Jump");
        isRolling = Input.GetButton("Fire");
        movement = new Vector3(Input.GetAxis("Roll"), Input.GetAxis("Pitch"), 0);
    }


    bool waitJump;
    void Jump() { if (!waitJump) StartCoroutine(Jumping()); }
    IEnumerator Jumping() {
        waitJump = true;
        yield return new WaitForFixedUpdate();
        var jump = Vector3.Lerp(touch, Vector3.up, (1+movement.x)/2*jumpAngle);
        rigidbody.AddForce(jump*power, ForceMode.Impulse);
        yield return new WaitForSeconds(delay);
        waitJump = false;
    }


    float speed;
    void FixedUpdate() {
        if (isGrounded && isJumping) Jump();
        rigidbody.AddTorque(movement*torque, ForceMode.VelocityChange);
        transform.localRotation = Quaternion.Euler(0,90,transform.localEulerAngles.z);
        foreach (var col in colliders) col.enabled = isRolling;
        foreach (var leg in children)
            leg.localPosition = new Vector3 { y=0, z=0, x=Mathf.SmoothDamp(
                current: leg.localPosition.x, target: (isRolling)?0:length,
                currentVelocity: ref speed, smoothTime: 0.1f, maxSpeed: 100) };
    }

    void OnCollisionEnter(Collision collision) {
        isGrounded = true;
        touch = transform.position-collision.contacts.FirstOrDefault().point;
    }

    void OnCollisionExit(Collision collision) {
        isGrounded = false; touch = Vector3.up; }

    void OnCollisionStay(Collision collision) {
        foreach (var hit in collision.contacts)
            if (hit.thisCollider.name=="leg")
                ApplyLegForce(hit.point, hit.normal); }

    // void OnTriggerStay(Collider c) => rigidbody.AddForce(
    //     (transform.position-c.transform.localPosition).normalized*impulse,
    //     ForceMode.Impulse);

}
