(function () {
    var _this = this;
    var downloadDelegate = null;
    $("#contact_us_dialog .centred-button button").on("click", function () {
        var validator = $("#modalformControls").validator('validate');
        var email = $('#inputEmail').val();

        if ($("#modalformControls .has-error").length == 0) {
            $.ajax({
                url: "http://localhost:57882/Home/ContactUs",
                dataType:"json",
                data:{ Email: email, IpAddress: _localAddress, Challenge: Recaptcha.get_challenge(), Response: Recaptcha.get_response() },
                type: "POST"
            }).done(function (data) {
                if (data !== "")
                    alertify.alert(data, function () {
                        createCaptcha();
                    });
                else {
                    $("#contact_us_dialog").modal('toggle');
                    if (_this.downloadDelegate != null) {
                        _this.downloadDelegate();
                        _this.downloadDelegate = null;
                    }
                    //captcha ok
                }
            }).fail(function (jqXHR, textStatus, errorThrown) {
                alert("error");
                createCaptcha();
            });
        }
    });
    $("#contactusid").on("click", function () {
        createCaptcha();
        $('#inputEmail').val("");
    });
    function createCaptcha() {
        Recaptcha.create(_publicKey, "contact_us_dialog_captcha", { theme: "clean" });
        $("#recaptcha_response_field").attr("placeholder", "Please Enter Your Security Code");
    }

    //$(".case-studies-panel p").tooltip();
    //$(".case-studies-panel p").on("click", function () {
    //    var sender = this;
    //    $.ajax("https://www.intelleges.com/Home/NeedUseContactUs", { type: "POST", cache: false }).done(function (result) {
    //        if (result === "False") {
    //            window.location.href = "https://www.intelleges.com/Home/CaseStudy/" + $(sender).text();
    //        } else {
    //            _this.downloadDelegate = function () {
    //                window.location.href = "https://www.intelleges.com/Home/CaseStudy/" + $(sender).text();
    //            }
    //            $("#contact-us-menu").click();
    //        }

    //    });
    //});
    //$(".footer .right a").on("click", function () {
    //    $("#contact-us-menu").click();
    //});
}
)();