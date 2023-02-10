namespace AFSInterview
{
    using UnityEngine;

    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float speed;

        private GameObject targetObject;

        public void Initialize(GameObject target)
        {
            targetObject = target;
        }

        private void Update()
        {
            var direction = (targetObject.transform.position - transform.position).normalized;

            transform.position += direction * speed * Time.deltaTime;

            if ((transform.position - targetObject.transform.position).magnitude <= 0.2f)
            {
                Destroy(gameObject);
                Destroy(targetObject);
            }
        }
    }
}