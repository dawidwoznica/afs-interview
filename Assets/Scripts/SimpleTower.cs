namespace AFSInterview
{
    using UnityEngine;

    public class SimpleTower : Tower
    {
        protected override void Fire()
        {
            var bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity).GetComponent<Bullet>();
            bullet.Initialize(TargetEnemy.gameObject);
        }
    }
}
