function mtcSubmitForm(obj, browserEvent) 
{
    var getstr = "?";
    var targetstr = "";
    var checkCookie;
   
    if (document.getElementById('UseCookie') == null )
    {
        checkCookie = "";
    }
    else
    {
        if ( document.getElementById('UseCookie').value == "true" )
        {
               checkCookie = document.getElementById('UseCookie').value;
        }
        else
        {
            checkCookie = "";
        }
    }
    
	try {

// generate the query string
        GetCurrentPage();
        getstr += mtc1001s(obj);                 
        targetstr = mtc1000s(obj);
        
        if (checkCookie ) {
            mtc1005(obj);
        }         
        
        
        if (IsActionSFGA() == true)
        {
            ModifyReturnValueForSFGA(encodeURIComponent(targetstr) + getstr);
            //ModifyReturnValueForSFGA("http://localhost:4287/HostedForms/mtcContactReg.aspx" + getstr);
        }
        else
        {
            obj.action = targetstr + getstr;
            //obj.action = "http://localhost:4287/HostedForms/mtcContactReg.aspx" + getstr;
        }
        
        try
        {        
            obj.submit(); 
        }
        catch(submitErr)
        {
        }
	}
	catch(er)
	{
	    if(er=="EmailInvalid") 
	        alert("Error! Email Address is Required");
	    if(er == "ReqEmpty") 
	        alert("Error! A Required Field is Empty");
	    
	    if (navigator.appVersion.indexOf("MSIE") != -1)
	    {
	        window.event.returnValue = false; 
	    }
	    else if (browserEvent != null)
	    {
	        browserEvent.preventDefault();
	    }
	}	
}

function GetCurrentPage()
{
    var currentPage = location.href

    if (currentPage.indexOf("errorMsg") != -1)
    {
        var currentPageSubstring = currentPage.substring(0, currentPage.indexOf("errorMsg"))
        var tempString = currentPage.substring(currentPage.indexOf("errorMsg"))
        
        if (tempString.indexOf("&") != -1)
        {
            tempString = tempString.substring(tempString.indexOf("&")+1)
            currentPage = currentPageSubstring + tempString
        }
        else if ((currentPageSubstring.indexOf("?") == currentPageSubstring.length - 1) ||
            (currentPageSubstring.indexOf("&") == currentPageSubstring.length - 1))
        {
            currentPage = currentPageSubstring.substring(0, currentPageSubstring.length - 1)  
        }
        else
        {
            currentPage = currentPageSubstring
        }
    }
    
    if (document.getElementById("CurrentPage") != null)
    {
        var hiddenCurrentPage = document.getElementById("CurrentPage");
        
        hiddenCurrentPage.value = currentPage;
    }
}

function IsActionSFGA()
{
    var forms = document.forms;
    for( var i = 0; i < forms.length; i++ ) 
    {
        var form = forms[i];
        var _action = form.getAttribute("action");
        
        if (_action != null && (_action == "https://lct.salesforce.com/sfga" || _action == "http://lct.salesforce.com/sfga"))
        {
            return true;
        }
    }
    
    return false;
}

function ModifyReturnValueForSFGA(returnURL)
{
    var forms = document.forms;
    for( var i = 0; i < forms.length; i++ ) 
    {
        var form = forms[i];
        var _action = form.getAttribute("action");
        
        if (_action != null && (_action == "https://lct.salesforce.com/sfga" || _action == "http://lct.salesforce.com/sfga"))
        {
            for( var j = 0; j < form.elements.length; j++ ) 
            {
                var input = form.elements[j];
                if(input.name == 'sfga') 
                {                    
                    var newInputValue;
                    var inputValue = input.value.split("&");

                    if (inputValue != null)
                    {
                        for (var k=0; k < inputValue.length; k++)
                        {
                            if (inputValue[k].indexOf("retURL=") == 0)
                            {
                                newInputValue = newInputValue + "&retURL=" + returnURL;  
                            }
                            else if (newInputValue == null || newInputValue == "")
                            {
                                newInputValue = inputValue[k];
                            }
                            else
                            {
                                newInputValue = newInputValue + "&" + inputValue[k];
                            }
                        }
                    }
                    input.value = newInputValue

                    break;
                }
            }
        }
    }
}

