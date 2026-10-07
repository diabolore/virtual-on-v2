using UnityEngine;

namespace Combat
{
    public class WeaponController : MonoBehaviour
    {
        [SerializeField] private WeaponBase primaryWeapon;
        [SerializeField] private WeaponBase secondaryWeapon;

        public void UsePrimaryAttack()
        {
            if (primaryWeapon != null)
            {
                primaryWeapon.Use();
            }
        }

        public void UseSecondaryAttack()
        {
            if (secondaryWeapon != null)
            {
                secondaryWeapon.Use();
            }
        }
    }
}
