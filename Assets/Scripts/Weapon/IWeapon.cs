using UnityEngine;

public interface IWeapon
{
    string WeaponName { get; }
    void Equip(Transform cameraTransform);
    void Unequip();
    void TryAttack();
}
