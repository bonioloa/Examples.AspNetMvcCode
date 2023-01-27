/*dependent on
 * materialize
 * immutable
 */

//translation not part of materialize library
//for now we force universal date format 
//freeze must be used instead of immutable

var ConfigDatePicker = Object.freeze({
    firstDay: 1,
    format: 'yyyy-mm-dd',
    showClearBtn: true,
    i18n: {
        cancel: 'Annulla',
        clear: 'Svuota',
        done: 'Conferma',
        months: [
            'Gennaio', 'Febbraio', 'Marzo'
            , 'Aprile', 'Maggio', 'Giugno'
            , 'Luglio', 'Agosto', 'Settembre'
            , 'Ottobre', 'Novembre', 'Dicembre'
        ],
        monthsShort: [
            'Gen', 'Feb', 'Mar', 'Apr', 'Mag', 'Giu', 'Lug'
            , 'Ago', 'Set', 'Ott', 'Nov', 'Dic'
        ],
        weekdays: ['Domenica', 'Lunedì', 'Martedì', 'Mercoledì', 'Giovedì', 'Venerdì', 'Sabato'],
        weekdaysShort: ['Dom', 'Lun', 'Mar', 'Mer', 'Gio', 'Ven', 'Sab'],
        weekdaysAbbrev: ['D', 'L', 'M', 'M', 'G', 'V', 'S']
    }
});