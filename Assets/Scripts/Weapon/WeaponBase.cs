using UnityEngine;

public abstract class WeaponBase : MonoBehaviour, IWeapon
{
    [SerializeField] protected string weaponName = "Weapon";
    [SerializeField] protected float attackCooldown = 0.5f;
    [SerializeField] protected bool unlocked = false;

    protected Transform cameraTransform;
    protected float lastAttackTime = -999f;

    public string WeaponName => weaponName;
    public bool Unlocked => unlocked;

    public virtual void Equip(Transform cameraTransform)
    {
        this.cameraTransform = cameraTransform;
        gameObject.SetActive(true);
    }

    public virtual void Unequip()
    {
        gameObject.SetActive(false);
    }

    public void TryAttack()
    {
        if (Time.time - lastAttackTime < attackCooldown)
        {
            return;
        }

        lastAttackTime = Time.time;
        Attack();
    }

    protected abstract void Attack();
}