function mtc1000s(obj) {    
    var targetstr = ((location.protocol=='http:')?'http:':'https:') + "//"
    targetstr += document.getElementById('MTC_SERVER').value + "/Data/" + document.getElementById('MTC_GROUP').value + "/" + document.getElementById('MTC_ID').value + "/" + document.getElementById('MTC_KEY').value + "/mtcContactReg.aspx"; 
    return targetstr;
}
   
function mtc1001s(obj) {

      var getstr = "";  
      var children = obj.childNodes; 
      for (var i=0; i<children.length; i++) {    
            getstr = getstr + mtc1002s(children[i])
            getstr = getstr + mtc1001s(children[i])                        
      }     
                              
    return getstr
}
   
function mtc1002s(obj) {

    var getstr = "";
   	 
    if (obj.tagName == "INPUT") {
	    
        if (obj.type == "text") {    
            if ( !mtc1004b(obj) ) {
	            throw "ReqEmpty";
	        }     
	 	    if (obj.name == "EmailAddress") {
		        if (!mtc1003b( obj.value ) ) {
			        throw "EmailInvalid";
		        }
		    }
		    if (obj.value.length > 0) {
                getstr += obj.name + "=" + escape(obj.value) + "&";		
            }
        }        
        
        //Am currently not escaping hidden fields because those can be controlled more on a user level
        if (obj.type == "hidden") {  
            if ( !mtc1004b(obj) ) {
	            throw "ReqEmpty";
	        } 
            if (obj.value.length > 0) 
            { 
                if ((obj.name.toLowerCase() != "sfga") || (IsActionSFGA() == false))
                {
                    getstr += obj.name + "=" + obj.value + "&";
                }
            }
        }
        
        if (obj.type == "checkbox") {
            if ( !mtc1004abc(obj) ) {
	            throw "ReqEmpty";
	        }
	        if (obj.name.indexOf("ListManagement") >= 0 ) {
                if (!obj.checked && document.getElementById('ListOff') != null) {    
                  document.getElementById('ListOff').value += escape(obj.value) + ","                                  
                }
            }
            else
            {
                if (obj.checked) 
                {    
                    getstr += obj.name + "=" + escape(obj.value) + "&";                 
                }
                else
                {
                    var node = obj
                    while (node.nodeName.toLowerCase() != "form" && node.parentNode) 
                    {
                        node = node.parentNode;
                    }

                    var input = document.createElement("input");
                    input.setAttribute("type", "hidden");
                    input.setAttribute("name", obj.name);
                    input.setAttribute("value", "");
                    
                    if ((node.id != null) && (node.id != ""))
                    {
                        var tempNode = document.getElementById(node.id)
                        
                        if (tempNode != null)
                        {
                            tempNode.appendChild(input);
                        }
                    }                      
                } 
            }
            
        }
        if (obj.type == "radio") { 
            if ( !mtc1004abc(obj) ) {
	            throw "ReqEmpty";
	        }        
            if (obj.checked) {
                getstr += obj.name + "=" + escape(obj.value) + "&";                 
            }
        }
    } 
      
    if (obj.tagName == "SELECT" && obj.disabled == false) {      
        if ( !mtc1004ab(obj) ) {
	        throw "ReqEmpty";
	    } 
	   
        var sel = obj;
        if (sel.selectedIndex != -1) { 	 
          if (sel.name == "Country" && document.getElementById('MTC_CRM') != null && document.getElementById('MTC_CRM').value == "MTCSUBSVC_SIEBEL" && (sel.options[sel.selectedIndex].text != "-None-"))
          {
            getstr += sel.name + "=" + escape(sel.options[sel.selectedIndex].text) + "&";  
            
            if (document.getElementById('CRMCountry') != null)
            {
                document.getElementById('CRMCountry').value = escape(sel.options[sel.selectedIndex].text);
            }
          }
          else
          {
            getstr += sel.name + "=" + escape(sel.options[sel.selectedIndex].value) + "&";           
          }
        } else {
            var selected;
            for (var i = 0; i < sel.options.length; i++) {
                if (sel.options[i].selected) 
                {
                    selected += escape(sel.options[i].value) + "|";
                }
            }
            
            getstr += sel.name + "=" + selected
        }
    }    
    if (obj.tagName == "TEXTAREA") {
        if ( !mtc1004b(obj) ) {
	        throw "ReqEmpty";
	    }
        
        if (IsActionSFGA() == true)
        {
            if (obj.value.indexOf("\r\n") > -1)
            {
                obj.value = obj.value.replace(/\r\n/g, " ");
            }        
            else if (obj.value.indexOf("\n") > -1)
            {
                obj.value = obj.value.replace(/\n/g, " ");
            }            
            else if (obj.value.indexOf("\r") > -1)
            {
                obj.value = obj.value.replace(/\r/g, " ");
            }
        }
        
        //getstr += obj.name + "=" + escape(obj.value) + "&";      
        getstr += obj.name + "=" + encodeURI(obj.value) + "&";     
    }
    
    //Am currently not escaping hidden fields because those can be controlled more on a user level
    if (obj.tagName == "HIDDEN" ) {
        if ( !mtc1004b(obj) ) {
	        throw "ReqEmpty";
	    }
	    if (obj.value.length > 0 ) {
            getstr += obj.name + "=" + obj.value + "&"; 
        }
    }
   
    return getstr    
}   

