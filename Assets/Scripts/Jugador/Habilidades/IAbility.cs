using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAbility
{
    //Metodo para activar habilidad
    public void Execute();       
    //Metodo para comprobar si se cumplen los requisitios para que se active la habilidad
    public bool CheckAvailability();   
    
    //Metodo para asignar atributos necesarios
    public void AssignAttributes(Dictionary<string, object> attributes);
}
