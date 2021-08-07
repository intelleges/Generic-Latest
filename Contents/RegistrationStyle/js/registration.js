//function showElement(elem) {
//    elem.show();
//    alert(elem.css("display"));
//    //if (jQuery.browser.msie) {
//    elem.css({ "visibility": "visible" });
//    // }
//}
//function hideElement(elem) {
//    elem.hide();
//    //if (jQuery.browser.msie) {
//    elem.css({ "visibility": "hidden" });
//    //}
//}
function validateEsignature() {
    debugger;
    var isValid = true;
    if (jQuery("#firstName").val() == "") {
        isValid = false;
        jQuery("#errorFrstName").show();
    } else {
        jQuery("#errorFrstName").hide();
    }
    if (jQuery("#lastName").val() == "") {
        isValid = false;
        jQuery("#errorlastName").show();
    } else {
        jQuery("#errorlastName").hide();
    }
    if (jQuery("#email").val() == "") {
        isValid = false;
        jQuery("#errorEmail").show();
    } else {
        jQuery("#errorEmail").hide();
    }
    return isValid;
}


function progressBar(percent, $element) {

    var progressBarWidth = percent * $element.width() / 100;
    $element.find('div').animate({ width: progressBarWidth }, 500).html(percent + "%&nbsp;");
}



function showdiv(sender) {
    if (jQuery("#AllDiv_" + result).length > 0) {
        jQuery("#AllDiv_" + result).show();
    }
    alert(sender.name);

    var s = sender.id;

    var parts = s.split("_");

    var result = parts[parts.length - 2];
    alert(result);
    var val = sender.value;
    alert(val);
    var radioButtons = document.getElementsByName(); //'ctl00$contentQuestionnaireBodyjQueryquestion_1349_379');
    alert(radioButtons.length);
    for (var x = 0; x < radioButtons.length; x++) {
        if (radioButtons[x].checked) {
            alert("You checked " + radioButtons[x].value);
        }
    }
    if (val == 74) {
        jQuery("#Div_1350").show();
    }
    else {
        jQuery("#Div_1350").hide();
    }
}
function removevalidation(sender) {
    var cont = jQuery('#' + sender).innerHTML;
    jQuery(function () {
        jQuery(cont).find("input").each(function () {
            var id = jQuery(this).attr("id");
            if (jQuery("#" + id).prop("checked")) {
                var splitter = jQuery(this).attr("id").split("_");
                //alert(splitter);
                var logic = splitter[(splitter.length) - 1];

                var question_id = splitter[(splitter.length) - 3];
                // alert(question_id);
                if (logic == 1) {
                    //alert(""+question_id+"_R");

                    //  ValidatorEnable(document.getElementById("" + question_id + "_R"), false);
                }
                else {
                    //  ValidatorEnable(document.getElementById("" + question_id + "_R"), true);
                }
            }
        });


    });
    //var logic
}
function showIfNeeded(divname, id, code) {
    if (jQuery("#" + id + " option:selected").data("code") === code) {
        jQuery(divname).show();
    } else {
        jQuery(divname).hide();
    }
}
function showdropdowndiv(sender, showIfNeededFuntion) {
    if (!showIfNeededFuntion) {
        showIfNeededFuntion = showIfNeeded;
    }
    var expr = new RegExp("\\([A-Z][A-Z]\\)");
    var s = jQuery(sender).prop("id");
    var parts = s.split("_");
    var result = parts[parts.length - 2];
    var divname = "#nDiv_" + result;

    var arr = expr.exec(jQuery(divname).data("code"));
    if (arr && arr.length > 0) {
        showIfNeededFuntion(divname, s, arr[0]);
    } else {
        if (jQuery(divname).data("code")) {
            showIfNeededFuntion(divname, s, "(" + jQuery(divname).data("code") + ")");
        }
    }

    divname = "#yDiv_" + result;
    arr = expr.exec(jQuery(divname).data("code"));
    if (arr && arr.length > 0) {
        showIfNeededFuntion(divname, s, arr[0]);
    } else {
        if (jQuery(divname).data("code")) {
            showIfNeededFuntion(divname, s, "(" + jQuery(divname).data("code") + ")");
        }
    }


    if (jQuery("#AllDiv_" + result).length > 0) {
        jQuery("#AllDiv_" + result).show();
    }
}
function showdivnew(sender) {
    //alert("Hello");
    var allInputs = document.querySelectorAll("input,textarea");
    var last = "NameUnlikelyToBeUsedAsAnElementName";

    var s = sender.id;

    var parts = s.split("_");

    var result = parts[parts.length - 2];

    var divname = "Div_" + result;
    if (jQuery("#All" + divname).length > 0) {
        jQuery("#All" + divname).show();
    }
    for (i = 0; i < allInputs.length; i++) {
        //            animatedcollapse.addDiv('y' + divname, 'fade=1,height=50px');
        //            animatedcollapse.addDiv('n' + divname, 'fade=1,height=20px');
        //            animatedcollapse.init();
        var input = allInputs[i];
        if (input.name == last) continue; // if this object name is the same as the last checked radio, go to next iteration


        // checks to see if any  one of  similarly named radiobuttons is checked 
        else if (input.type == "radio") {
            last = input.name;
            // alert(last);
            var radios = document.getElementsByName(input.name);

            var radioSelected = false;
            // alert(input.id);
            // alert(sender.id);
            // var s1 = sender.id;

            var parts1 = s.split("_");

            var result1 = parts1[parts1.length - 2];

            var s2 = input.name;

            var parts2 = s2.split("_");

            var result2 = parts2[parts2.length - 2];
            // alert('result1 ' + result1 + ' result2 ' + result2);
            // alert('s2 ' + s2 + ' s ' + s);

            if (result1 == result2) {
                //iterate over question options
                for (j = 0; j < radios.length; j++) {
                    if (radios[j].checked) {
                        animatedcollapse.addDiv('y' + divname, 'fade=1,height=50px');
                        animatedcollapse.addDiv('n' + divname, 'fade=1,height=50px');
                        animatedcollapse.init();

                        jQuery('#y' + divname).parent().find('.field-validation-error').html('');
                        jQuery('#n' + divname).parent().find('.field-validation-error').html('');

                        if (radios[j].value == 74) {
                            //document.getElementById('y' + divname).style.display = 'block';
                            //document.getElementById('n' + divname).style.display = 'none';
                            animatedcollapse.show('y' + divname);
                            animatedcollapse.hide('n' + divname);
                        }
                        else if (radios[j].value == 75) {
                            //                            document.getElementById('y' + divname).style.display = 'none';
                            //                            document.getElementById('n' + divname).style.display = 'block';
                            // alert('n' + divname);
                            //                                document.getElementById('n' + divname).style.display = 'block';
                            //                                document.getElementById('y' + divname).style.display = 'none';
                            animatedcollapse.show('n' + divname);
                            animatedcollapse.hide('y' + divname);
                        }
                        else if (radios[j].value == 76) {
                            //                                document.getElementById('n' + divname).style.display = 'none';
                            //                                document.getElementById('y' + divname).style.display = 'none';
                            animatedcollapse.hide('y' + divname);
                            animatedcollapse.hide('n' + divname);
                            //                                document.getElementById('y' + divname).style.display = 'none';
                            //                                document.getElementById('n' + divname).style.display = 'none';
                        }

                        //                       radioSelected=true;
                        //                       break;
                    }
                }
                //            if (!radioSelected) // no option selected
                //            {       // warn user, focus question
                //                    alert("You did not answer question " + input.id.substring(0,input.id.length-1));
                //                    input.focus();
                //                    return false;
                //            }                                   
            }
        }

    }
}

