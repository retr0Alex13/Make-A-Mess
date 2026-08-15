using UnityEngine;
using CodeMonkey.HealthSystemCM;
using System.Collections;

public class ObjectBreakable : MonoBehaviour, IGetHealthSystem
{
    public float CurrentSwingSpeed => currentSwingSpeed;
    [SerializeField] private float maxHealth = 100;
    [SerializeField] private float damageMultiplier = 3f;
    [SerializeField] private float damageThresholdSpeed = 2f;
    [SerializeField] private float healthbarDisplayTime = 3f;

    [SerializeField] private GameObject intactObject;
    [SerializeField] private GameObject[] destroyedObjects;
    [SerializeField] private GameObject healthBar;

    [SerializeField] private Rigidbody objectRigidBody;
    [SerializeField] private Collider objectCollider;
    [SerializeField] private ObjectGrabable objectGrabable;

    private HealthSystem healthSystem;
    private Vector3 previousPosition;
    private float currentSwingSpeed;
    private Coroutine healthbarCoroutine;

    private void Awake()
    {
        healthSystem = new HealthSystem(maxHealth);
        healthSystem.OnDead += OnObjectBreak;
        healthSystem.OnDamaged += ShowHealthBar;
    }

    private void ShowHealthBar(object sender, System.EventArgs e)
    {
        if (healthbarCoroutine != null)
        {
            StopCoroutine(healthbarCoroutine);
        }

        healthbarCoroutine = StartCoroutine(ShowHealthBarForSeconds());
    }

    private IEnumerator ShowHealthBarForSeconds()
    {
        healthBar.SetActive(true);
        yield return new WaitForSeconds(healthbarDisplayTime);
        healthBar.SetActive(false);
        healthbarCoroutine = null;
    }

    private void FixedUpdate()
    {
        Vector3 calculatedVelocity = (transform.position - previousPosition) / Time.fixedDeltaTime;
        currentSwingSpeed = calculatedVelocity.magnitude;
        previousPosition = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        float impactSpeed = collision.relativeVelocity.magnitude;

        if (currentSwingSpeed > impactSpeed)
        {
            impactSpeed = currentSwingSpeed;
        }

        ObjectBreakable[] otherObjects = collision.gameObject.GetComponents<ObjectBreakable>();
        if (otherObjects != null && otherObjects.Length > 0)
        {
            foreach(var breakable in otherObjects)
            {
                float otherSwingSpeed = breakable.CurrentSwingSpeed;

                if (otherSwingSpeed > impactSpeed)
                {
                    impactSpeed = otherSwingSpeed;
                }
            }
        }

        if (impactSpeed > damageThresholdSpeed)
        {
            healthSystem.Damage(impactSpeed * damageMultiplier);
        }
    }

    private void OnObjectBreak(object sender, System.EventArgs e)
    {
        BreakObject();
    }

    private void BreakObject()
    {
        if (objectGrabable != null)
        {
            objectGrabable.Drop(Vector3.zero);
        }

        objectRigidBody.isKinematic = true;
        objectCollider.enabled = false;
        healthBar.SetActive(false);

        intactObject.SetActive(false);

        foreach (GameObject piece in destroyedObjects)
        {
            if (piece != null)
            {
                piece.SetActive(true);
                piece.transform.SetParent(null);
            }
        }
    }

    public HealthSystem GetHealthSystem()
    {
        return healthSystem;
    }

    private void OnDestroy()
    {
        healthSystem.OnDead -= OnObjectBreak;
        healthSystem.OnDamaged -= ShowHealthBar;
    }
}
