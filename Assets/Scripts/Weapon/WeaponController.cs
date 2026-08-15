using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    public class WeaponController : MonoBehaviour
    {
        [SerializeField] private Transform weaponHoldPoint;
        [SerializeField] private WeaponBase[] weapons;
        [SerializeField] private float scrollThreshold = 0.05f;

        private StarterAssetsInputs _input;
        private Transform _cameraTransform;
        private int _currentIndex = -1;
        private bool[] _unlocked;

        private void Awake()
        {
            _input = GetComponent<StarterAssetsInputs>();
            _cameraTransform = weaponHoldPoint != null ? weaponHoldPoint : transform;
            _unlocked = new bool[weapons.Length];
            for (int i = 0; i < weapons.Length; i++)
            {
                _unlocked[i] = weapons[i] != null && weapons[i].Unlocked;
            }
        }

        private void Start()
        {
            foreach (WeaponBase weapon in weapons)
            {
                if (weapon != null)
                {
                    weapon.gameObject.SetActive(false);
                }
            }
        }

        private void Update()
        {
            if (_input.hit && _currentIndex >= 0)
            {
                weapons[_currentIndex]?.TryAttack();
                _input.hit = false;
            }

            HandleWeaponSwitchInput();
        }

        private void HandleWeaponSwitchInput()
        {
            HandleScrollSwitch();
            HandleNumberKeySwitch();
        }

        private void HandleScrollSwitch()
        {
            float scroll = _input.weaponScroll;

            if (scroll > scrollThreshold)
            {
                NextWeapon();
            }
            else if (scroll < -scrollThreshold)
            {
                PreviousWeapon();
            }

            if (Mathf.Abs(scroll) > 0f)
            {
                _input.weaponScroll = 0f;
            }
        }

        private void HandleNumberKeySwitch()
        {
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current == null)
            {
                return;
            }

            for (int i = 0; i < weapons.Length && i < 9; i++)
            {
                Key key = Key.Digit1 + i;
                if (Keyboard.current[key].wasPressedThisFrame)
                {
                    if (!_unlocked[i])
                    {
                        break;
                    }

                    if (i == _currentIndex)
                    {
                        UnequipCurrent();
                    }
                    else
                    {
                        EquipWeapon(i);
                    }
                    break;
                }
            }
#endif
        }

        public void EquipWeapon(int index)
        {
            if (index < 0 || index >= weapons.Length || index == _currentIndex || !_unlocked[index])
            {
                return;
            }

            if (_currentIndex >= 0 && weapons[_currentIndex] != null)
            {
                weapons[_currentIndex].Unequip();
            }

            _currentIndex = index;
            weapons[_currentIndex]?.Equip(_cameraTransform);
        }

        public bool IsUnlocked(int index)
        {
            return index >= 0 && index < _unlocked.Length && _unlocked[index];
        }

        public void UnlockWeapon(int index, bool autoEquip = true)
        {
            if (index < 0 || index >= weapons.Length)
            {
                return;
            }

            if (_unlocked[index])
            {
                return;
            }

            _unlocked[index] = true;

            if (autoEquip)
            {
                EquipWeapon(index);
            }
        }

        public void UnlockWeapon(WeaponBase weapon, bool autoEquip = true)
        {
            int index = System.Array.IndexOf(weapons, weapon);
            UnlockWeapon(index, autoEquip);
        }

        public void UnequipCurrent()
        {
            if (_currentIndex < 0)
            {
                return;
            }

            weapons[_currentIndex]?.Unequip();
            _currentIndex = -1;
        }

        public void NextWeapon()
        {
            if (weapons.Length == 0) return;

            for (int i = _currentIndex + 1; i < weapons.Length; i++)
            {
                if (_unlocked[i])
                {
                    EquipWeapon(i);
                    return;
                }
            }
        }

        public void PreviousWeapon()
        {
            if (weapons.Length == 0) return;

            for (int i = _currentIndex - 1; i >= 0; i--)
            {
                if (_unlocked[i])
                {
                    EquipWeapon(i);
                    return;
                }
            }

            UnequipCurrent();
        }
    }
}