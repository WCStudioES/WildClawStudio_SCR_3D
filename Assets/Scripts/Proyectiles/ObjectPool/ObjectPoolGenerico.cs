using System.Collections.Generic;
public class ObjectPoolGenerico : IObjectPool
{
    //PROTOTIPO DE LOS ELEMENTOS QUE CONTENDRA EL POOL
    private IPooleableObject prototipo;
    //INDICA SI ESTA PERMITIDA LA CREACIÓN DE NUEVOS ELEMENTOS
    private bool permitirLaCreacionDeNuevosElementos;
    //LISTA DE ELEMENTOS
    private List<IPooleableObject> listaDeElementos;
    //NUMERO DE ELEMENTOS ACTIVOS
    private int elementosActivos = 0;

    //CONSTRUCTOR
    public ObjectPoolGenerico(IPooleableObject prototipo, int numeroDeElementosInicial, bool permitirLaCreacionDeNuevosElementos)
    {
        this.prototipo = prototipo;
        listaDeElementos = new List<IPooleableObject>();
        this.permitirLaCreacionDeNuevosElementos = permitirLaCreacionDeNuevosElementos;
        for (int i = 0; i < numeroDeElementosInicial; i++)
        {
            IPooleableObject nuevoElemento = crearNuevoElemento();
            if(nuevoElemento != null)
                listaDeElementos.Add(nuevoElemento);
        }
    }
    
    //DEVUELVE UN ELEMENTO QUE NO ESTUVIERA ACTIVO, SI NO LO HAY DEVUELVE NULL
    public IPooleableObject Get()
    {
        //RECORRE LA LISTA DE ELEMENTOS, DEVUELVE EL PRIMERO QUE ENCUENTRE QUE NO ESTE ACTIVO
        if (elementosActivos < listaDeElementos.Count)
        {
            foreach (IPooleableObject elemento in listaDeElementos)
            {
                if (!elemento.Active)
                {
                    elemento.Active = true;
                    elementosActivos++;
                    return elemento;
                }
            }
        }
        //SI SE PERMITE LA CREACIÓN DE NUEVOS ELEMENTOS, CREA UNO Y LO DEVUELVE
        else if(permitirLaCreacionDeNuevosElementos)
        {
            IPooleableObject nuevoElemento = crearNuevoElemento();
            if (nuevoElemento != null)
            {
                nuevoElemento.Active = true;
                listaDeElementos.Add(nuevoElemento);
                elementosActivos++;
                return nuevoElemento;
            }
        }
        return null;
    }

    //DESACTIVA Y RESETEA UN ELEMENTO
    public void Release(IPooleableObject elemento)
    {
        elemento.Active = false;
        elemento.Reset();
        elementosActivos--;
    }

    //CREA Y DEVUELVE UN NUEVO ELEMENTO
    private IPooleableObject crearNuevoElemento()
    {
        IPooleableObject nuevoElemento = prototipo.Clone();
        return nuevoElemento;
    }
}