function showdivRadioList(sender) {

    var allInputs = document.querySelectorAll("input,textarea");
    var last = "NameUnlikelyToBeUsedAsAnElementName";

    var s = sender.id;

    var parts = s.split("_");

    var result = parts[parts.length - 2];

    var divname = "Div_" + result;

    if (jQuery("#All" + divname).length > 0) {
        jQuery("#All" + divname).show();
    }

    for (i = 0; i < allInputs.length; i++) {
        //            animatedcollapse.addDiv('y' + divname, 'fade=1,height=50px');
        //            animatedcollapse.addDiv('n' + divname, 'fade=1,height=20px');
        //            animatedcollapse.init();
        var input = allInputs[i];
        if (input.name == last) continue; // if this object name is the same as the last checked radio, go to next iteration


        // checks to see if any  one of  similarly named radiobuttons is checked 
        else if (input.type == "radio") {

            last = input.name;
            // alert(last);
            var radios = document.getElementsByName(input.name);

            var radioSelected = false;


            var parts1 = s.split("_");

            var result1 = parts1[parts1.length - 2];

            var s2 = input.name;

            var parts2 = s2.split("_");

            var result2 = parts2[parts2.length - 2];
            var lans = parts2[(parts2.length) - 1];
            if (result1 === result2) {
                //iterate over question options
                for (j = 0; j < radios.length; j++) {
                    if (radios[j].checked && !jQuery(radios[j]).data("code")) {
                        animatedcollapse.addDiv('y' + divname, 'fade=1,height=50px');
                        animatedcollapse.addDiv('n' + divname, 'fade=1,height=50px');
                        animatedcollapse.addDiv('commented' + divname, 'fade=1,height=50px');
                        animatedcollapse.init();

                        if (j === 0) {
                            //alert('Hi');
                            animatedcollapse.show('y' + divname);
                            animatedcollapse.hide('n' + divname);
                            animatedcollapse.hide('commented' + divname);
                        }
                        else if (j == 1) {
                            //alert('He');
                            animatedcollapse.show('n' + divname);
                            animatedcollapse.hide('y' + divname);
                            animatedcollapse.hide('commented' + divname);
                        }
                        else {
                            // alert(sender.id);
                            //alert(lans);
                            animatedcollapse.hide('y' + divname);
                            animatedcollapse.hide('n' + divname);
                            if (jQuery(radios[j]).data("commented") === "True") {
                                //$('#commented' + divname + ' span').text($("label[for='" + $(radios[j]).prop("id") + "']").text());
                                animatedcollapse.show('commented' + divname);
                                //animatedcollapse.hide('commented' + divname);
                            } else {
                                animatedcollapse.hide('commented' + divname);
                            }
                        }

                        //                       radioSelected=true;
                        //                       break;
                    }
                }
                //            if (!radioSelected) // no option selected
                //            {       // warn user, focus question
                //                    alert("You did not answer question " + input.id.substring(0,input.id.length-1));
                //                    input.focus();
                //                    return false;
                //            }                                   
            }
        }

    }
}
function showDivByCode(sender) {
    var s = jQuery(sender).prop("id");
    var parts = s.split("_");
    var result = parts[parts.length - 2];
    var divname = "#nDiv_" + result;
    if (jQuery(divname).data("code") === jQuery('#' + sender.id + ' input:checked').data("code")) {
        jQuery(divname).show();
    } else {
        jQuery(divname).hide();
    }

    divname = "#yDiv_" + result;
    if (jQuery(divname).data("code") === jQuery('#' + sender.id + ' input:checked').data("code")) {
        jQuery(divname).show();
    } else {
        jQuery(divname).hide();
    }

    if (jQuery("#AllDiv_" + result).length > 0) {
        jQuery("#AllDiv_" + result).show();
    }
}
var radioListShowIfNeeded = function (divname, id, code) {
    if (jQuery("#" + id + " input:checked").data("code") === code) {
        jQuery(divname).show();
        //jQuery(divname).show();
    } else {
        //hideElement(jQuery(divname));
        jQuery(divname).hide();
    }
};

