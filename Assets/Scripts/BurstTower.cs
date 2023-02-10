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

        private WaitForSeconds waitBetweenShots;
        private Vector3 previousEnemyPosition;
        private Coroutine fireControl;
        private List<GameObject> pooledBullets;

        
        public override void Initialize(IReadOnlyList<Enemy> enemies)
        {
            base.Initialize(enemies);

            waitBetweenShots = new WaitForSeconds(intervalBetweenShots);
            PreparePool();
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

                var bullet = GetBullet().GetComponent<CurveBullet>();
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
        
        private void PreparePool()
        {
            pooledBullets = new List<GameObject>();
            
            for (var i = 0; i < bulletsAmount; i++)
            {
                CreateNewBullet();
            }
        }

        private GameObject CreateNewBullet()
        {
            var bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);
            pooledBullets.Add(bullet);
            return bullet;
        }

        private GameObject GetBullet()
        {
            foreach (var pooledObject in pooledBullets)
            {
                if(!pooledObject.activeInHierarchy)
                    return pooledObject;
            }

            return CreateNewBullet();
        }
    }
}
