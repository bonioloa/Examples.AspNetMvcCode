//no dependencies

function getColors(setDimension) {

    var testSet = testIfCanUsePrecompiledPalettes(setDimension);
    if (testSet.length > 0) {
        return testSet.slice(0, setDimension);
    }
    return generateColorSet(setDimension);
}

//manual palette inspired to old site color. Added blue and lilly to get more variety
var palette6 = ["#FF0000", "#0000FF", "#AAAAAA", "#FFAA00", "#6600CC", "#00FF00"]

//palettes generated with a better algorithm online http://phrogz.net/css/distinct-colors.html
var palette17 =
    ["#730000", "#00e67a", "#3d00e6", "#bf4d00", "#00403c"
        , "#ff00cc", "#664400", "#00eeff", "#400022", "#f2c200"
        , "#003059", "#8c0038", "#b8e600", "#0081f2", "#f20020"
        , "#245900", "#000059"];
var palette25 =
    ["#33000e", "#e600d6", "#002999", "#00e2f2", "#1f7300"
        , "#ffee00", "#e53d00", "#7f0022", "#3d004d", "#004b8c"
        , "#003329", "#74d900", "#664400", "#e5005c", "#6100f2"
        , "#0088ff", "#00f2c2", "#293300", "#e57a00", "#8c0083"
        , "#000e33", "#00708c", "#008055", "#8c8300", "#592400"];
var palette47 =
    ["#b20018", "#002b40", "#eeff00", "#7f0022", "#004466"
        , "#333000", "#f20061", "#00a2f2", "#665f00", "#4d003d"
        , "#007a99", "#a68500", "#8c0070", "#00ccff", "#ffcc00"
        , "#ff00cc", "#005359", "#664400", "#7700b3", "#00eeff"
        , "#ffaa00", "#aa00ff", "#00735c", "#402200", "#000040"
        , "#00ffcc", "#b25f00", "#000073", "#00331b", "#8c3800"
        , "#0000e6", "#008c4b", "#ff6600", "#0000f2", "#00f261"
        , "#591800", "#002999", "#0a4d00", "#bf3300", "#002e73"
        , "#4b8c00", "#400000", "#0066ff", "#88ff00", "#ff0000"
        , "#007ae6", "#9ba600"]
function testIfCanUsePrecompiledPalettes(setDimension) {
    if (setDimension <= palette6.length) {
        return palette6;
    }
    if (setDimension <= palette17.length) {
        return palette17;
    }
    if (setDimension <= palette25.length) {
        return palette25;
    }
    if (setDimension <= palette47.length) {
        return palette47;
    }
    return [];
}

/**
 * generate an array of random colors sequentially distinct.
 * There is a more specific library (distinct-color) but it's built for npm,
 * a nightmare to set up in this application because of dependencies. Maybe in future
 * Other alternative is https://nagix.github.io/chartjs-plugin-colorschemes/
 * but the color schemes are too small
 * @param {any} setDimension output array dimension
 */
function generateColorSet(setDimension) {
    var maxEuclideanColorDistance = 441.67; //https://stackoverflow.com/a/6823717
    var distanceForSet = maxEuclideanColorDistance / setDimension;
    var a = [];
    var tmpRgbCol;
    for (; setDimension > 0; setDimension--) {
        tmpRgbCol = hex2rgb(freshColor(a, distanceForSet));
        a.push(rgbToHexString(tmpRgbCol[0], tmpRgbCol[1], tmpRgbCol[2]));
    }
    return a;
}


function freshColor(sofar, d) {
    var n, ok;
    while (true) {
        ok = true;
        n = Math.random() * 0xFFFFFF << 0;
        for (var c in sofar) {
            if (distance(hex2rgb(sofar[c]), hex2rgb(n)) < d) {
                ok = false;
                break;
            }
        }
        if (ok) { return n; }
    }
}

function distance(a, b) {
    var d = [a[0] - b[0], a[1] - b[1], a[2] - b[2]];
    return Math.sqrt((d[0] * d[0]) + (d[1] * d[1]) + (d[2] * d[2]));
}

//utility functions, maybe to move in site.js if needed on other library
function hex2rgb(h) {
    return [(h & (255 << 16)) >> 16, (h & (255 << 8)) >> 8, h & 255];
}
function rgbToHexString(r, g, b) {
    return "#" + componentToHex(r) + componentToHex(g) + componentToHex(b);
}
function componentToHex(c) {
    var hex = c.toString(16);
    return hex.length == 1 ? "0" + hex : hex;
}