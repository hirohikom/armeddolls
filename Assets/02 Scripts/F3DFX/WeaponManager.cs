using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static bool isShoot_LS = false;
    public static bool isShoot_RS = false;
    public static bool isShoot_LH = false;
    public static bool isShoot_RH = false;
    public static bool isLookAtTarget = false;

    void Update()
    {
        if (isShoot_LS || isShoot_RS || isShoot_LH || isShoot_RH)
        {
            isLookAtTarget = true;
        }
        else
        {
            isLookAtTarget = false;
        }
    }
}
