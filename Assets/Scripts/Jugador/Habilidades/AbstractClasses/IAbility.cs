using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAbility
{
    //Metodo para asignar atributos necesarios
    //public void AssignAttributes(List<object> attributes);

    //Metodo para llamar cuando se pulsa el botón de una habilidad activa o cuando se da el caso de uso en una pasiva
    public void Execute();
    //Metodo para comprobar si se cumplen los requisitios para que se active la habilidad
    public bool CheckAvailability();   
    //Metodo para activar habilidad
    public void AbilityExecution();

    
}
