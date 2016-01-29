
function validateEsignature() {
    debugger;
    var isValid = true;
    if ($("#firstName").val() == "") {
        isValid = false;
        $("#errorFrstName").show();
    } else {
        $("#errorFrstName").hide();
    }
    if ($("#lastName").val() == "") {
        isValid = false;
        $("#errorlastName").show();
    } else {
        $("#errorlastName").hide();
    }
    if ($("#email").val() == "") {
        isValid = false;
        $("#errorEmail").show();
    } else {
        $("#errorEmail").hide();
    }
    return isValid;
}


function progressBar(percent, $element) {

    var progressBarWidth = percent * $element.width() / 100;
    $element.find('div').animate({ width: progressBarWidth }, 500).html(percent + "%&nbsp;");
}



function showdiv(sender) {
    alert(sender.name);

    var s = sender.id;

    var parts = s.split("_");

    var result = parts[parts.length - 2];
    alert(result);
    var val = sender.value;
    alert(val);
    var radioButtons = document.getElementsByName(); //'ctl00$contentQuestionnaireBody$question_1349_379');
    alert(radioButtons.length);
    for (var x = 0; x < radioButtons.length; x++) {
        if (radioButtons[x].checked) {
            alert("You checked " + radioButtons[x].value);
        }
    }
    if (val == 74) {
        $("#Div_1350").show();
    }
    else {
        $("#Div_1350").hide();
    }
}
function removevalidation(sender) {
    var cont = $('#' + sender).innerHTML;
    $(function () {
        $(cont).find("input").each(function () {
            var id = $(this).attr("id");
            if ($("#" + id).prop("checked")) {
                var splitter = $(this).attr("id").split("_");
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
    if ($("#" + id + " option:selected").data("code") === code) {
        $(divname).show();
    } else {
        $(divname).hide();
    }
}
function showdropdowndiv(sender) {
    var expr = new RegExp("\\([A-Z][A-Z]\\)");
    var s = $(sender).prop("id");
    var parts = s.split("_");
    var result = parts[parts.length - 2];
    var divname = "#nDiv_" + result;

    var arr = expr.exec($(divname).data("code"));
    if (arr && arr.length > 0) {
        showIfNeeded(divname, s, arr[0]);
    } else {
        if ($(divname).data("code")) {
            showIfNeeded(divname, s, "(" + $(divname).data("code") + ")");
        }
    }
}
function showdivnew(sender) {
    //alert("Hello");
    var allInputs = document.getElementsByTagName("input");
    var last = "NameUnlikelyToBeUsedAsAnElementName";

    var s = sender.id;

    var parts = s.split("_");

    var result = parts[parts.length - 2];

    var divname = "Div_" + result;

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
    //alert("Hello");
    var allInputs = document.getElementsByTagName("input");
    var last = "NameUnlikelyToBeUsedAsAnElementName";

    var s = sender.id;

    var parts = s.split("_");

    var result = parts[parts.length - 2];

    var divname = "Div_" + result;

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
            if (result1 == result2) {
                //iterate over question options
                for (j = 0; j < radios.length; j++) {
                    if (radios[j].checked) {
                        animatedcollapse.addDiv('y' + divname, 'fade=1,height=50px');
                        animatedcollapse.addDiv('n' + divname, 'fade=1,height=50px');
                        animatedcollapse.addDiv('commented' + divname, 'fade=1,height=50px');
                        animatedcollapse.init();

                        if (j == 0) {
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
                            if ($(radios[j]).data("commented") === "True") {
                                $('#commented' + divname + ' span').text($("label[for='" + $(radios[j]).prop("id") + "']").text());
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
    var s = $(sender).prop("id");
    var parts = s.split("_");
    var result = parts[parts.length - 2];
    var divname = "#nDiv_" + result;
    if (('(' + $(divname).data("code") + ')') === $('#' + sender.id + ' input:checked').data("code")) {
        $(divname).show();
    } else {
        $(divname).hide();
    }
}