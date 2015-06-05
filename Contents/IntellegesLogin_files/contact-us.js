(function () {
    var _this = this;
    
    $("#contact_us_dialog .centred-button button").on("click", function () {
        var validator = $("#modalformControls").validator('validate');
        var email = $('#inputEmail').val();

        if ($("#modalformControls .has-error").length == 0) {
            $.ajax({
                url: _contactUsHost+"Home/ContactUs",                
                data: { Email: email, page: 2, IpAddress: _localAddress, Challenge: Recaptcha.get_challenge(), Response: Recaptcha.get_response(), enterpriseId: enterpriseId },
                type: "POST"
            }).done(function (data) {
                if (data !== "")
                    alertify.alert(data, function () {
                        createCaptcha();
                    });
                else {
                    alertify.alert("Thank you for submitting your contact information, you should receive an email shortly with a link for your comments. Please check your email for an email from contactUs@intelleges.com.", function () {
                        $("#contact_us_dialog").modal('toggle');
                    });
                                    
                    //captcha ok
                }
            }).fail(function (jqXHR, textStatus, errorThrown) {
                alertify.alert(textStatus);
                createCaptcha();
            });
        }
    });
    $("#contactusid").on("click", function () {
        $("#contact_us_dialog").modal('toggle');
        createCaptcha();
        $('#inputEmail').val("");
    });
    function createCaptcha() {
        Recaptcha.create(_publicKey, "contact_us_dialog_captcha", { theme: "clean" });
        $("#recaptcha_response_field").attr("placeholder", "Please Enter Your Security Code");
    }
    $("#forgotPasswordLinq").on("click", function () {

        $("#get_password_dialog").modal('toggle');
        $('#inputEmailGetPassword').val("");
    });
    $("#get_password_dialog #modalGetPassword button").on("click", function () {
        var validator = $("#modalGetPassword").validator('validate');
        var email = $('#inputEmailGetPassword').val();
        if ($("#modalformControls .has-error").length == 0) {
            //GetForgotPassword
            $.ajax({
                url: "https://www.intelleges.com/mvcmt/Generic/Admin/GetForgotPassword",
                data: { Email: email },
                type: "POST"
            }).done(function (data) {
                if (data !== "")
                    alertify.alert(data, function () {
                        //createCaptcha();
                    });
                else {
                    alertify.alert("We've sent an email to <b>" + email + "</b>. Please check your email now for a message from Intelleges.com with the subject line 'Intelleges Account Request'.<br /><br /><b>Click the link in the email</b> to set a new password for your intelleges.com account.<br /><br />To protect your privacy, we only send this information to the email address on file for this account.<br /><br /><b>PLEASE NOTE:</b>  If you use email filtering or anti-spam software, please make sure email from Intelleges.com is not filtered or blocked.", function () {
                        $("#get_password_dialog").modal('toggle');
                    });                    
                    //captcha ok
                }
            }).fail(function (jqXHR, textStatus, errorThrown) {
                alertify.alert(textStatus);
                //createCaptcha();
            });
        }
    });
    if (enterpriseId !== 1 && enterpriseId!==1122) {
        $("#contact_us_dialog").modal('toggle');
        createCaptcha();
    }
}
)();