<!--#include file="Connections/intellegesSQL.asp" -->

<% 
   dim tp,sp,ref,shdw,sp_ssap
	 
   tp = request.querystring("tp")
	 sp = request.querystring("sp")
	 ref = Int(request.querystring("ref"))
	 shdw = request.querystring("shdw")
	 
	 select case tp
	   case 0
		   header = "Promote Shadow"
			 sp_ssap = "{call sp_getShadowProviderSSAP }"
			 leftH = "Shadow"
			 rightH = "Reference"
		 case 1 
		   header = "Demote Reference"
			 sp_ssap = "{call sp_getReferenceProviderSSAP }"
  			leftH = "Reference"
			 rightH = "Shadow"

	 end select
	 
	 sub getSSAP()
	   Dim str
			Set ssapRS = Server.CreateObject("ADODB.Recordset")
			ssapRS.ActiveConnection = MM_intellegesSQL_STRING 
			ssapRS.Source =  sp_ssap
			ssapRS.CursorType = 0
			ssapRS.CursorLocation = 2
			ssapRS.LockType = 1
			ssapRS.Open()		
			
  		str = str & "<select name=""ssap"" onChange=""javascript:go(" &tp & ");"">" &vbCrlf
      str = str & "<option>Please select one</option> "  &vbCrlf
			while not ssapRS.eof
			  if (ssapRS("id") = cint(sp)) then
				  selected = "selected"
			  else
				  selected = ""
				end if
			  str = str & "<option value=""" & ssapRS("id") & """ "&selected&">" & ssapRS("name") & "</option>" &vbCrlf
				ssapRS.moveNext
			wend
			str = str & "</select>"
			response.write str
	 end sub
	 
sub getLeft(ssap)
  shdw = Int(request.querystring("shdw"))
  if ssap ="" then
		  ssap = 0
	end if
	
	select case tp
	   case 0
		   sp_name = "{call sp_getShadowProviderBySSAP("& ssap & ")}"
		 case 1
		   sp_name = "{call sp_getReferenceProviderBySSAP("& ssap & ")}"
	End select
	 
	   Dim str,selected
			Set leftRS = Server.CreateObject("ADODB.Recordset")
			leftRS.ActiveConnection = MM_intellegesSQL_STRING 
			leftRS.Source =  sp_name
			leftRS.CursorType = 0
			leftRS.CursorLocation = 2
			leftRS.LockType = 1
			leftRS.Open()			
			str = str & "<select size=""100"" class= ""drop"" name=""leftRS"" onClick=""javascript:go2(" &tp & ");"">" &vbCrlf
			while not leftRS.eof
			 if (shdw = leftRS("id")) then
			  selected = "selected"
			 else
			   selected = ""
			 end if
			  str = str & "<option value=""" & leftRS("id") & """" &selected &">" & leftRS("name") & "</option>" &vbCrlf
				leftRS.moveNext
			wend
			str = str & "</select>"
		response.write str
end sub

sub getRight(ref)
    if (ref = "") then
		  ref = 0
		end if

		select case tp
			 case 0
				 sp_name = "{call sp_getReferenceByShadow("& ref & ")}"
			 case 1
				 sp_name = "{call sp_getShadowByReference("& ref & ")}"
		End select					
     
	   Dim str,selected
			Set rightRS = Server.CreateObject("ADODB.Recordset")
			rightRS.ActiveConnection = MM_intellegesSQL_STRING 
			rightRS.Source =  sp_name
			rightRS.CursorType = 0
			rightRS.CursorLocation = 2
			rightRS.LockType = 1
			rightRS.Open()			
			

			
			str = str & "<select size=""10"" class= ""drop"" name=""rightRS"">" &vbCrlf
			while not rightRS.eof
			 if (ref = rightRS("id")) then
			  selected = "selected"
			 else
			   selected = ""
			 end if
			
			  str = str & "<option value=""" & rightRS("ssap") & " "" " &selected &">" & rightRS("name") & "</option>" &vbCrlf
				rightRS.moveNext
			wend
			str = str & "</select>"
		response.write str
end sub
%>
<html>
<head>
<link rel="stylesheet" type="text/css" href="css/disp.css">
<link rel="stylesheet" type="text/css" href="css/styles.css">
<style>
  body {
	  background-color:#ecf3fb;
	}
  .rightPane {
	 overflow:auto; 
		OVERFLOW-X: scroll; 
		WIDTH: 560px;
		scrollbar-arrow-color:#000090;
	}
	img {
	  border: 0px;
	}
	select.drop {
	  width:270px;
		height:230px;
	}
  select {
	  font-size: 12px;
 }
 td {
   font-size:14px;
	 font-weight:bold;
	 font-family:Helvetica,Verdana, Arial,sans-serif;
	 text-align:center;
	 vertical-align:middle;
 }
</style>
<script language="JavaScript">
<!-- Hide JavaScript from older browsers

function go(tp) {
 sp = document.forms[0].ssap.value
 url = "switch.asp?sp="+sp+"&tp="+tp
 window.location = url;

}
function go2(tp) {
  sp = document.forms[0].ssap.value
	left = document.forms[0].leftRS.value
	url = "switch.asp?tp="+tp+"&sp="+sp+"&shdw="+left
	window.location = url;
	//alert(shdw)
}
// End hiding JavaScript from older browsers -->
</script>
</head>

<body>
<form method="post">
<table cellSpacing=0 cellPadding=0 width="700" align="center" class="grid">
 
 <tr>
	  <td width="698" height="28" valign="top">
			<table cellSpacing=0 cellPadding=0 width=700 class="spreadsheet" align="center">
         <th><span style="text-align:left ">Provider &gt; Switch &gt; <%=header%></span></th>						
			</table>
	  </td>
	</tr>	
	<tr>
	  <td height="25" valign="top">Please select an SSAP: <% getSSAP() %></td>
	</tr>
	<tr>
	 <td height="300" valign="top">
	    <table width="100%" border="0" cellpadding="0" cellspacing="0" class="dropDown">
	     <!--DWLayoutTable-->
				<tr>
					 <td width="312" height="25" valign="top"><%=leftH%></td>
					 <td width="72">&nbsp;</td>
					 <td width="45%" valign="top"><%=rightH%></td>
				</tr>
				<tr>
					 <td rowspan="5" valign="top" align="center"><% getLeft sp %></td>
					 <td height="25">&nbsp;</td>
					 <td width="45%" valign="top" align="center" rowspan="7"><% getRight shdw %></td>				  
				</tr>
				<tr>
				 <td height="31" valign="top">
				   <input type="image" src="images/switch_button.gif" onClick="this.form.action='doSwitch.asp?tp=<%=tp%>'">
				 </td>
			 </tr>
				<tr>
				 <td height="14">&nbsp;</td>
			 </tr>
				<tr>
				 <td height="32" valign="top">
				    <input type="image" src="images/doneSmall_button.gif" onClick="this.form.action='companySearch.asp'">
				</td>
			 </tr>
				<tr>
				 <td height="171">&nbsp;</td>
			 </tr>
			 </table>
	 </td>
  </tr>
</table>
</form>
</body>
</html>