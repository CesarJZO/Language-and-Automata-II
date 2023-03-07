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
