# Examples.AspNetMvcCode


Esempio di architettura n-tier

Scaletta dipendenza

Referenziate da tutti i progetti
> Examples.AspNetMvcCode.CodeUtility
> Examples.AspNetMvcCode.Common
> Examples.AspNetMvcCode.Localization

I seguenti a cascata
> Examples.AspNetMvcCode.Data

> Examples.AspNetMvcCode.Logic

> Examples.AspNetMvcCode.Web


vedere i relativi readme per ogni progetto per spiegazioni


NOTE:

- i metodi raramente sono asincroni per requisito espressa, in ogni caso l'hardware originale non è in grado di sfruttarne le potenzialità in quanto single core

- la parte di validazione files non è ancora stata implementata

-non è permesso il caching dei file statici per evitare il permanere di file non aggiornati nella cache dei client browser. Una implementazione per il cache busting è stata messa a bassa priorità