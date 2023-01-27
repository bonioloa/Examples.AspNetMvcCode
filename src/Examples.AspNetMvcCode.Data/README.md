# Examples.AspNetMvcCode.Data

Contiene sia i metodi per astrarre l'uso di ADO/Dapper, sia i servizi e le interfacce per l'interrogazione dei database.

Tutti gli input utente sono passati tramite parametri. Tutte le volte che i comandi SQL hanno concatenazioni di stringhe, si tratta di dati che sono interni all'applicativo e non provengono in nessun modo da input utente.

- **Constants**:  
    - metodi statici per generare codice SQL usato frequentemente, tipo date in formato stringa 
    - definizioni parametri SQL

- **DataAccess**
  - *DataCommandManager*: Classe astratta per fare chiamate con ADO e Dapper, da implementare con tutte le classi che rappresentano un tipo di base dati. Qui abbiamo *DataCommandManagerRoot* che viene interrogato quando l'utente fornisce il token di identificazione token. Con le informazioni recuperate si può inizializzare la class *DataCommandManagerTenant* per interrogare la base dati dedicata al tenant. Le operazioni di inizializzazione delle connection strings vengono fatte nella layer Logic

- Dto: contiene le classi e record poco per trasferimento dati con la layer Logic

- ServiceCrypting: contiene i metodi per la criptazione di stringhe e dati

- ServiceQueries: contiene i servizi di interrogazione delle basi dati con SQL esplicito, il pattern è simil-repository

- UnitOfWork: contiene aggregazioni di chiamate a servizi di scrittura dati per garantire l'esecuzione all'interno di un'unica transazione e separare l'orchestrazione dalla layer Logic