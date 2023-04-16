# Algoritmo

## General

- Leer tabla de tokens
- Escribir tabla de símbolos
- Actualizar tabla de tokens con posiciones en tabla de símbolos
- Lanzar errores de semántica


## Paso a paso

- Leer archivo de tabla de tokens
- Separlo por lineas y luego por comas
- Guardar los tokens en una lista
- Obtener identificadores dentro del bloque de `var` (que tengan una posicion de -2)
- Checar que no haya identificadores repetidos, si hay, lanzar un error
- Añadir los identificadores a la tabla de simbolos y asignarles un valor default
- Actualizar cada token con la posición en la tabla de simbolos
- Escribir las tablas de tokens y de simbolos en archivos
- Revisar el uso de los identificadores, es decir, verificar que existan en la tabla de simbolos
- Si un identificador no está en la tabla de simbolos, lanzar un error
- Si no se le asigna un valor del tipo correcto, lanzar un error
- Checar que el identificador sea del mismo tipo que el que tiene asignado en la tabla de simbolos


# Vector de código intermedio

- Leer tabla de tokens que contiene el código de todo un programa
- Meter en VCI todas las expresiones desde el primer `inicio` hasta el ultimo `fin`
- Salida: el contenido del VCI separado por comas

# Algoritmo

- Inicio se ignora
- Si el token es un `;` vaciar pila de operadores.
- Si es un `)` vaciar pila de operadores hasta encontrar un `(`.
- Leer token
  - Si es identificador, constante o funcion, mandar directo a VCI
  - Si es operador, entra en la pila de operadores con su prioridad correspondiente.
    - Si el operador es `(`, se mete directo a la pila
    - Si la prioridad del operador en la pila es menor a la del operador actual, se mete directo a la pila
    - Si la prioridad del operador en al pila es mayor o igual a la del operador actual, se saca el operador de la pila y se mete al VCI
  - Si es estatuto, se guarda en pila de estatutos
    - Si el estatuto es `repeat`: se almacena la direccion en la pila de direcciones
    - Cuando llego a `end`, si el estatuto en la pila es `repeat`, verifico que lo siguiente sea un `unitl` lo guardo momentaneamente y escribo la condicion del hasta en VCI, o sea, hasta el `;`, llegando al `;` escribir la direccion de la pila de direcciones y el `until`.
