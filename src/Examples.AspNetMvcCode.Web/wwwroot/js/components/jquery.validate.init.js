/*
 * add here all functions necessary for jquery.validate configuration
 * 
 **/
//dependencies
//jquery
//jquery-validate
//materialize
//SharedConstSite - view _layout


function setupJqueryValidate() {
    var errorClass = 'invalid form-error';
    var validClass = 'valid';

    jQuery.validator.setDefaults({
        //debug: true, //NEVER enable it else every html post submission will not work
        success:
            function (label, element) {
                $(element).addClass(validClass);
                label.addClass(validClass);

                if ($(element).is('select')) { //manually set styles for materialize select, jquery validate doesn't handle it
                    $(element).attr('aria-invalid', 'false');
                    $(element).removeClass(errorClass);

                    $(element).siblings('input').removeClass(errorClass)
                    $(element).siblings('input').addClass(validClass);
                    $(element).siblings('input').attr('aria-describedby', $(element).attr('aria-describedby'));
                    $(element).siblings('input').attr('aria-invalid', $(element).attr('aria-invalid'));
                }
            },
        errorClass: errorClass,
        errorPlacement: function (error, element) {
            var errorText = error.text();
            error = error.attr("class", "helper-text").attr("data-error", errorText).text('');

            if (element.is('input:not([type]) '
                + ', input[type=text], input[type=password]'
                + ', input[type=email], input[type=url]'
                + ', input[type=time], input[type=date]'
                + ', input[type=datetime], input[type=datetime-local]'
                + ', input[type=tel], input[type=number]'
                + ', input[type=search], textarea.materialize-textarea'
                + ', select')) {

                if ($("span#" + element.attr("id") + "-error").length === 0) {
                    error.insertAfter(element);
                }
                else {
                    $("span#" + element.attr("id") + "-error").attr("data-error", errorText);
                }

                //materialize select wrapper fix
                if ($(element).is('select.invalid.form-error')) {
                    $(element).siblings('input').addClass("invalid form-error")
                }
            }
            if (element.is('input[type = radio],input[type=checkbox]')) {
                if ($("span#" + element.attr("name") + "-error").length === 0) {
                    error.insertAfter(element);
                }
                else {
                    $("span#" + element.attr("name") + "-error").attr("data-error", errorText);
                }
            }
        },
        errorElement: "span",
        ignore: ".ignore-validation"
    });

    setValidationRules();
}



function setValidationRules() {

    jQuery.validator.addMethod("user", function (value, element) {
        var pattern = new RegExp(SharedConstSite.get('RegexUser'));
        return this.optional(element) || pattern.test(value);
    }, SharedConstSite.get('SharedJqValErrorUserMessage'));

    jQuery.validator.addMethod("email", function (value, element) {
        var pattern = new RegExp(SharedConstSite.get('RegexEmail'));
        return this.optional(element) || pattern.test(value);
    }, SharedConstSite.get('SharedJqValErrorEmailMessage'));

    jQuery.validator.addMethod("emailSearch", function (value, element) {
        var pattern = new RegExp(SharedConstSite.get('RegexEmailSearch'));
        return this.optional(element) || pattern.test(value);
    }, SharedConstSite.get('SharedJqValErrorEmailSearchMessage'));

    jQuery.validator.addMethod("passwordWb", function (value, element) {
        var pattern = new RegExp(SharedConstSite.get('RegexPassword'));
        return this.optional(element) || pattern.test(value);
    }, SharedConstSite.get('SharedJqValErrorPasswordMessage'));

    jQuery.validator.addMethod("passwordConfirmWb", function (value, element) {
        var pattern = new RegExp(SharedConstSite.get('RegexPassword'));
        return this.optional(element) || pattern.test(value);
    }, SharedConstSite.get('SharedJqValErrorConfirmPasswordMessage'));

    jQuery.validator.addMethod("nameSurname", function (value, element) {
        return this.optional(element)
            || (value.length >= SharedConstSite.get('PersonalNameSurnameMinimumCharacters')
                && value.length <= SharedConstSite.get('PersonalNameSurnameMaximumCharacters'));
    }, SharedConstSite.get('SharedJqValErrorNameSurnameMessage'));

    jQuery.validator.addMethod("nameSurnameSearch", function (value, element) {
        return this.optional(element)
            || (value.length >= SharedConstSite.get('PersonalNameSurnameMinimumCharacters')
                && value.length <= SharedConstSite.get('PersonalNameSurnameMaximumCharactersSearch'));
    }, SharedConstSite.get('SharedJqValErrorNameSurnameSearchMessage'));

    jQuery.validator.addMethod("loginCodeWb", function (value, element) {
        var pattern = new RegExp(SharedConstSite.get('RegexLoginCode'));
        return this.optional(element) || pattern.test(value);
    }, SharedConstSite.get('SharedJqValErrorLoginCode'));

    jQuery.validator.addMethod("numericLocalized", function (value, element) {
        var pattern = new RegExp(SharedConstSite.get('RegexNumericLocalized'));
        return this.optional(element) || pattern.test(value);
    }, SharedConstSite.get('SharedJqValErrorNumericLocalized'));

    jQuery.validator.addMethod("isoDate", function (value, element) {
        var pattern = new RegExp(SharedConstSite.get('RegexIsoDate'));
        return this.optional(element) || pattern.test(value);
    }, SharedConstSite.get('SharedJqValErrorDateLocalized'));

    initializeValidate();

    //setup field error messages, can be overridden adding above rules
    var normalInputs = $(
        'form input[type="text"].validate '
        + ', form input[type="email"].validate'
        + ', form input[type="password"].validate'
        + ', form textarea.materialize-textarea.validate');
    if (normalInputs.length > 0) {
        normalInputs.each(function () {
            $(this).rules('add', {
                messages: {
                    required: SharedConstSite.get('SharedJqValErrorMandatoryFieldGenericHtmlMessage')
                }
            })
        });
    }

    var numericInputs = $('.input-numeric.validate');
    if (numericInputs.length > 0) {
        numericInputs.each(function () {
            $(this).rules('add', { 'numericLocalized': true })
        });
    }


    var dateInputs = $('.datepicker.validate');
    if (dateInputs.length > 0) {
        dateInputs.each(function () {
            $(this).rules('add', { 'isoDate': true })
        });
    }

    var fileInputs = $('form input[type="text"].validate.file-path');
    if (fileInputs.length > 0) {
        fileInputs.each(function () {
            $(this).rules('add', {
                messages: {
                    required: SharedConstSite.get('SharedJqValErrorMandatoryFileField')
                }
            })
        });
    }

    var optionsInputs = $(
        'form input[type="radio"].validate'
        + ', form input[type="checkbox"].validate'
        + ', form select.validate');
    if (optionsInputs.length > 0) {
        optionsInputs.each(function () {
            $(this).rules('add', {
                messages: {
                    required: SharedConstSite.get('SharedJqValErrorMandatoryOptionField')
                }
            })
        });
    }
    var selects = $('select.validate[required]');
    if (selects.length > 0) {
        selects.each(function () {
            $(this).on('change', function () {
                $(this).valid();
            })
        });
    }
}

function initializeValidate() {
    //initialize plugin for all form except ajax posts
    $('form').each(function () {
        $(this).validate({
            submitHandler: function (form) {
                //always show preloader unless submit button exists and opts out with this class
                if (isNull($(this)[0].submitButton)
                    || isNull($(this)[0].submitButton.className)
                    || $(this)[0].submitButton.className.indexOf('no-preloader') <= 0) {
                    showPreloader();
                }

                form.submit();
            }
        });
    });
}
