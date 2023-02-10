using System;
using UnityEngine;

namespace AFSInterview
{
    public class CurveBullet : MonoBehaviour
    {
        private Rigidbody rb;
        public Action<CurveBullet> OnTargetReached;

        public void Initialize(Vector3 velocity)
        {
            rb = GetComponent<Rigidbody>();
            rb.velocity = velocity;
        }
        
        private void Update()
        {
            if (transform.position.y <= 0)
                OnTargetReached?.Invoke(this);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Enemy>() == null) 
                return;
            
            OnTargetReached?.Invoke(this);
            Destroy(other.gameObject);
        }
    }
}