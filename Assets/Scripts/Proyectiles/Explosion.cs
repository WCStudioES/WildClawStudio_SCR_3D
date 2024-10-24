using Unity.Netcode;
using UnityEngine;

namespace DefaultNamespace.Proyectiles
{
    public class Explosion: NetworkBehaviour
    {
        public int dmg;
        
        //Funcion para iniciar la explosion
        public void Crear(int dañoPadre)
        {
            Debug.Log("Explosion creada");
            dmg = dañoPadre;
            Invoke("DestruirMisilServer", 1f);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log(other.gameObject.name);
            //Al colisionar comprueba si el otro ente puede recibir daño
            ICanGetDamage target = other.gameObject.GetComponentInParent<ICanGetDamage>();
            if (target == null)
                { other.gameObject.GetComponent<ICanGetDamage>();}
            Debug.Log(target);
            if (target != null)
            {
                //Debug.Log("Entro al if de target");
                target.GetDamage(dmg);
            }
        }
        
        protected void DestruirMisilServer()
        {
            Debug.Log("Explosion destruida");
            //DestruirMisilClientRpc();
            Destroy(gameObject);
        }
        
        /*[ClientRpc]
        protected void DestruirMisilClientRpc()
        {
            //Debug.Log("Explosion destruida");
            gameObject.SetActive(false);
        }*/
    }
}