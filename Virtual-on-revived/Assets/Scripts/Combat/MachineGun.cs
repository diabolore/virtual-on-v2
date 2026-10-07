using UnityEngine;
using System;
using UnityEngine.Pool;

namespace Combat
{
    public class MachineGun : WeaponBase
    {
        public event Action<int> OnAmmoChanged;

        [SerializeField] private int maxAmmo = 100;
        private int currentAmmo;
        [SerializeField] private Transform firePoint;
        [SerializeField] private float fireRate = 0.1f;
        private float nextFireTime;

        // Optionally bind directly if not using a central manager, 
        // or replace this logic with ObjectPoolManager.Instance.
        private ObjectPool<Projectile> localPool;
        [SerializeField] private Projectile projectilePrefab;

        private void Awake()
        {
            currentAmmo = maxAmmo;

            if (projectilePrefab != null)
            {
                localPool = new ObjectPool<Projectile>(
                    createFunc: () => Instantiate(projectilePrefab),
                    actionOnGet: (obj) => {
                        obj.transform.position = firePoint.position;
                        obj.transform.rotation = firePoint.rotation;
                        obj.gameObject.SetActive(true);
                        obj.SetPool(localPool);
                    },
                    actionOnRelease: (obj) => obj.gameObject.SetActive(false),
                    actionOnDestroy: (obj) => Destroy(obj.gameObject),
                    collectionCheck: false,
                    defaultCapacity: 20,
                    maxSize: 100
                );
            }
        }

        public override void Use()
        {
            if (Time.time >= nextFireTime && currentAmmo > 0)
            {
                nextFireTime = Time.time + fireRate;
                currentAmmo--;
                OnAmmoChanged?.Invoke(currentAmmo);
                
                if (localPool != null)
                {
                    localPool.Get();
                }
            }
        }
    }
}
