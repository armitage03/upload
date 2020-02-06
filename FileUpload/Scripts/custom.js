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