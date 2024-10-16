<p align="center">
 <img width=1000 heigth=1000 src="https://github.com/jagonmes/Imagenes-JeR/blob/main/SCRLogoNegro.png"> <br>
</p>

# Índice
---

 1. [Introducción](#introducción)
    * [Concepto](#concepto)
    * [Historia y trama](#historia-y-trama)
    * [Propósito, público objetivo y plataformas](#propósito-público-objetivo-y-plataformas)
 3. [Mecánicas y elementos del juego](#mecánicas-y-elementos-del-juego)
    * [Descripción detallada del concepto de juego](#descripción-detallada-del-concepto-de-juego)
    * [Descripción de mecánicas de juego](#descripción-de-mecánicas-de-juego)
    * [Controles](#controles)
    * [Objetivo](#objetivo)
    * [Objetos, armas y power ups](#objetos-armas-y-power-ups)
 5. [Arte](#arte)
    * [Estética general del juego](#estética-general-del-juego)
    * [Referencias del arte Space Combar Rush 2D](#referencias-del-arte-space-combar-rush-2d)
    * [Concepts](#concepts)
    * [Modelos 3D](#modelos-3d)
 7. [Sonido](#sonido)
    * [Música de ambiente](#música-de-ambiente)
    * [SFX](#sfx)
 9. [Interfaz](#interfaz)
    * [Referencias de interfaz](#referencias-de-interfaz)
    * [Diseños de menús](#diseños-de-menús)
    * [Diagrama de flujo](#diagrama-de-flujo)
 13. [Monetización](#monetización)
     * [Modelo de negocio](#modelo-de-negocio)
     * [Roadmap](#roadmap)
     * [Información del usuario](#información-del-usuario)
     * [Mapa de empatía](#mapa-de-empatía)
     * [Modelo de caja de herramientas](#modelo-de-caja-de-herramientas)
     * [Modelo de canvas o lienzo](#modelo-de-canvas-o-lienzo)
     * [Modelo de monetización](#modelo-de-monetización)
     * [Tabla de productos y precios](#tabla-de-productos-y-precios)
 15. [Planificación y costes](#planificación-y-costes)
     * [Equipo humano](#equipo-humano)
     * [Estimación temporal del desarrollo](#estimación-temporal-del-desarrollo)
     * [Costes asociados](#costes-asociados)
     * [Hoja de ruta del desarrollo](#hoja-de-ruta-del-desarrollo)
 18. [Post mortem (lecciones aprendidas)](#post-mortem-lecciones-aprendidas)
     * [Trabajo individual realizado](#trabajo-individual-realizado)
         * [Anastasia Ihnatsenka Shakhova](#anastasia-ihnatsenka-shakhova)
         * [José Antonio González Mesado](#josé-antonio-gonzález-mesado)
         * [Jesús Bastante López](#jesús-bastante-lópez)
         * [Jorge Juán Xuclá Esparza](#jorge-juán-xuclá-esparza)
         * [Miguel Ángel Jimenez Montemayor](#miguel-ángel-jimenez-montemayor)
     * [Trabajo colectivo realizado](#trabajo-colectivo-realizado)

# Introducción
---
## Concepto
"Space Combat Rush" es un juego multijugador con vista cenital en el que dos o más jugadores se enfrentarán en batallas de naves espaciales en un campo de asteroides. Cada jugador controlará una nave y su misión será acabar con el resto de jugadores.
## Historia y trama
Con el avance de la tecnología y la ingeniería espacial, la humanidad ha conseguido extenderse a través del cosmos. La población humana crece exponencialmente, los recursos obtenidos de las colonias son casi infinitos y las naves cada vez más extravagantes y potentes. En este contexto nació Space Combat Rush, el deporte más letal y espectacular a escala galáctica. La gloria y la fama esperan a nuestros pilotos en un emocionante combate 1vs1 lleno de metal, plasma y mucho fuego en el que se lo juegan todo.

¿Quién caerá?

¿Quién saldrá victorioso?

Estas preguntas solo se contestan en la arena.

## Propósito, público objetivo y plataformas
Con este juego se buscará ocupar el tiempo muerto de cualquier jugador en momentos como un viaje en el metro, la espera del comienzo de una clase o similar. Principalmente nos centraremos en el público masculino entre las edades de 12 a 18 años, con posibilidad a adultos jóvenes entre los 18 y 25, y se subirá a los navegadores de Google Chrome, Edge y Mozilla Firefox para que tengan fácil acceso al juego.

# Mecánicas y elementos del juego
---
## Descripción detallada del concepto de juego
Space Combat Rush 3D se basará fuertemente en el juego ya creado previamente Space Combat Rush. Se podría considerar como la transferencia de la versión original a un mundo 3D; pero con algunas diferencias. El juego buscará destacar por su sencillez donde, a parte de los menús a través de los que puede navegar el jugador, solamente habrá una única pantalla de juego.

La pantalla de juego consistirá en un entorno cerrado (un mundo con límites) que simulará el espacio. Aquí el jugador podrá encontrar al enemigo al que tendrá que enfrentarse (el otro jugador con el que ha sido emparejado en un 1vs1, se replanteará en un futuro si aumentarlo a batallas por equipos) y elementos como meteoritos y debris espacial (elementos que el jugador podrá destruir disparándoles).

El usuario jugará en tercera persona desde una vista cenital controlando una nave. Podrá personalizar su nave con las skins básicas o adquiriendo, con los pases de batalla, skins especiales. En un principio estas skins serán meramente estéticas, pero se reevaluará en el futuro si añadir efectos especiales que puedan tener algunas.

Además, el jugador podrá adquirir power-ups a base de la destrucción de los meteoritos que le puedan prestar una boost o ayuda temporal para luchar contra su enemigo. Se plantea también, para revisiones futuras, la adición de boosters especiales que aparezcan en el mapa como drops con aviso para obligar una lucha más encarecida entre los jugadors.
## Descripción de mecánicas de juego
El juego principalmente tendrá como mecánicas la destrucción de la nave enemiga y la de meteoritos o debris del entorno para quitarlos del medio y/o conseguir boosters. El jugador controlará su nave en movimiento y disparo.
## Controles
Para el movimieto de la nave, en versión dispositivo con teclado, se usarán WAD (donde W es para acelerar y AD es para el control de la dirección, no se puede volar hacia atrás) o flechas arriba, izquierda y derecha. Para la versión en móvil **--completar--**

En cuanto a los disparos, en versión ordenador se utilizará la barra espaciadora. En cuanto a la versión dispositivo táctil **--completar--**

Y, finalmente, para navegar entre las distintas pantallas, se utilizará el ratón para clickar sobre los distintos botones. En versión móvil se hará utilizando la funcionalidad táctil de esta o el mando.
## Objetivo
El objetivo en este juego es simple, conseguir matar a todo jugador al que te enfrentes para ascender en el ranking que se visualizará al terminar una partida. Se apelará a la competitividad de los jugadores para que busquen conseguir estar en el top entre todos los usuarios.
## Objetos, armas y power ups
Content

# Arte
---
## Estética general del juego
El juego buscará simular una estética arcade en 3D. Por lo que se apelará a una mezcla futurista/retro con modelos low poly. Buscaremos tener una mezcla de diseños que recuerden a los tiempos de antaño, pero con detalles de luces neón y elementos futurísticos para dar la sensación de que nos encontramos ante una sociedad más desarrollada de lo que fueron esos tiempos.
## Referencias del arte Space Combar Rush 2D
El juego original se diseñó para una estética pixelar apelando en su totalidad a los juegos retro del pasado. A continuación se podrá ver algunas imágenes de dicho juego y se pondrá el enlace al juego para poder probarlo.

<p align="center">
 <img width=145 heigth=200 src="https://github.com/jagonmes/Imagenes-JeR/blob/main/50e92bb3f19423b47d032f11a80fe288.jpg">
 <img width=200 heigth=200 src="https://github.com/jagonmes/Imagenes-JeR/blob/main/raf%2C360x360%2C075%2Ct%2Cfafafa_ca443f4786.jpg">
 <img width=400 heigth=200 src="https://github.com/jagonmes/Imagenes-JeR/blob/main/retrofuturyzm-5.jpeg"> <br>
</p>
<p align="center">
 <img width=200 heigth=200 src="https://github.com/jagonmes/Imagenes-JeR/blob/main/thumbnail_8b978dc5-9927-441c-85ba-44224c1fe562.jpg.256x256_q85.jpg">
 <img width=200 heigth=200 src="https://github.com/jagonmes/Imagenes-JeR/blob/main/77a7a10aca1740618fa4ad57e0c7eed9.jpg">
 <img width=355 heigth=200 src="https://github.com/jagonmes/Imagenes-JeR/blob/main/1.jpg"> <br>
</p>

## Concepts
Principalmente nos centraremos en los diseños de las naves, pero elementos del entorno como los meteoritos y el debris espacial también deberán ser diseñados. Así como los distintos boostes y elementos 2D como la interfaz. A continaución se presentan los diseños.

Diseño de la nave **RAVAGER**:
<p align="center">
 <img width=800 heigth=800 src="https://github.com/jagonmes/Imagenes-JeR/blob/main/RavagerCD.png"> <br>
</p>

## Modelos 3D
En este apartado se mostrarán las versiones 3D de los concepts para tener una clara comprensión del contenido visual dentro del juego.

# Sonido
---
## Música de ambiente
Content
## SFX
Content

# Interfaz
---
## Referencias de interfaz
Aunque este juego se basa en su versión arcade, para adaptarlo al nuevo público y añadir el toque futurista que  buscamos en el concepto del juego en su versión 3D, las interfaces tomarán referencia de juegos con mayor tendencia a lo tecnológico-futurista. Sin embargo, buscaremos mantener la idea del arcade en cuanto a los colores utilizados; por lo que habrá una tendencia a añadir colores fosforitos y chillones.

Además, al ser un juego competitivo con la necesidad de tener buen manejo o control de la nave y la mayor velocidad posible para acabar con el enemigo, tomará influencias también de las interfaces de juegos de carreras. Algunas de estas referencias, de forma general, son las siguientes:

<p align="center">
 <img width=320 heigth=320 src="https://github.com/jagonmes/Imagenes-JeR/blob/main/login-form-page-on-blue-background-technology-futuristic-interface-background-vector.jpg">
 <img width=210 heigth=210 src="https://github.com/jagonmes/Imagenes-JeR/blob/main/sign-form-on-hud-futuristic-260nw-2341185149.jpg">
 <img width=360 heigth=200 src="https://github.com/jagonmes/Imagenes-JeR/blob/main/blastonui24.png"> <br>
</p>
<p align="center">
 <img width=300 heigth=300 src="https://github.com/jagonmes/Imagenes-JeR/blob/main/598f6625615359.57cc2076364ef.png">
 <img width=300 heigth=300 src="https://github.com/jagonmes/Imagenes-JeR/blob/main/ff34f1da872e07410f4bd1a0ef3f4743.jpg">
 <img width=300 heigth=300 src="https://github.com/jagonmes/Imagenes-JeR/blob/main/z4dyiyu6amb71.jpg"> <br>
</p>

## Diseños de menús
Antes de diseñar los distintos elementos finales de los menús, se realizan unos concepts para visualizar de manera rápida y sencilla la navegación visual de cada una de ellas. Estos diseños son los siguientes:

<p align="center">
 <img width=500 heigth=500 src="https://github.com/jagonmes/Imagenes-JeR/blob/main/ConceptsInterfaceGDD-SCR3D/1.jpg">
 <img width=500 heigth=500 src="https://github.com/jagonmes/Imagenes-JeR/blob/main/ConceptsInterfaceGDD-SCR3D/2.jpg"> <br>
</p>
<p align="center">
 <img width=500 heigth=500 src="https://github.com/jagonmes/Imagenes-JeR/blob/main/ConceptsInterfaceGDD-SCR3D/3.jpg">
 <img width=500 heigth=500 src="https://github.com/jagonmes/Imagenes-JeR/blob/main/ConceptsInterfaceGDD-SCR3D/4.jpg"> <br>
</p>
<p align="center">
 <img width=500 heigth=500 src="https://github.com/jagonmes/Imagenes-JeR/blob/main/ConceptsInterfaceGDD-SCR3D/5.jpg">
 <img width=500 heigth=500 src="https://github.com/jagonmes/Imagenes-JeR/blob/main/ConceptsInterfaceGDD-SCR3D/8.jpg"> <br>
</p>
<p align="center">
 <img width=500 heigth=500 src="https://github.com/jagonmes/Imagenes-JeR/blob/main/ConceptsInterfaceGDD-SCR3D/6.jpg">
 <img width=500 heigth=500 src="https://github.com/jagonmes/Imagenes-JeR/blob/main/ConceptsInterfaceGDD-SCR3D/7.jpg"> <br>
</p>
<p align="center">
 <img width=500 heigth=500 src="https://github.com/jagonmes/Imagenes-JeR/blob/main/ConceptsInterfaceGDD-SCR3D/13.jpg">
 <img width=500 heigth=300 src="https://github.com/jagonmes/Imagenes-JeR/blob/main/ConceptsInterfaceGDD-SCR3D/15.jpg"> <br>
</p>
<p align="center">
 <img width=500 heigth=500 src="https://github.com/jagonmes/Imagenes-JeR/blob/main/ConceptsInterfaceGDD-SCR3D/16.jpg">
 <img width=500 heigth=500 src="https://github.com/jagonmes/Imagenes-JeR/blob/main/ConceptsInterfaceGDD-SCR3D/12.jpg"> <br>
</p>
<p align="center">
 <img width=500 heigth=500 src="https://github.com/jagonmes/Imagenes-JeR/blob/main/ConceptsInterfaceGDD-SCR3D/14.jpg">
 <img width=500 heigth=500 src="https://github.com/jagonmes/Imagenes-JeR/blob/main/ConceptsInterfaceGDD-SCR3D/9.jpg">  <br>
</p>
<p align="center">
 <img width=500 heigth=500 src="https://github.com/jagonmes/Imagenes-JeR/blob/main/ConceptsInterfaceGDD-SCR3D/11.jpg">
 <img width=500 heigth=500 src="https://github.com/jagonmes/Imagenes-JeR/blob/main/ConceptsInterfaceGDD-SCR3D/10.jpg"> <br>
</p>

A partir de estos diseños conceptuales, se procede a crear el material necesario para traducirlo a interfaces visuales atractivas.

## Diagrama de flujo
Teniendo el diseño de las interfaces conceptuales, se diseña además cómo se pasa dentre las distintas pantallas. Para ello, creamos el siguiente flujograma.

<p align="center">
 <img width=100% heigth=100% src="https://github.com/jagonmes/Imagenes-JeR/blob/main/Flujograma%20SCR3D.png"> <br>
</p>

# Monetización
---
## Modelo de negocio
Se plantea un modelo Freemium ya que daremos el juego con acceso gratuito para todos los jugadores. Sin embargo, para todo aquel que desee más contenido, se le dará la opción de poder optar a la compra de Battle Passes. En él, se podrá realizar colaboraciones con otras empresas para crear skins o elementos similares con los que se pueden publicitar estas empresas.

Además, se realizarán tornos bianuales de pago como eventos que mantengan la base de nuetra comunidad activa constantemente y fidelizada.
## Roadmap
Content
## Información del usuario
El usuario destinatario de nuestro juego tendrá las siguientes características:

  * Pertenece al segmento de edad entre los 12 y 18 años.
  * Son estudiantes.
  * Se apelará al género masculino principalmente.
  * Son personas con mucho tiempo libre.
  * Tienden a tener falta de socialización en persona.
  * Son competitivos.
## Mapa de empatía
Conociendo las características base de quienes serán nuestros principales jugadores, procedemos a estudiarlo más de cerca con un mapa de empatía.

<p align="center">
 <img width=1000 heigth=1000 src="https://github.com/jagonmes/Imagenes-JeR/blob/main/Mapa%20Empat%C3%ADa%20SCR3D.png"> <br>
</p>

## Modelo de caja de herramientas
Utilizando este modelo, construimos nuestra empresa en base a una serie de bloques y las relaciones existentes entre ellos. Esto nos permite visualizar la estructura general del funcionamiento de Wild Claw Estudio (específicamente basado en el juego de Space Combat Rush 3D).

<p align="center">
 <img width=1000 heigth=1000 src="https://github.com/jagonmes/Imagenes-JeR/blob/main/CajaHerramientas.png"> <br>
</p>

## Modelo de canvas o lienzo
Gracias al modelo de lienzo, podemos representar nuetra infraestructura orientada al producto de Space Combat Rush 3D de manera más detallada. Ello se puede visualizar en la siguiente representación gráfica:
<p align="center">
 <img width=1000 heigth=1000 src="https://github.com/jagonmes/Imagenes-JeR/blob/main/ModeloLienzo.png"> <br>
</p>

## Modelo de monetización
Space Combat Rush se centrará en el modelo de Battle Pass y la generación de torneos bianuales con entrada de pago.
## Tabla de productos y precios
| Producto | Precio |
|--------|-------|
| Juego | Gratuito|
| Battle Pass | 10€ cada 3 meses |
| Torneos Bianuales | 3€ cada 6 meses |

# Planificación y costes
---
## Equipo humano
El equipo constará de 5 miembros core. Estos serán los siguientes; junto al departamente al que pertenecen:

  * **Miguel Ángel Jimenez Montemayor**: departamentos de arte y diseño.
  * **Jesús Bastante López**: departamentos de diseño y programación.
  * **Jorge Juan Xuclá Esparza**: departamentos de programación y música.
  * **Anastasia Ihnatsenka Shakhova**: departamentos de arte y diseño.
  * **José Antonio González Mesado**: departamento de programación.

Independientemente del departamento al que pertenezca cada uno y de su especialización, se buscará la participación en todos los aspectos del proyecto tanto de opinión como en las ayudas que se necesiten dependiendo de las necesidades del proyecto en el momento.
## Estimación temporal del desarrollo
La duración del desarrollo del proyecto tiene una estimación de un periodo de 3 a 4 meses; incluyendo un tiempo añadido para incoventientes y posibles arreglos de errores.
## Costes asociados
El coste del desarrollo se estima en unos **50.000€**. Esto equivaldría a 4 meses de sueldo de los miembros del equipo (1.400€ brutos más las cotizaciones, aproximadamente 37.500€), unos 5.000€ para el hardware y software, 2.500€ para los servidores y otras infraestructuras, y otros 5.000€ para la publicidad. Véase la siguiente tabla para tenerlo de forma más visual:

| Elemento | Coste |
|----------|-------|
| Equipo 5 personas | 37.500€ |
| Hardware y software | 5.000€ |
| Servidores y otras infraestructuras | 5.000€ |
| Publicidad | 5.000€ |
| **Total** | **50.000€** |

# Hoja de ruta del desarrollo
---
A continuación se detallan las fechas estimadas.

| Fecha | Información |
|-------|-------------|
| 23/09/2024 | Inicio del proyecto |
| 14/10/2024 | Inicio del desarrollo del prototipo |
| 27/10/2024 | Entrega del primer prototipo jugable |
| 24/11/2024 | Entrega de la beta con todos los sistemas del videojuego funcionales |
| 08/12/2024 | Entrega de la versión gold master |
| 18/12/2024 | Lanzamiento del juego y del primer battle pass |
| 01/04/2025 | Lanzamiento del segundo battle pass |
| 15/06/2025 - 30/06/2025 | Primer Torneo de SCR 3D |
| 01/07/2025 | Lanzamiento del tercer battle pass |
| 01/10/2025 | Lanzamiento del cuarto battle pass |
| 15/12/2025 - 31/12/2025 | Segundo Torneo de SCR 3D |
| 01/01/2026 | Lanzamiento del quinto battle pass |
| 01/04/2026 | Lanzamiento del sexto battle pass |
| 15/06/2026 - 30/06/2026 | Tercer Torneo de SCR 3D |
| 01/07/2026 | Lanzamiento del séptimo battle pass |
| 01/10/2026 | Lanzamiento del octavo battle pass |
| 15/12/2026 - 31/12/2026 | Cuarto Torneo de SCR 3D |
# Post mortem (lecciones aprendidas)
---
## Trabajo individual realizado
### Anastasia Ihnatsenka Shakhova
Content
### José Antonio González Mesado
Content
### Jesús Bastante López
Content
### Jorge Juán Xuclá Esparza
Content
### Miguel Ángel Jimenez Montemayor
Content
## Trabajo colectivo realizado
Content
