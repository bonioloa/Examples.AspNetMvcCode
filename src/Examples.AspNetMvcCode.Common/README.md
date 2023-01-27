# Examples.AspNetMvcCode.Common

Codice che deve essere condiviso con tutti i progetti della solution

Suddivisione del progetto
- **AppsettingsSectionModels**: contiene le classi che rappresentano sezioni dei files appsettings.json. Referenziate nella solution tramite IOptions se la sezione non è configurabile "a caldo", IOptionsSnapshot per le sezioni modificabili "a caldo" (durante l'esecuzione dell'applicazione). 

   Ad esempio:

        IOptionsSnapshot<DataAccessRootSettings> optDataAccessRoot


- **Classes**: contiene Enum, Costanti, Regex, Eccezioni e la classe per misurare la durata di esecuzione di un metodo 

        OperationTimingLogger

- **ContextDtos**: contenitori POCO per dati di sessione che devono essere disponibili nella layer Logic e Dati

- **Extensions**: estensioni enums e POCO presenti in questo progetto
