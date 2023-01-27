//dependencies
//jquery
//immutable.js
//site.js
//chart.js
//chartjs-plugin-datalabels
//color-distinct-generator
//SharedConstStatisticsPage -  page view
//SharedVarPieGraphConfig - page view
//SharedConstKeyLine - page view

/*NOTA: chart.js  da versione 3 in poi non è supportato da IE 11 e IE 10
 da ripristinare versione 2.9.4 se richiesto da cliente ne ha bisogno
 dovrà essere riscritto il codice per inizializzare il grafico 
 e si perderà il contatore elementi per area grafico*/
$(function () {
});


var chart;

$(window).on('load', function () {
    //don't set size from body

    if (!SharedConstStatisticsPage.get('ShowGraphic')) {
        return;
    }

    var chartArea = document.getElementById("chart-area");
    if (chartArea != null) //controllo altrimenti saltano i componenti devexpress
        buildChartAndLegend(chartArea);
});



function buildChartAndLegend(chartArea) {
    if (chart) {
        chart.destroy();
    }

    //setup defaults
    Chart.defaults.font.family = $("body").css('font-family');
    var chartLabelsColor = 'black';

    //add colors to dataset
    var chartColors = getColors(SharedChartData.datasets[0].data.length);
    if (!SharedChartData.datasets[0].backgroundColor) {
        SharedChartData.datasets[0].backgroundColor = [];
    }
    SharedChartData.datasets[0].backgroundColor = chartColors;

    /*NOTE: we are building our legend separately because we happen to have some
     * customers who have many steps for one process (30 happened), resulting in many slices.
     * Chart.js draws the legend inside chart space so in the end the legend
     * takes all the space from the pie graph, or completely goes of bounds
    */
    chart = new Chart(
        chartArea.getContext("2d"),
        {
            plugins: [ChartDataLabels],
            type: "pie",
            data: SharedChartData,//from view            
            options: {
                responsive: true,
                maintainAspectRatio: true,
                aspectRatio: 1,//necessary because chart canvas need a square not a rectangle
                animation: {
                    duration: 2000,
                },
                plugins: {
                    legend: {
                        display: false, //see NOTE above
                    },
                    //this plugin allow us to display slice value as label and place it at slice border
                    datalabels: {
                        color: chartLabelsColor,
                        anchor: "end",
                        align: "end",
                        offset: 0,
                        font: {
                            weight: 'bold',
                            size: 14,
                        },
                    }
                },
                //padding is necessary to prevent slice label overflowing outside canvas area, disappearing
                layout: {
                    padding: {
                        top: 17,
                        bottom: 17,
                    }
                },
                onClick: function (e) {
                    var sliceClicked = chart.getElementsAtEventForMode(e, 'index', { intersect: true }, false);

                    if (!sliceClicked || !sliceClicked.length) {
                        return; // exit if clicked element was not a slice of chart
                    }

                    var url = getLinkByIndex(sliceClicked[0].index);
                    if (!isWhitespace(url)) {
                        jqNavigate(url);
                    } //navigate so stored search url
                }
            },
        });

    buildLegendFromChartDataset();
}

//legend building code included in plugin has scarce customization
//but more importantly seems to not work at all with screen scaling and mobile
//so we are forced to build manually our legend. 
//Maybe check again for next release of chart.js 
function buildLegendFromChartDataset() {
    var ds = chart.data.datasets[0];

    //define text html with placeholders

    var liBaseStart = '<li class="collection-item legend-item">';

    var legendColorPlh = '_LegendColorsPlh_';
    var iconColorHtmlText =
        '<i class="' + SharedConstStatisticsPage.get('IconClasses')
        + ' legend-sample" style = "color:' + legendColorPlh + '" >'
        + SharedConstStatisticsPage.get('LegendBoxIcon') + '</i >';

    var tmpLiText = "";
    var tmpIcon = "";
    for (var index = 0; index < ds.data.length; index++) {
        tmpLiText = ds.labels[index];
        tmpIcon = iconColorHtmlText.replace(legendColorPlh, ds.backgroundColor[index]);

        $("#chart-legend").append(liBaseStart + tmpIcon + tmpLiText + '</li>');
    }
}

function getLinkByIndex(elemIndex) {
    var urlToReturn = '';
    if (!isNull(elemIndex) && elemIndex >= 0
        && SharedSliceLabelsToStepSearchLinks) {

        var linkSearchs = SharedSliceLabelsToStepSearchLinks.get('items');
        if (linkSearchs && linkSearchs.length > 0) {

            for (var index = 0; index < linkSearchs.length; index++) {

                if (index === elemIndex) {
                    urlToReturn = linkSearchs[index].linkToStepItemsSearch
                    return urlToReturn;
                }
            }
        }
    }
}



//*
// * TEST DATA OBJECT : uncomment to see chart code behavior for a massive dataset
// * elements above maximum pre compiled palette, colors will be randomized
//SharedChartData = {
//    labels: ["elem1", "elem2", "elem3", "elem4", "elem5"
//        , "elem6", "elem7", "elem8", "elem9", "elem10"
//        , "elem11", "elem12", "elem13", "elem14", "elem15"
//        , "elem16", "elem17", "elem18", "elem19", "elem20"
//        , "elem21", "elem22", "elem23", "elem24", "elem25"
//        , "elem26", "elem27", "elem28", "elem29", "elem30"
//        , "elem31", "elem32", "elem33", "elem34", "elem35"
//        , "elem36", "elem37", "elem38", "elem39", "elem40"
//        , "elem41", "elem42", "elem43", "elem44", "elem45"
//        , "elem46", "elem47"
//    ],
//        datasets: [{
//            data: [10, 10, 10, 10, 10
//                , 10, 10, 10, 10, 10
//                , 11, 12, 13, 14, 15
//                , 16, 17, 18, 19, 20
//                , 10, 10, 10, 10, 10
//                , 10, 10, 10, 10, 10
//                , 10, 10, 10, 10, 10
//                , 10, 10, 10, 10, 10
//                , 10, 10, 10, 10, 10
//                , 10, 10
//            ],
//            labels: ["elem1", "elem2", "elem3", "elem4", "elem5"
//                , "elem6", "elem7", "elem8", "elem9", "elem10"
//                , "elem11", "elem12", "elem13", "elem14", "elem15"
//                , "elem16", "elem17", "elem18", "elem19", "elem20"
//                , "elem21", "elem22", "elem23", "elem24", "elem25"
//                , "elem26", "elem27", "elem28", "elem29", "elem30"
//                , "elem31", "elem32", "elem33", "elem34", "elem35"
//                , "elem36", "elem37", "elem38", "elem39", "elem40"
//                , "elem41", "elem42", "elem43", "elem44", "elem45"
//                , "elem46", "elem47"
//            ],
//        }]
//    };