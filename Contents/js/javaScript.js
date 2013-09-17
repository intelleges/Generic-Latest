// JScript File
//This function is used on sSupplierSearch.aspx
function validateSelectDatabase() 
{
    var keyword = document.getElementById("ctl00_adminBodyContent_txtKeyword").value;
    var msg = "";
	var valid = false;
	var vendorID = document.getElementById("ctl00_adminBodyContent_txtVendorId").value;
	var naics = document.getElementById("ctl00_adminBodyContent_ddlNaicsCode").value;
	var zcode = document.getElementById("ctl00_adminBodyContent_ddlZcode").value;
	var spendCategory = document.getElementById("ctl00_adminBodyContent_lbxAvailableSpendCategory").value;
	var current = document.getElementById("ctl00_adminBodyContent_cbxCurrent");
	var pending = document.getElementById("ctl00_adminBodyContent_cbxPending");
	var prospective = document.getElementById("ctl00_adminBodyContent_cbxProspective");
	var pronet = document.getElementById("ctl00_adminBodyContent_cbxPronet");
	
    if(!current.checked && !prospective.checked && !pronet.checked && !pending.checked) {
	  msg = "You must check one or more of the categories under 'Select Database'!"
	}
  else {
	  if (keyword == "" && naics == "") {
		  if (current.checked || prospective.checked || pending.checked ) {
			  valid = true;
			}
			else {
			  msg = "You must enter keyword from Pronet Search";
			}
		}
		else 
		 valid = true;
	  //valid = true;  
	}
	if (vendorID != "") {
		 if (current.checked || prospective.checked || pending.checked) {
			  valid = false;
			  msg = "Vendor IDs are can only search the Current Database";
			}
		}
		
	if (zcode != "") {
		 if (pronet.checked && keyword == "" && naics == "" ) {
			  valid = false;
			  msg = "Zcode can only search the Current, Pending, and/or Prospective Database";
			}
		}
	
	if (valid) {
	  selectAll();
	  	if (spendCategory != "") {
		 if (pronet.checked) {
			  msg = "Spend Category can only search the Current, Pending, and/or Prospective Database";
			  alert(msg);
		return false;
			}
			else {
			return true;
			}
		}
	}
	else {
	  alert(msg);
		return false;
	}
}

function selectAll() {
var spendCategory = document.getElementById("ctl00_adminBodyContent_lbxAvailableSpendCategory");
var category = "";
	for (var i=0;i < spendCategory.length;i++)
		 {
			 category = spendCategory[i];
			 category.selected= true
		 }

}

function hideStar()
        {
            var inputs = document.getElementsByTagName('span');
            var actionType = getQueryVariable('actionType');
            if (actionType != "1" && actionType != "10")
            {
                for (var i = 0; i < inputs.length; i++)
                {
                   if (inputs[i].className == "star")
                  {
                     inputs[i].style.display = "none";
                  }
                }
            }
        }
        
        function getQueryVariable(variable) 
        {
            var query = window.location.search.substring(1);
            var vars = query.split("&");
            for (var i=0;i<vars.length;i++) 
            {
                var pair = vars[i].split("=");
                    if (pair[0] == variable) 
                    {
                        return pair[1];
                    }
            }
            return "";
        } 