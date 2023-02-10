using System.Collections.Generic;
using UnityEngine;

namespace AFSInterview
{
    public abstract class Tower : MonoBehaviour
    {
        [SerializeField] protected GameObject bulletPrefab;
        [SerializeField] protected Transform bulletSpawnPoint;
        [SerializeField] protected float firingRate;
        [SerializeField] private float firingRange;

        protected Enemy TargetEnemy;
        protected float fireTimer;

        private IReadOnlyList<Enemy> enemies;

        
        public virtual void Initialize(IReadOnlyList<Enemy> enemies)
        {
            this.enemies = enemies;
            fireTimer = firingRate;
        }

        protected virtual void Update()
        {
            TargetEnemy = FindClosestEnemy();
            
            if (TargetEnemy != null)
            {
                var lookRotation = Quaternion.LookRotation(TargetEnemy.transform.position - transform.position);
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, lookRotation.eulerAngles.y, transform.rotation.eulerAngles.z);
            }

            fireTimer -= Time.deltaTime;
            
            if (fireTimer <= 0f)
            {
                if (TargetEnemy != null)
                    Fire();

                fireTimer = firingRate;
            }
        }
        
        private Enemy FindClosestEnemy()
        {
            Enemy closestEnemy = null;
            var closestDistance = float.MaxValue;

            foreach (var enemy in enemies)
            {
                var distance = (enemy.transform.position - transform.position).magnitude;
                
                if (distance <= firingRange && distance <= closestDistance)
                {
                    closestEnemy = enemy;
                    closestDistance = distance;
                }
            }

            return closestEnemy;
        }

        protected abstract void Fire();
    }
}