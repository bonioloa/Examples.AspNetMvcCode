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
        cancel: 'Anulado',
        clear: 'Limpia',
        done: 'Confirma',
        months: [
            "Enero", "Febrero", "Marzo"
            , "Abril", "Mayo", "Junio"
            , "Julio", "Agosto", "Septiembre"
            , "Octubre", "Noviembre", "Diciembre"
        ],
        monthsShort: [
            "Ene", "Feb", "Mar", "Abr", "May", "Jun"
            , "Jul", "Ago", "Set", "Oct", "Nov", "Dic"
        ],
        weekdays: ["Domingo", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado"],
        weekdaysShort: ["Dom", "Lun", "Mar", "Mie", "Jue", "Vie", "Sab"],
        weekdaysAbbrev: ["D", "L", "M", "M", "J", "V", "S"]
    }
});