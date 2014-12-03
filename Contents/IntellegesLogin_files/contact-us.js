(function () {
    var _this = this;
    
    $("#contact_us_dialog .centred-button button").on("click", function () {
        var validator = $("#modalformControls").validator('validate');
        var email = $('#inputEmail').val();

        if ($("#modalformControls .has-error").length == 0) {
            $.ajax({
                url: _contactUsHost+"Home/ContactUs",                
                data:{ Email: email, IpAddress: _localAddress, Challenge: Recaptcha.get_challenge(), Response: Recaptcha.get_response() },
                type: "POST"
            }).done(function (data) {
                if (data !== "")
                    alertify.alert(data, function () {
                        createCaptcha();
                    });
                else {
                    $("#contact_us_dialog").modal('toggle');                    
                    //captcha ok
                }
            }).fail(function (jqXHR, textStatus, errorThrown) {
                alert(textStatus);
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
}
)();