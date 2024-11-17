using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Jugador.Habilidades
{
    public class RavagerAbility: MovementAbility    
    {
        //De momento con cooldwon, futuro con energia
        private float cooldownTotal = 15; // Tiempo de recarga de la habilidad
        private float cooldownActual = 0; // Tiempo de recarga que tiene actualmente
        
        //private float energyCost;
        //private float maxEnergy;
        //private float currentEnergy;
        
        private ControladorNave controlador; //Controlador de la nave, para poder aumentar la velocidad
        private bool isActivated = false;   //Booleano que indica si está activado
        
        private float duracionTotal= 3; // Tiempo total que se puede usar la habilidad
        private float duracionActual = 0; // Tiempo de uso de la habilidad
        
        //Metodo para asignar atributos necesarios
        //public override void AssignAttributes(List<object> attributes)
        //{
        //    if (attributes[0] is ControladorNave)
        //    {
        //        controlador = (ControladorNave) attributes[0];
        //    }
        //    Debug.Log("Controlador nave Ravager" + controlador);
        //}
        
        //Metodo para activar habilidad: impulso de velocidad durante unos segundos
        public override void AbilityExecution()
        {
            if (CheckAvailability())
            {
                //Activar habilidad
                if (!isActivated)
                {
                    isActivated = true;
                    controlador.maxSpeed += 5;
                    controlador.acceleration += 10;
                    cooldownActual = cooldownTotal;
                    StartCoroutine("DurationCounter"); //Activa el contador de duracion de la habilidad
                }
                
                //Desactivar habilidad
                else
                {
                    AcabarHabilidad();
                }
            }
        }

        //Metodo que acabar la habilidad y restaura los valores a su modo inicial
        private void AcabarHabilidad()
        {
            if (isActivated)
            {
                isActivated = false;
                controlador.maxSpeed -= 5;
                controlador.acceleration -= 10;
                duracionActual = 0;
                StartCoroutine("CooldownCounter");
                
                //Si no funciona checkear el StartCorutine
            }
        }

        //Metodo para comprobar si se cumplen los requisitios para que se active la habilidad
        public override bool CheckAvailability()
        {
            if(cooldownActual == 0 || isActivated){ return true; }
            return false; 
        }
        

        //Metodo para ir restando cooldown a la habilidad
        IEnumerator CooldownCounter()
        {
            while (cooldownActual > 0)
            {
                yield return new WaitForSeconds(1f); // Esperar un segundo para restar
                
                cooldownActual--;
                
                Debug.Log("Recargando segundos Ravager" + cooldownActual);
            }
        }
        
        //Metodo para aumentar elcontador de duración de la habilidad
        IEnumerator DurationCounter()
        {
            while (duracionActual <= duracionTotal && isActivated)
            {
                yield return new WaitForSeconds(1f); // Esperar un segundo para restar
                
                duracionActual++;
                
                Debug.Log("Duracion segundos Ravager" + cooldownActual);
            }
            
            AcabarHabilidad();
            
        }
    }
}