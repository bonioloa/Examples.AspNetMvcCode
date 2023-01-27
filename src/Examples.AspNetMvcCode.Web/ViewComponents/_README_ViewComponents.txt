per tutte le classi che ereditano da ViewComponent usare il suffisso "Comp"

evitare di usare la sintassi con tag helper, poiché renderà impossibile tracciare le referenze di utilizzo del viewcomponent

fare attenzione ad eventuali rename, MVC cercherà le views del viewcomponent nei seguenti subpath

Views\(controller della view dove viene richiamato il view component)\Components\(nome classe view component)
Views\Shared\Components\(nome classe view component)

quindi per eventuali rename della classe sarà obbligatorio rinominare la cartella associata nei paths indicati
