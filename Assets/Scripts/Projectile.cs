using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Stats")]
    public float speed = 10f;
    public float lifetime = 3f;
    public bool isHoming = true;
    
    [Header("Visual Effects")]
    public GameObject hitEffect;
    
    private Enemy target;
    private float damage;
    private float currentLifetime;
    
    private void Start()
    {
        currentLifetime = lifetime;
    }
    
    private void Update()
    {
        currentLifetime -= Time.deltaTime;
        
        if (currentLifetime <= 0)
        {
            DestroyProjectile();
            return;
        }
        
        if (isHoming && target != null && target.gameObject.activeInHierarchy)
        {
            // Đạn tự dẫn: Di chuyển về phía mục tiêu
            Vector3 direction = (target.transform.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
            
            // Xoay đạn để hướng về mục tiêu
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        else
        {
            // Đạn thẳng: Di chuyển theo hướng ban đầu
            transform.position += transform.right * speed * Time.deltaTime;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            // Deal damage
            enemy.TakeDamage(damage);
            
            // Create hit effect
            if (hitEffect != null)
            {
                Instantiate(hitEffect, transform.position, transform.rotation);
            }
            
            // Destroy projectile
            DestroyProjectile();
        }
    }
    
    public void SetTarget(Enemy enemy)
    {
        target = enemy;
    }
    
    public void SetDamage(float damageAmount)
    {
        damage = damageAmount;
    }
    
    private void DestroyProjectile()
    {
        Destroy(gameObject);
    }
}
