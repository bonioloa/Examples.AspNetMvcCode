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

- i metodi raramente sono asincroni per requisito espressa, in ogni caso l'hardware originale non � in grado di sfruttarne le potenzialit� in quanto single core

- la parte di validazione files non � ancora stata implementata

-non � permesso il caching dei file statici per evitare il permanere di file non aggiornati nella cache dei client browser. Una implementazione per il cache busting � stata messa a bassa priorit