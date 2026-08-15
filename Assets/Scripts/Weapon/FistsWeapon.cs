using CodeMonkey.HealthSystemCM;
using UnityEngine;

public class FistsWeapon : WeaponBase
{
    [SerializeField] private float damage = 5f;
    [SerializeField] private float hitRadius = 0.5f;
    [SerializeField] private float hitRange = 1.2f;
    [SerializeField] private LayerMask hittableLayers = -0;

    private readonly Collider[] _hitBuffer = new Collider[16];

    protected override void Attack()
    {
        Vector3 origin = cameraTransform.position + cameraTransform.forward * hitRange;
        int hitCount = Physics.OverlapSphereNonAlloc(origin, hitRadius, _hitBuffer, hittableLayers, QueryTriggerInteraction.Ignore);

        for (int i = 0; i < hitCount; i++)
        {
            Collider collider = _hitBuffer[i];
            if (collider == null)
            {
                continue;
            }

            ObjectBreakable breakable = collider.GetComponentInParent<ObjectBreakable>();
            if (breakable != null)
            {
                breakable.TakeWeaponDamage(damage, cameraTransform.forward);
            }
            else
            {
                IGetHealthSystem target = collider.GetComponentInParent<IGetHealthSystem>();
                if (target != null)
                {
                    target.GetHealthSystem().Damage(damage);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 origin = cameraTransform.position + cameraTransform.forward * hitRange;
        Gizmos.DrawWireSphere(origin, hitRadius);
    }
}
