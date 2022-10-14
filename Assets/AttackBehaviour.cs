using UnityEngine;

public class AttackBehaviour : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private Equipment equipmentSystem;

    void Update()
    {
        if(Input.GetMouseButtonDown(0) && equipmentSystem.equipedWeaponItem != null)
        {
            animator.SetTrigger("Attack");
        }
    }
}
