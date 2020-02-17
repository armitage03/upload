function isValidFileUploadSize(input, errMessageLabel) {
    var totalFileSize = 0;
    var result;
    for (var i = 0; i < input.files.length; i++) {
        totalFileSize += input.files[i].size;
    }
                        
    if (totalFileSize > 1048576) {                      
        $(errMessageLabel).html("Allowed file size exceeded. (Max. 1 MB)<br />");
        $(errMessageLabel).removeClass("field-validation-valid");
        $(errMessageLabel).addClass("field-validation-error");
        result = false;
    }
    else {
        $(errMessageLabel).html("");
        $(errMessageLabel).removeClass("field-validation-error");
        $(errMessageLabel).addClass("field-validation-valid");
        result = true;
    }
    return result;
}
function clearAllValidationError() {
    $(".field-validation-error").addClass("field-validation-valid");
    $(".field-validation-error").removeClass("field-validation-error");
    $(".input-validation-error").removeClass("input-validation-error");
    $(".field-validation-valid").html("");
}
function removeValidationError(errorInput) {
    var inputId = $(errorInput).prop('name');
    var errMessageLabel;
    if ($(errorInput).is(':radio') || $(errorInput).is(':checkbox')) {
        errMessageLabel = $(errorInput).closest("div").parent().find("span[data-valmsg-for='" + inputId + "']");
    }
    else if ($(errorInput).hasClass("selectpicker")) {
        errMessageLabel = $(errorInput).closest("div").parent().find("span[data-valmsg-for='" + inputId + "']");
        $(errorInput).closest("div").parent().find("button").removeClass("input-validation-error");
    }
    else {
        errMessageLabel = $(errorInput).closest("div").find("span[data-valmsg-for='" + inputId + "']");
        $(errorInput).removeClass("input-validation-error");
    }
    $(errMessageLabel).html("");
    $(errMessageLabel).removeClass("field-validation-error");
    $(errMessageLabel).addClass("field-validation-valid");
}
function addValidationError(errorInput, errorMessage) {
    var inputId = $(errorInput).prop('name');
    var errMessageLabel;
    if ($(errorInput).is(':radio') || $(errorInput).is(':checkbox')) {
        errMessageLabel = $(errorInput).closest("div").parent().find("span[data-valmsg-for='" + inputId + "']");
        // $(errorInput).closest("div").parent().addClass("input-validation-error");
    }
    else if ($(errorInput).hasClass("selectpicker")) {
        errMessageLabel = $(errorInput).closest("div").parent().find("span[data-valmsg-for='" + inputId + "']");
        $(errorInput).closest("div").parent().find("button").addClass("input-validation-error");
    }
    else {
        $(errorInput).addClass("input-validation-error");
        errMessageLabel = $(errorInput).closest("div").find("span[data-valmsg-for='" + inputId + "']");        
    }
    $(errMessageLabel).html(errorMessage);
    $(errMessageLabel).removeClass("field-validation-valid");
    $(errMessageLabel).addClass("field-validation-error");
}
function isLeapYear(year) {
    return ((year % 4 === 0) && (year % 100 !== 0)) || (year % 400 === 0);
}
function formatDateInput(txtDate) {
    let inputVal = $(txtDate).val();
    var y, m, d;
    if (inputVal === '') {
        return;
    }
    else if ($.isNumeric(inputVal)) {
        if (inputVal.length === 4) {//1199 to 1/1/99
            y = inputVal.substr(2);
            m = inputVal.substr(1, 1);
            d = inputVal.substr(0, 1);
        }
        else if (inputVal.length === 5) {//21199 to 21/1/99
            y = inputVal.substr(3);
            m = inputVal.substr(2, 1);
            d = inputVal.substr(0, 2);
        }
        else if (inputVal.length >= 6) {//211299 to 21/12/99
            y = inputVal.substr(4);
            m = inputVal.substr(2, 2);
            d = inputVal.substr(0, 2);
        }
        else {
            addValidationError($(txtDate), "Please enter a valid date.");
            return;
        }
        if (y.length === 2) {
            var year = parseInt(y, 10);
            let currentYear = new Date().getFullYear();
            //If(CCCC - 19YY) <= 90, convert YY to 19YY
            if (currentYear - (year + 1900) <= 90) {
                y = year + 1900;
            }
            else {
                //If(CCCC - 19YY) > 90, convert YY to 20YY
                y = year + 2000;
            }
        }
        $(txtDate).val(d + "/" + m + "/" + y);
    }
}
function isValidDateFormat(txtDate) {
    let inputVal = $(txtDate).val();
    var y, m, d;
    if (inputVal === '') {
        return true;
    }
    else if (inputVal.includes("/")) {
        var mydate = inputVal.split("/");
        if (mydate.length !== 3) {//check if format is d/M/yyyy
            addValidationError($(txtDate), "Please enter a valid date.");
            return false;
        }
        else {
            d = parseInt(mydate[0], 10);
            m = parseInt(mydate[1], 10);
            y = parseInt(mydate[2], 10);
        }
    }
    else {
        addValidationError($(txtDate), "Please enter a valid date.");
        return false;
    }

    if (m <= 0 || m > 12 || d > 31) {
        addValidationError($(txtDate), "Please enter a valid date.");
        return false;
    }
    if (d > 30 && (m === 9 || m === 4 || m === 6 || m === 11)) {
        addValidationError($(txtDate), "Please enter a valid date.");
        return false;
    }
    if (m === 2) {
        if (isLeapYear(y) && d > 29) {
            addValidationError($(txtDate), "Please enter a valid date.");
            return false;
        }
        else if (!isLeapYear(y) && d > 28) {
            addValidationError($(txtDate), "Please enter a valid date.");
            return false;
        }
    }
    var isoDate = y + "-" + (m.toString().length > 1 ? m : "0" + m) + "-" + (d.toString().length > 1 ? d : "0" + d);//YYYY-MM-DD
    if (isNaN(Date.parse(isoDate))) {
        addValidationError($(txtDate), "Please enter a valid date.");
        return false;
    }
    else {
        removeValidationError($(txtDate));
        return true;
    }
}
function displayBlockUI() {
    $("#div_BlockUI").html($("#div_BlockUI").html());
    $.blockUI({ baseZ: 1030, message: $("#div_BlockUI") });
    return true;
}
function hideBlockUI() {
    $.unblockUI();
    return true;
}
$(document).ready(function () {
    $(".datetimepicker-container").each(function () {
        if ($(this).find(".input-group-addon").length === 0)
            $("<span class='input-group-addon'><span class='glyphicon glyphicon-calendar'></span></span>").appendTo(this);

        var icon = $(this).find(".input-group-addon").first();
        var dateinput = $(this).find(".datepicker-input").first();
       

        var tmpStr;
        icon.click(function () {
            if (dateinput.length > 0) {
                dateinput.focus();
                tmpStr = dateinput.val();
                dateinput.val('');
                dateinput.val(tmpStr);
            }           
        });
    });

    // Date picker
    $(".datepicker-input").datepicker({ dateFormat: "d/m/yy", changeYear: true, changeMonth: true, yearRange: "1950:2050", onSelect: function (dateText) { $(this).change(); } });
   
    $(".datepicker-input").attr("placeholder", "D/M/YYYY");
    $(".datepicker-input").attr("maxlength", "10");
    $(".datepicker-input").change(function () {
        formatDateInput(this);
        isValidDateFormat(this);
    });
});