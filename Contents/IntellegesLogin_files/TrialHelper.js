
$(document).ready(function () {

    //Change these values to style your modal popup
    var source = "Popup.aspx";
    var width = 300;
    var align = "center";
    var top = 285;
    var padding = 10;
    var backgroundColor = "#FFFFFF";
    var borderColor = "#F69157";
    var borderWeight = 4;
    var borderRadius = 5;
    var fadeOutTime = 300;
    var disableColor = "#666666";
    var disableOpacity = 40;
    var loadingImage = "images/lightbox-ico-loading.gif";

    //This method initialises the modal popup
    $(".modal").click(function () {
        modalPopup(align,
            top,
		    width,
		    padding,
		    disableColor,
		    disableOpacity,
		    backgroundColor,
		    borderColor,
		    borderWeight,
		    borderRadius,
		    fadeOutTime,
		    source,
		    loadingImage);
    });

    //This method hides the popup when the escape key is pressed
    $(document).keyup(function (e) {
        if (e.keyCode == 27) {
            closePopup(fadeOutTime);
        }
    });

});

function ValidateForm() {
    var email = document.getElementById("errEmail");
    alert();
    var company = document.getElementById("errCompany");
}