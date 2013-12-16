<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<title>Untitled Document</title>
<link href="css/dhtmlwindow.css" rel="stylesheet" type="text/css" />
<link href="css/modal.css" rel="stylesheet" type="text/css" />

<script language="javascript">
function validate(){
var msg = "";
var name = document.getElementById("name").value;
var address = document.getElementById("address").value;
var city = document.getElementById("city").value;
var state = document.getElementById("state").value;
var postalCode = document.getElementById("postalcode").value;
var country = document.getElementById("country").value;
var firstName = document.getElementById("firstname").value;
var lastName = document.getElementById("lastname").value;
var title = document.getElementById("title").value;
var phone = document.getElementById("phone").value;
var email = document.getElementById("email").value;
var subject = document.getElementById("subject").value;

if (name.length == 0) {
msg = msg + "Company name is required \n";
}

if (address.length == 0) {
msg = msg + "Address One is required \n";
}

if (city.length == 0) {
msg = msg + "City is required \n";
}

if (country == "United" && state == 0) {
msg = msg + "Please select a state \n"
}

if (postalCode.length == 0) {
msg = msg + "Postal Code is required \n";
}

if (country == 0) {
msg = msg + "Please select a country \n";
}

if (firstName.length == 0) {
msg = msg + "First Name is required \n";
}

if (lastName.length == 0) {
msg = msg + "Last Name is required \n";
}

if (title.length == 0) {
msg = msg + "Title is required \n";
}

if (phone.length == 0) {
msg = msg + "Phone is required \n";
}

if (email.length == 0) {
msg = msg + "Email is required \n";
}

if (subject.length == 0) {
msg = msg + "Subject is required \n";
}

if (msg.length > 1) {
alert(msg) ;
return false;}
else  {
return true; }

}

</script>
</head>

<?php include("connection/connection.php"); ?>

<?php
function getState()
{
  $results= mssql_query($sqlquery);
  
  $sqlquery="SELECT * FROM state";

  $results= mssql_query($sqlquery);

  while ($row=mssql_fetch_array($results)){
  $name = $name."<option value=".$row[stateCode].">".$row[stateCode]."</option><br/>";
  }
  mssql_close($sqlconnect);
  echo $name;
}

function getCountry()
{
  $sqlconnect=mssql_connect("localhost,1433", "intelleges", "dcN43hs$7");
  $sqldb=mssql_select_db("homesiteINTELLEGESProd",$sqlconnect);
  $results= mssql_query($sqlquery);
  
  $sqlquery="SELECT * FROM country";

  $results= mssql_query($sqlquery);

  while ($row=mssql_fetch_array($results)){
  $name = $name."<option value=".$row[name].">".$row[name]."</option><br/>";
  }
  mssql_close($sqlconnect);
  echo $name;
}
?>

