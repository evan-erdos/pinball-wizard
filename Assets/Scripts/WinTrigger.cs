using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinTrigger : MonoBehaviour {
    [SerializeField] protected GameObject successParticles;
    void OnTriggerEnter(Collider c) { if (c.attachedRigidbody.name!="Pinball") return;
        Instantiate(successParticles, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
