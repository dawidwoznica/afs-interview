using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AFSInterview
{
    public class BurstTower : Tower
    {
        [SerializeField] private float intervalBetweenShots;
        [SerializeField] private int bulletsAmount;
        [SerializeField] private int initialAngle;
        
        private BulletPool<CurveBullet> bulletPool;
        private WaitForSeconds waitBetweenShots;
        private Vector3 previousEnemyPosition;
        private Coroutine fireControl;

        
        public override void Initialize(IReadOnlyList<Enemy> enemies)
        {
            base.Initialize(enemies);
            bulletPool = new BulletPool<CurveBullet>(bulletsAmount, bulletPrefab);
            waitBetweenShots = new WaitForSeconds(intervalBetweenShots);
        }

        protected override void Update()
        {
            base.Update();

            if(TargetEnemy)
                previousEnemyPosition = TargetEnemy.transform.position;
        }

        protected override void Fire()
        {
            StopFireControlIfRunning();
            fireControl = StartCoroutine(FireControl());
        }

        private IEnumerator FireControl()
        {
            var expectedEnemy = TargetEnemy;
            
            for (var i = 0; i < bulletsAmount; i++)
            {
                if (TargetEnemy == null) 
                    break;
                
                if (TargetEnemy != expectedEnemy) 
                {
                    fireTimer -= (bulletsAmount - i) / (float)bulletsAmount * firingRate;
                    break;
                }

                var predictedPosition = PredictPosition(TargetEnemy.transform.position);

                var bullet = bulletPool.GetBullet();
                bullet.transform.position = bulletSpawnPoint.position;
                bullet.gameObject.SetActive(true);
                bullet.OnTargetReached += OnBulletReachedTarget;
                bullet.Initialize(CalculateVelocity(predictedPosition));

                yield return waitBetweenShots;
            }
        }
        
        private Vector3 PredictPosition(Vector3 enemyPosition)
        {
            var enemyForward = (enemyPosition - previousEnemyPosition).normalized;
            var enemySpeed = Vector3.Distance(enemyPosition, previousEnemyPosition) / Time.deltaTime;
            
            var position = enemyPosition + enemyForward * enemySpeed;
            
            return position;
        }

        private Vector3 CalculateVelocity(Vector3 targetPosition)
        {
            var gravity = Physics.gravity.magnitude;
            var angle = initialAngle * Mathf.Deg2Rad;

            var bulletSpawnPosition = bulletSpawnPoint.position;
 
            var planarTarget = new Vector3(targetPosition.x, 0, targetPosition.z);
            var planarPosition = new Vector3(bulletSpawnPosition.x, 0, bulletSpawnPosition.z);
            
            var distance = Vector3.Distance(planarTarget, planarPosition);
            
            var yOffset = bulletSpawnPosition.y - targetPosition.y;
 
            var initialVelocity = 1 / Mathf.Cos(angle) * Mathf.Sqrt(0.5f * gravity * Mathf.Pow(distance, 2) / (distance * Mathf.Tan(angle) + yOffset));
 
            var velocity = new Vector3(0, initialVelocity * Mathf.Sin(angle), initialVelocity * Mathf.Cos(angle));
 
            var angleBetweenObjects = Vector3.Angle(Vector3.forward, planarTarget - planarPosition) * (targetPosition.x > transform.position.x ? 1 : -1);
            
            return Quaternion.AngleAxis(angleBetweenObjects, Vector3.up) * velocity;
        }
        
        private void OnBulletReachedTarget(CurveBullet bullet)
        {
            bullet.gameObject.SetActive(false);
        }

        private void StopFireControlIfRunning()
        {
            if (fireControl == null) return;
            
            StopCoroutine(fireControl);
            fireControl = null;
        }
    }
}