<body>
<form name="contactUsForm" id="contactUsForm" method="post" action="doContactUs.asp" >
<table width="425" border="0" cellspacing="0" cellpadding="0">
  <tr>
    <td class="normaltext"><table width="100%" border="0" cellspacing="0" cellpadding="0">
      <tr>
        <td height="10" width="29%"></td>
        <td width="3%"></td>
        <td width="68%"></td>
      </tr>
      <tr>
        <td align="right">Company Name*</td>
        <td></td>
        <td><input class="contactbox" type="text" name="name" id="name" /></td>
      </tr>
      <tr>
        <td height="10" width="29%"></td>
        <td width="3%"></td>
        <td width="68%"></td>
      </tr>
      <tr>
        <td align="right">Address One*</td>
        <td></td>
        <td><input class="contactbox" type="text" name="address" id="address" /></td>
      </tr>
      <tr>
        <td height="10" width="29%"></td>
        <td width="3%"></td>
        <td width="68%"></td>
      </tr>
      <tr>
        <td align="right">Address Two </td>
        <td></td>
        <td><input class="contactbox" type="text" name="address2" id="address2" /></td>
      </tr>
      <tr>
        <td height="10" width="29%"></td>
        <td width="3%"></td>
        <td width="68%"></td>
      </tr>
      <tr>
        <td align="right">City*</td>
        <td></td>
        <td><input class="contactbox" type="text" name="city" id="city" /></td>
      </tr>
      <tr>
        <td height="10" width="29%"></td>
        <td width="3%"></td>
        <td width="68%"></td>
      </tr>
      <tr>
        <td align="right">State* <? echo 'TAj'; ?></td>
        <td></td>
        <td><select class="contactdropdown" name="state" id="state" style="width: 260px;">
		<option value="0">Please select one</option>
		<?php getState(); ?>
          </select>
        </td>
      </tr>
      <tr>
        <td height="10" width="29%"></td>
        <td width="3%"></td>
        <td width="68%"></td>
      </tr>
      <tr>
        <td align="right">Postalcode*</td>
        <td></td>
        <td><input class="contactboxone" type="text" name="postalcode" id="postalcode" /></td>
      </tr>
      <tr>
        <td height="10" width="29%"></td>
        <td width="3%"></td>
        <td width="68%"></td>
      </tr>
      <tr>
        <td align="right">Country*</td>
        <td></td>
        <td><select class="contactdropdown" name="country" id="country" style="width: 260px;">
			<option value="0">Please select one</option>
	<?php getCountry() ?>
        </select></td>
      </tr>
      <tr>
        <td height="10" width="29%"></td>
        <td width="3%"></td>
        <td width="68%"></td>
      </tr>
      <tr>
        <td align="right">First Name*</td>
        <td></td>
        <td><input class="contactbox" type="text" name="firstname" id="firstname" /></td>
      </tr>
      <tr>
        <td height="10" width="29%"></td>
        <td width="3%"></td>
        <td width="68%"></td>
      </tr>
      <tr>
        <td align="right">Last Name*</td>
        <td></td>
        <td><input class="contactbox" type="text" name="lastname" id="lastname" /></td>
      </tr>
      <tr>
        <td height="10" width="29%"></td>
        <td width="3%"></td>
        <td width="68%"></td>
      </tr>
      <tr>
        <td align="right">Title*</td>
        <td></td>
        <td><input class="contactbox" type="text" name="title" id="title" /></td>
      </tr>
      <tr>
        <td height="10" width="29%"></td>
        <td width="3%"></td>
        <td width="68%"></td>
      </tr>
      <tr>
        <td align="right">Phone*</td>
        <td></td>
        <td><table width="100%" border="0" cellspacing="0" cellpadding="0">
            <tr>
              <td width="44%"><input class="contactboxone" type="text" name="phone" id="phone" /></td>
              <td width="9%">Fax</td>
              <td width="47%"><input class="contactboxone" type="text" name="Fax" id="Fax" /></td>
            </tr>
        </table></td>
      </tr>
      <tr>
        <td height="10" width="29%"></td>
        <td width="3%"></td>
        <td width="68%"></td>
      </tr>
      <tr>
        <td align="right">Email*</td>
        <td></td>
        <td><input class="contactbox" type="text" name="email" id="email" /></td>
      </tr>
      <tr>
        <td height="10" width="29%"></td>
        <td width="3%"></td>
        <td width="68%"></td>
      </tr>
      <tr>
        <td align="right">Subject*</td>
        <td></td>
        <td><input class="contactbox" type="text" name="subject" id="subject" /></td>
      </tr>
      <tr>
        <td height="10" width="29%"></td>
        <td width="3%"></td>
        <td width="68%"></td>
      </tr>
      <tr>
        <td align="right" valign="top">Note</td>
        <td></td>
        <td><textarea class="contactbox" name="note" id="note" cols="38" rows="5"></textarea></td>
      </tr>
      <tr>
        <td height="10"></td>
        <td></td>
        <td></td>
      </tr>
      <tr>
        <td height="10"></td>
        <td></td>
        <td><table width="100%" border="0" cellspacing="0" cellpadding="0">
            <tr>
              <td width="28%"><input class="contactsubmit" type="submit" name="submit" id="submit" value="" onclick="return validate()"/></td>
              <td width="72%"><input class="contactcancle" type="button" name="submit2" id="submit2" value="" onClick="javascript: window.close()" /></td>
            </tr>
        </table></td>
      </tr>
      <tr>
        <td height="10" width="29%"></td>
        <td width="3%"></td>
        <td width="68%"></td>
      </tr>
    </table></td>
  </tr>
  <tr>
    <td height="20"></td>
  </tr>
</table>
</div>
</body>
</html>