var registration = (function ($) {
    var _this = {
        vars: {
            currentNarrativeSubjectsList: []
        }
    };

    jQuery(document).ready(function ($) {
        $(".narrative-hint").click(function () {
            //alert();
            CKEDITOR.instances["text-field"].setData("");
            _this.vars.currentQuestionId = $(this).prop("id").split("-")[2];
            $.getJSON("GetSubjectsByQuestion?question=" + _this.vars.currentQuestionId, function (data) {

                var options = "<option>Select subject...</option>";
                if (data) {
                    _this.vars.currentNarrativeSubjectsList = data;
                    for (var i = 0; i < data.length; i++) {
                        options += "<option value='" + data[i].id + "'>" + data[i].subject + "</option>";
                    }
                }
                $("#subjects-field").html(options);
            });
            $("#myModal").modal("toggle");
        });

        $("#subjects-field").change(function () {
            var value = $("#subjects-field").val();
            for (var i = 0; i < _this.vars.currentNarrativeSubjectsList.length; i++) {
                if (value == _this.vars.currentNarrativeSubjectsList[i].id) {
                    CKEDITOR.instances["text-field"].setData(_this.vars.currentNarrativeSubjectsList[i].narrative);
                    break;
                }
            }

        });
        $("#myModal #saveButton").click(function () {
            $("textarea[id^=question_" + _this.vars.currentQuestionId + "]").val(CKEDITOR.instances["text-field"].getData());
            $("#myModal").modal("toggle");
            //$("#question_" + _this.vars.currentQuestionId + "_" + "_text")
            //$.post("SaveDataFromHint", {
            //	questionId: _this.vars.currentQuestionId,
            //	text: CKEDITOR.instances["text-field"].getData()
            //}, function (data) {
            //	if (!data) {
            //		$("#myModal").modal("toggle");
            //	} else {
            //		alertify.alert(data);
            //	}
            //});

        });
    });
    return _this;
})(jQuery);