function mtc1003b(str) {
    //  return (str.search(/^\w+((-\w+)|(\.\w+))*\@[A-Za-z0-9]+((\.|-)[A-Za-z0-9]+)*\.[A-Za-z0-9]+$/) != -1);
    return (str.search(/^[a-z0-9!#$%&'*+\/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+\/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+(?:[A-Z]{2}|aero|asia|biz|cat|com|coop|edu|gov|info|int|jobs|mil|mobi|museum|name|net|org|pro|tel|travel)$/i) != -1 );
    //^[A-Z0-9._%+-]+@(?:[A-Z0-9-]+\.)+(?:[A-Z]{2}|com|org|net|gov|mil|biz|info|mobi|name|aero|jobs|museum)$
}

function mtc1004b(obj) {  
    if (!IsNullOrEmpty(obj.getAttribute("isRequired")) && obj.getAttribute("isRequired").toLowerCase() == "true" && obj.value.length == 0 ) {       
	    return false;
    } else {
	    return true;
    } 
}

function mtc1004ab(obj) 
{  
    if (!IsNullOrEmpty(obj.getAttribute("isRequired")) && 
        obj.getAttribute("isRequired").toLowerCase() == "true" && 
        (obj.selectedIndex == -1 || 
            obj.options[obj.selectedIndex].value.length == 0 || 
            obj.options[obj.selectedIndex].value == "-1" ||
            obj.options[obj.selectedIndex].value == "-Auto Populate-")) 
    { 
	    return false;
    } else {
	    return true;
    } 
}

function mtc1004abc(obj) {  

    if (!IsNullOrEmpty(obj.getAttribute("isRequired")) && obj.getAttribute("isRequired").toLowerCase() == "true" && !obj.checked) {     
	    return false;
    } else {
	    return true;
    } 
}

function mtc1005() {

//get cookie, check for the value, update, and set
    var nameEQ = "MTC_FORM=";
	var ca = document.cookie.split(';');
	var value = "";
	var found = false;
	var formId = "";
	
	if ( document.getElementById('FormID') == null )
	{	
	    return;
	}
	else
	{
	    formID = document.getElementById('FormID').value; 	  
	}
	
	for(var i=0;i < ca.length;i++) {
		var c = ca[i];
		while (c.charAt(0)==' ') c = c.substring(1,c.length);
		if (c.indexOf(nameEQ) == 0)
		{
		    value = c.substring(nameEQ.length,c.length);
		    break;
		}
	}	
	var caValue = "";
	if (value.length > 0)
	{
	    caValue = value.split(',');
    }	
	for(var i=0;i<caValue.length;i++)
	{	   
	    if (caValue[i] == formID)
	    {
	        found = true;
	        break;
	    }
	}	
	if (!found)
	{
	    value =  formID + ',' + value;
	}
	var date = new Date();
	// cookie will expire in one year
	date.setTime(date.getTime()+(365*24*60*60*1000));
	var test = nameEQ + value + ";expires=" + date.toGMTString() + "; path=/";
	document.cookie = test
}

var oldonload = window.onload
if (typeof window.onload != 'function') 
{   
   window.onload = mtc1006;
 } 
 else 
 {   
    window.onload = function() 
    {
      if (oldonload) 
      {
        oldonload();
      }
      
      mtc1006();
    };
}

function mtc1006()
{
    var nameEQ = "MTC_FORM=";
	var ca = document.cookie.split(';');
	var value = "";
	var found = false;
	var formID = "";	
		
	mtc1008();
	mtc1009();
	
	//alert(document.cookie);
	if (document.getElementById('UseCookie') == null )
    {
        return true;
    }
    else if( document.getElementById('UseCookie').value == "false" )
    {
        return true;
    }
    
	if ( document.getElementById('FormID') == null )
	{	
	    return true;
	}
	else
	{
	    formID = document.getElementById('FormID').value; 	  
	}
	
	for(var i=0;i < ca.length;i++) {
		var c = ca[i];
		while (c.charAt(0)==' ') c = c.substring(1,c.length);
		if (c.indexOf(nameEQ) == 0)
		{
		    value = c.substring(nameEQ.length,c.length);
		}
	}	
	var caValue = "";
	if (value.length > 0)
	{
	    caValue = value.split(',');
    }	
	for(var i=0;i<caValue.length;i++)
	{	   
	    if (caValue[i] == formID )
	    {
	        found = true;
	        break;
	    }
	}	
	if (found)
	{	   
	    var redirect = "";
	    if( document.getElementById('Redirect') == null )
	    {
	        redirect = document.location;
	    }
	    else
	    {
	        redirect = document.getElementById('Redirect').value;
	    }
	    document.write("<html><body></body></html>");
	    document.location = redirect;
	}
}

function mtc1007( Mappings ) {
    var qsParm = new Array();   

    var query = window.location.search.substring(1);
    var parms = query.split('&');
    for (var i=0; i<parms.length; i++) {
        var pos = parms[i].indexOf('=');
        if (pos > 0) {
            var key = parms[i].substring(0,pos);       
            var val = parms[i].substring(pos+1);        
            qsParm[key] = val;
        }
    }   

    for (var i in Mappings) {
      //  alert(i + " : " + Mappings[i] + " : " + qsParm[i]);
        var field = document.getElementById(Mappings[i]);
        if (field != null) { 
           // alert(qsParm[i]);   
            if ( qsParm[i] != null ) {
                field.value = qsParm[i];
            }
        }
    }
}


function mtc1008()
{
    var currentPage = document.URL;
    
    if (document.getElementById("MTC_Errors") != null)
    {
        if (currentPage.indexOf("errorMsg") == -1)
        {
            var lblErrorMsg         = document.getElementById("MTC_Errors");
            lblErrorMsg.innerHTML   = "";
        }
        else
        {
            var errors = currentPage.substring(currentPage.indexOf("errorMsg=") + 9);

            errors = errors.replace(/%20/g, " ");
            
            var errorArray = errors.split(",");
            
            errors = "<table>";

            for (i=0; i < errorArray.length; i++)
            {
                switch (errorArray[i])
                {
                
                    case "Email Error":
                        errors += "<tr>";
                        errors += "<td><font color=\"#FF0000\">" + errorArray[i] + "</font></td>"
                        errors += "<td><font color=\"#FF0000\">The Email entered is not valid.</font></td>"
                        errors += "</tr>"
                        break;
                        
                    case "WorkPhone Error":
                    case "MobilePhone Error":
                    case "Fax Error":
                    case "HomePhone Error":
                        errors += "<tr>";
                        errors += "<td><font color=\"#FF0000\">" + errorArray[i] + "</font></td>"
                        errors += "<td><font color=\"#FF0000\">The Number entered is not valid.</font></td>"
                        errors += "</tr>"
                        break;
                }
            }
            errors += "</table>"
            
            var lblErrorMsg         = document.getElementById("MTC_Errors");
            lblErrorMsg.innerHTML   = errors;
        }
    }
}

function mtc1009()
{

    var region = document.getElementById("Region");
    var country = document.getElementById("Country");
     
    // if there is no country field, just look at the regioncode of the Region field. If US, populate with US states,
    // otherwise leave the default values.
    if( country == null)
    {
        if (region != null )       
        {
            // only have a region field. If there are more options than -Auto Populate- use those values,
            // if only one option is in the list, check the regioncode and add the US states.
            if (region.getAttribute("regioncode") != "NONE")
            {                  
               mtc1010(region);    
            }
        }    
    }    
    else
    {
        if( region == null)
        {
            if (country.getAttribute("regioncode") != "NONE")
            { 
                mtc1011(country);
            }
        }
        else
        {   
            if (country.getAttribute("regioncode") != "NONE")
            {      
                 var countrycode = country.getAttribute("regioncode");       
                 if ( countrycode == "AUTO" ) {
                    region.disabled = true;
                    country.onchange = function() { mtc1012(region, country); }
                    mtc1011(country);
                    mtc1010(region);
                 }
                  else 
                 {
                    region.setAttribute("regioncode", countrycode);
                    mtc1011(country);
                    mtc1010(region);
                 }
            }
            else
            {
                if (region.getAttribute("regioncode") != "NONE")
                {                  
                    mtc1010(region);    
                }
            }
        }
    }    
}

function mtc1012(region, country)
{
    if (country.options[country.selectedIndex].value == "US")
    {
        region.setAttribute("regioncode", "US");
        region.disabled = false;
        mtc1010(region);
    } 
    else if (country.options[country.selectedIndex].value == "AU")
    {
        region.setAttribute("regioncode", "AU");
        region.disabled = false;
        mtc1010(region);    
    }
    else if (country.options[country.selectedIndex].value == "CA")
    {
        region.setAttribute("regioncode", "CA");
        region.disabled = false;
        mtc1010(region);    
    }
    else
    {
        region.disabled = true;
        mtc1010(region);    
    }
}


function mtc1010(region)
{
    //We are making an assumption that region is a dropdown, but that is not always the case
    if (region.options == null)
    {
       return;
    }
     
     region.options.length = 0;  
     var optionObject = new Option("-None-","");
     var optionRank = region.options.length;
     region.options[optionRank]=optionObject;  
     
     mtc1010_Default(region)
}

function mtc1010_Default(region)
{
    var regioncode = region.getAttribute("regioncode");
    if ( regioncode == "AUTO" || regioncode == "US")
    {
        // add the us states       
        optionObject = new Option("AL-Alabama","AL");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                    
        optionObject = new Option("AK-Alaska","AK");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("AZ-Arizona","AZ");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option( "AR-Arkansas","AR");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("CA-California","CA");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("CO-Colorado","CO");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("CT-Connecticut","CT");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("DE-Delaware","DE");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("DC-District of Columbia","DC");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("FL-Florida","FL");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("GA-Georgia","GA");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("HI-Hawaii","HI");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("ID-Idaho","ID");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("IL-Illinois","IL");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("IN-Indiana","IN");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("IA-Iowa","IA");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("KS-Kansas","KS");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("KY-Kentucky","KY");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option( "LA-Louisiana","LA");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("ME-Maine","ME");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("MD-Maryland","MD");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("MA-Massachusetts","MA");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("MI-Michigan","MI");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("MN-Minnesota","MN");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("MS-Mississippi","MS");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("MO-Missouri","MO");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("MT-Montana","MT");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("NE-Nebraska","NE");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("NV-Nevada","NV");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("NH-New Hampshire","NH");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("NJ-New Jersey","NJ");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("NM-New Mexico","NM");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("NY-New York" ,"NY");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("NC-North Carolina","NC");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("ND-North Dakota","ND");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("OH-Ohio","OH");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("OK-Oklahoma","OK");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("OR-Oregon","OR");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("PA-Pennsylvania","PA");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("RI-Rhode Island","RI");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option( "SC-South Carolina","SC");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("SD-South Dakota","SD");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("TN-Tennessee","TN");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("TX-Texas","TX");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("UT-Utah","UT");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("VT-Vermont","VT");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("VA-Virginia","VA");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option( "WA-Washington","WA");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option( "WV-West Virginia","WV");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("WI-Wisconsin","WI");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("WY-Wyoming","WY");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;
    }
    if (regioncode == "CA")
    {
        optionObject = new Option("BC-British Columbia", "BC");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("AB-Alberta", "AB");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("SK-Saskatchewan", "SK");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("MB-Manitoba", "MB");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("ON-Ontario", "ON");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("QC-Quebec", "QC");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("NB-New Brunswick", "NB");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("NS-Nova Scotia", "NS");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("PE-Prince Edward Island", "PE");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("NL-Newfoundland and Labrador", "NL");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("YT-Yukon", "YT");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("NT-Northwest Territories", "NT");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("NU-Nunavut", "NU");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;     
    }
    if (regioncode == "AU")
    {
        optionObject = new Option("NSW-New South Wales", "NSW");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("QLD-Queensland", "QLD");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("SA-South Australia", "SA");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("TAS-Tasmania", "TAS");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("VIC-Victoria", "VIC");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("WA-Western Australia", "WA");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("ACT-Australian Capital Territory", "ACT");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("JBT-Jervis Bay Territory", "JBT");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;                 
        optionObject = new Option("NT-Northern Territory", "NT");
        optionRank = region.options.length;
        region.options[optionRank]=optionObject;     
    }
}

function mtc1011(country)
{
    if (country.options == null)
    {
        return;
    }
    
     country.options.length = 0;    
         var optionObject = new Option("-None-","");
        var optionRank = country.options.length;
        country.options[optionRank]=optionObject;       
    
   
     // add the us states       
     if (document.getElementById('MTC_CRM') != null && document.getElementById('MTC_CRM').value == "MTCSUBSVC_SIEBEL")
     {
        optionObject = new Option("USA" ,"US");
        optionRank = country.options.length;
        country.options[optionRank]=optionObject;
     }
     else
     {
        optionObject = new Option("United States" ,"US");
        optionRank = country.options.length;
        country.options[optionRank]=optionObject;
     }
                         
    optionObject = new Option("Albania","AL");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Algeria","DZ");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option( "American Samoa","AS");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Andorra","AD");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;  
    optionObject = new Option("Angola","AO");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Anguilla","AI");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Antarctica","AQ");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Antigua and Barbuda","AG");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Argentina","AR");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Armenia","AM");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Aruba","AW");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Australia","AU");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Austria","AT");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Azerbaijan","AZ");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Bahamas","BS");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Bahrain","BH");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Bangladesh","BD");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Barbados","BB");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Belarus","BY");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Belgium","BE");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Belize","BZ");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Benin","BJ");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Bermuda","BM");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Bhutan","BT");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Bolivia","BO");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Bosnia and Herzegovina","BA");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Botswana","BW");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Bouvet Island","BV");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Brazil","BR");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("British Indian Ocean Territory","IO");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("British Virgin Islands","VG");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Brunei","BN");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Bulgaria","BG");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Burkina Faso","BF");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Burundi","BI");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Cambodia","KH");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Cameroon","CM");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Canada","CA");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Cape Verde","CV");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Cayman Islands","KY");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Central African Republic","CF");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Chad","TD");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Chile","CL");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("China","CN");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Christmas Island","CX");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Cocos Islands","CC");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Colombia","CO");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Comoros","KM");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Congo","CG");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Cook Islands","CK");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Costa Rica","CR");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Croatia","HR");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Cuba","CU");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Cyprus","CY");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Czech Republic","CZ");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Denmark","DK");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Djibouti","DJ");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Dominica","DM");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Dominican Republic","DO");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("East Timor","TP");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Ecuador","EC");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Egypt","EG");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("El Salvador","SV");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Equatorial Guinea","GQ");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Eritrea","ER");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Estonia","EE");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Ethiopia","ET");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Falkland Islands","FK");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Faroe Islands","FO");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Fiji","FJ");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Finland","FI");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("France","FR");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("French Guiana","GF");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("French Polynesia","PF");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("French Southern Territories","TF");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Gabon","GA");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Gambia","GM");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Georgia","GE");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Germany","DE");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Ghana","GH");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Gibraltar","GI");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Greece","GR");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Greenland","GL");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Grenada","GD");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Guadeloupe","GP");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Guam","GU");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Guatemala","GT");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Guinea","GN");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Guinea-Bissau","GW");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Guyana","GY");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Haiti","HT");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Heard and McDonald Islands","HM");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Honduras","HN");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Hong Kong","HK");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Hungary","HU");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Iceland","IS");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("India","IN");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Indonesia","ID");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Iran","IR");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Iraq","IQ");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Ireland","IE");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Israel","IL");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Italy","IT");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Ivory Coast","CI");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Jamaica","JM");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Japan","JP");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Jordan","JO");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Kazakhstan","KZ");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Kenya","KE");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Kiribati","KI");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Korea, North","KP");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Korea, South","KR");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Kuwait","KW");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Kyrgyzstan","KG");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Laos","LA");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Latvia","LV");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Lebanon","LB");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Lesotho","LS");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Liberia","LR");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Libya","LY");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Liechtenstein","LI");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Lithuania","LT");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Luxembourg","LU");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Macau","MO");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Macedonia, Former Yugoslav Republic of","MK");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Madagascar","MG");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Malawi","MW");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Malaysia","MY");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Maldives","MV");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Mali","ML");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Malta","MT");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Marshall Islands","MH");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Martinique","MQ");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Mauritania","MR");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Mauritius","MU");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Mayotte","YT");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Mexico","MX");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Micronesia, Federated States of","FM");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Moldova","MD");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Monaco","MC");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Mongolia","MN");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Montserrat","MS");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Morocco","MA");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Mozambique","MZ");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Myanmar","MM");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Namibia","NA");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Nauru","NR");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Nepal","NP");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Netherlands","NL");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Netherlands Antilles","AN");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("New Caledonia","NC");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("New Zealand","NZ");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Nicaragua","NI");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Niger","NE");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Nigeria","NG");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Niue","NU");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Norfolk Island","NF");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Northern Mariana Islands","MP");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Norway","NO");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Oman","OM");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Pakistan","PK");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Palau","PW");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Panama","PA");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Papua New Guinea","PG");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Paraguay","PY");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Peru","PE");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Philippines","PH");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Pitcairn Island","PN");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Poland","PL");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Portugal","PT");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Puerto Rico","PR");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Qatar","QA");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Reunion","RE");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Romania","RO");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Russia","RU");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Rwanda","RW");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("S. Georgia and S. Sandwich Isls.","GS");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Saint Kitts & Nevis","KN");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Saint Lucia","LC");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Saint Vincent and The Grenadines","VC");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Samoa","WS");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("San Marino","SM");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Sao Tome and Principe","ST");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Saudi Arabia","SA");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Senegal","SN");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Seychelles","SC");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Sierra Leone","SL");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Singapore","SG");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Slovakia","SK");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Slovenia","SI");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Somalia","SO");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("South Africa","ZA");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Spain","ES");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Sri Lanka","LK");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("St. Helena","SH");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("St. Pierre and Miquelon","PM");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Sudan","SD");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Suriname","SR");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Svalbard and Jan Mayen Islands","SJ");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Swaziland","SZ");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Sweden","SE");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Switzerland","CH");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Syria","SY");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Taiwan","TW");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Tajikistan","TJ");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Tanzania","TZ");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Thailand","TH");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Togo","TG");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Tokelau","TK");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Tonga","TO");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Trinidad and Tobago","TT");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Tunisia","TN");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Turkey","TR");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Turkmenistan","TM");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Turks and Caicos Islands","TC");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Tuvalu","TV");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Uganda","UG");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Ukraine","UA");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("United Arab Emirates","AE");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("United Kingdom","UK");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;        
    
    // add the us states       
     if (document.getElementById('MTC_CRM') != null && document.getElementById('MTC_CRM').value == "MTCSUBSVC_SIEBEL")
     {
        optionObject = new Option("USA" ,"US");
        optionRank = country.options.length;
        country.options[optionRank]=optionObject;
     }
     else
     {
        optionObject = new Option("United States" ,"US");
        optionRank = country.options.length;
        country.options[optionRank]=optionObject;
     }
             
//    optionObject = new Option("United States","US");
//    optionRank = country.options.length;
//    country.options[optionRank]=optionObject;                 
    optionObject = new Option("U.S. Minor Outlying Islands","UM");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Uruguay","UY");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Uzbekistan","UZ");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Vanuatu","VU");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Vatican City","VA");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Venezuela","VE");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Vietnam","VN");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Virgin Islands","VI");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Wallis and Futuna Islands","WF");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Western Sahara","EH");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Yemen","YE");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Yugoslavia (Former)","YU");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Zaire","ZR");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;                 
    optionObject = new Option("Zambia","ZM");
    optionRank = country.options.length;
    country.options[optionRank]=optionObject;     

}

function IsNullOrEmpty(_string)
{
    if (_string == null)
    {
        return true;
    }
    else
    {
        var s = _string.replace(" ", "");
        if (s.length == 0)
        {
            return true;
        }
    }
    return false;
}



