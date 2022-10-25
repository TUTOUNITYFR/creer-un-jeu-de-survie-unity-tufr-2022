using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [Header("Other elements references")]
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private MoveBehaviour playerMovementScript;

    [Header("Health")]
    
    [SerializeField]
    private float maxHealth = 100f;
    public float currentHealth;

    [SerializeField]
    private Image healthBarFill;

    [SerializeField]
    private float healthDecreaseRateForHungerAndThirst;

    [Header("Hunger")]
    [SerializeField]
    private float maxHunger = 100f;
    public float currentHunger;

    [SerializeField]
    private Image hungerBarFill;

    [SerializeField]
    private float hungerDecreaseRate;

    [Header("Thirst")]
    [SerializeField]
    private float maxThirst = 100f;
    public float currentThirst;

    [SerializeField]
    private Image thirstBarFill;

    [SerializeField]
    private float thirstDecreaseRate;

    public float currentArmorPoints;

    [HideInInspector]
    public bool isDead = false;

    void Awake()
    {
        currentHealth = maxHealth;
        currentHunger = maxHunger;
        currentThirst = maxThirst;
    }

    void Update()
    {
        UpdateHungerAndThirstBarsFill();

        if(Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(50f);
        }
    }

    public void TakeDamage(float damage, bool overTime = false)
    {
        if(overTime)
        {
            currentHealth -= damage * Time.deltaTime;
        } else
        {
            currentHealth -= damage * (1 - (currentArmorPoints / 100));
        }

        if(currentHealth <= 0 && !isDead)
        {
            Die();
        }
        
        UpdateHealthBarFill();
    }

    private void Die()
    {
        Debug.Log("Player died !");
        isDead = true;

        // Bloque le mouvement du joueur + mode inspection
        playerMovementScript.canMove = false;

        // On bloque la diminution des barres de faim et soif
        hungerDecreaseRate = 0;
        thirstDecreaseRate = 0;

        animator.SetTrigger("Die");
    }

    public void ConsumeItem(float health, float hunger, float thirst)
    {
        currentHealth += health;

        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        currentHunger += hunger;

        if(currentHunger > maxHunger)
        {
            currentHunger = maxHunger;
        }

        currentThirst += thirst;

        if(currentThirst > maxThirst)
        {
            currentThirst = maxThirst;
        }

        UpdateHealthBarFill();
    }

    void UpdateHealthBarFill()
    {
        healthBarFill.fillAmount = currentHealth / maxHealth;
    }

    void UpdateHungerAndThirstBarsFill()
    {
        // Diminue la faim / soif au fil du temps
        currentHunger -= hungerDecreaseRate * Time.deltaTime;
        currentThirst -= thirstDecreaseRate * Time.deltaTime;

        // On empêche de passer dans le négatif
        currentHunger = currentHunger < 0 ? 0 : currentHunger;
        currentThirst = currentThirst < 0 ? 0 : currentThirst;

        // Mettre à jour les visuels
        hungerBarFill.fillAmount = currentHunger / maxHunger;
        thirstBarFill.fillAmount = currentThirst / maxThirst;

        // Si la barre de faim et/ou soif est à zéro -> Le joueur prend des dégâts (x2 si les deux barres sont à zéro)
        if(currentHunger <= 0 || currentThirst <= 0)
        {
            TakeDamage((currentHunger <= 0 && currentThirst <= 0 ? healthDecreaseRateForHungerAndThirst * 2 : healthDecreaseRateForHungerAndThirst), true);
        }
    }
}
