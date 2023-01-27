Convenzione:
in questa cartella vanno salvati: 
- oggetti JSON definiti da librerie javascript, necessari alla relativa configurazione
- contenitori di SOLI oggetti JSON serializzati


in generale se si deve restituire una stringa JSON serializzata come parte di un viewModel
non usare un campo string ma creare un oggetto a parte per contenere questi campi
(Es: i dati json per configurare i grafici riportati dalla pagina report)
