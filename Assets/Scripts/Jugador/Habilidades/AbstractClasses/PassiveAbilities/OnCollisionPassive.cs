using DefaultNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OnCollisionPassive : PassiveAbility
{
    protected IDamageable target;
    [SerializeField] protected List<Collider> affectedArea;
    public bool recievesCollisionDamage = false;

    private void Awake()
    {
        type = PassiveType.OnCollision;
    }

    public void CollideWith(Collision other)
    {
        Debug.Log("Te has chocado con una pasiva de chocarse");
        target = other.gameObject.GetComponentInParent<IDamageable>();
        
        if(IsInArea(other.collider))
        {
            Execute();
        }
    }

    public bool IsInArea(Collider other)
    {
        if (affectedArea.Count == 0) return true;

        foreach(Collider area in affectedArea) 
        {
            if (area != null && area.bounds.Intersects(other.bounds))  // Verifica si el área de colisión se solapa
            {
                // Si hay intersección, ejecutamos lo que sea necesario
                Debug.Log("El objeto está dentro del área afectada.");
                return true;
            }
        }
        return false;
    }
}
