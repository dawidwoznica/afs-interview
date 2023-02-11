using System.Collections.Generic;
using UnityEngine;

namespace AFSInterview
{
    public class BulletPool<T> where T : MonoBehaviour
    {
        private readonly GameObject bulletPrefab;
        private readonly List<T> pooledBullets;

        public BulletPool(int initialBulletAmount, GameObject bulletPrefab)
        {
            this.bulletPrefab = bulletPrefab;
            
            pooledBullets = new List<T>();
            
            for (var i = 0; i < initialBulletAmount; i++)
            {
                CreateNewBullet();
            }
        }

        private T CreateNewBullet()
        {
            var bullet = Object.Instantiate(bulletPrefab).GetComponent<T>();
            bullet.gameObject.SetActive(false);
            pooledBullets.Add(bullet);
            return bullet;
        }

        public T GetBullet()
        {
            foreach (var pooledObject in pooledBullets)
            {
                if(!pooledObject.gameObject.activeInHierarchy)
                    return pooledObject;
            }

            return CreateNewBullet();
        }
    }
}