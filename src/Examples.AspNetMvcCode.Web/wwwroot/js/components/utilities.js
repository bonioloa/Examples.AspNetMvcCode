/**
 * test if undefined or null
 * @param {any} elem
 */
function isNull(elem) {
    return elem === undefined || elem === null;
}

/**
 * test if IsNull or length = 0
 * @param {any} o
 */
function isObjEmpty(o) {
    return (isNull(o) || o.length === 0)
}

/**
 * test if object is not null/undefined and has a length > 0
 * @param {any} o
 */
function isObjNotEmpty(o) {
    return !isObjEmpty(o);
}

// whitespace
//BOI, followed by one or more whitespace characters, followed by EOI.
var reWhitespace = /^\s+$/;

/**
 * test if IsObjEmpty or whitespace regex
 * @param {string} s
 */
function isWhitespace(s) {   // Is s empty?
    return (isObjEmpty(s) || reWhitespace.test(s));
}

/**
 * check if input equals to string 'true'
 * @param {any} myValue
 */
function stringIsTrue(myValue) {
    return myValue === 'true'
}

/**
 * strips elements matching selector from possibly dangerous string
 * @param {string} selector
 * @param {string} text
 */
function safeHtmlDisplay(selector, text) {
    $(selector).html(text
        .replace("script", "")
        .replace("iframe", "")
        .replace("form", "")
        .replace("object", "")
        .replace("embed", "")
        .replace("link", "")
        .replace("head", "")
        .replace("meta", "")
        .replace("href", "")
    );
}

/*
 * Clears the selected form elements.
 * Necessary for input[type=file]
 */
$.fn.clearFields = $.fn.clearInputs = function (includeHidden) {
    var re = /^(?:color|date|datetime|email|month|number|password|range|search|tel|text|time|url|week)$/i; // 'hidden' is not in this list
    return this.each(function () {
        var t = this.type, tag = this.tagName.toLowerCase();
        if (re.test(t) || tag == 'textarea') {
            this.value = '';
        }
        else if (t == 'checkbox' || t == 'radio') {
            this.checked = false;
        }
        else if (tag == 'select') {
            this.selectedIndex = -1;
        }
        else if (t == "file") {
            if (/MSIE/.test(navigator.userAgent)) {
                $(this).replaceWith($(this).clone(true));
            } else {
                $(this).val('');
            }
        }
        else if (includeHidden) {
            // includeHidden can be the value true, or it can be a selector string
            // indicating a special test; for example:
            //  $('#myForm').clearForm('.special:hidden')
            // the above would clean hidden inputs that have the class of 'special'
            if ((includeHidden === true && /hidden/.test(t)) ||
                (typeof includeHidden == 'string' && $(this).is(includeHidden)))
                this.value = '';
        }
    });
};

/**
 * do cleanStringSafe value for elements matched by selector.
 * Prevents undefined or null errors
 * @param {string} selector
 */
function getValCleanBySelector(selector) {
    if (isNull($(selector)) || isObjEmpty($(selector))) {
        return '';
    }
    else {
        return cleanStringSafe($(selector).val());
    }
}
/**
 * Removes non breakable space entity from given string
 * @param {string} str
 */
function cleanStringSafe(str) {
    if (isNull(str) || isWhitespace(str)) {
        return '';
    }
    else {
        return str.trim().replace(/&nbsp;/gi, '');
    }
}
/**
 * Add text to element and replaces newlines in text with html newline tag
 * @param {any} text
 */
$.fn.textAsHtmlMultiline = function (text) {
    this.text(text);
    this.html(this.html().replace(/\n/g, '<br/>'));
    return this;
}