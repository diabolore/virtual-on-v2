using UnityEngine;

namespace Combat
{
    public class WeaponController : MonoBehaviour
    {
        [SerializeField] private MachineGun machineGun;
        [SerializeField] private LaserSword laserSword;

        public void UsePrimaryAttack()
        {
            if (machineGun != null)
            {
                machineGun.Fire();
            }
        }

        public void UseSecondaryAttack()
        {
            if (laserSword != null)
            {
                laserSword.Swing();
            }
        }
    }
}
