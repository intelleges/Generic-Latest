<!--#include file="connection/connectionAsp.asp" -->

<%
dim webinarId, webinar, name, address, address2, city, state, stateId, countryId, postalCode, country, firstName, lastName, title, phone, fax, emailAddress, subject, note

webinarId = request.form("webinar")
webinar = getWebinar(webinarId)
name = request.form("name")
address = request.form("address")
address2 = request.form("address2")
city = request.form("city")
stateId = request.form("state")
state = getState(stateId)
postalCode = request.form("postalcode")
countryId = request.form("country")
country = getCountry(countryId)
firstName = request.form("firstName")
lastName = request.form("lastName")
title = request.form("title")
phone = request.form("phone")
fax = request.form("fax")
emailAddress = request.form("email")
subject = request.form("subject")
note = request.form("note")


body = body & "Webinar: " & webinar & "<Br />"
body = body & "Company Name: " & name & "<Br />"
body = body & "Address: " & address & "<br />"
body = body & "Address2: " & address2 & "<br />"
body = body & "City: " & city & "<br />"
body = body & "State: " & state & "<br />"
body = body & "Postal Code: " & postalCode & "<br />"
body = body & "Country: " & country & "<br />"
body = body & "First Name: " & firstName & "<br />"
body = body & "Last Name: " & lastName & "<br />"
body = body & "Title: " & title & "<br />"
body = body & "Phone: " & phone & "<br />"
body = body & "Fax: " & fax & "<br />"
body = body & "Email: " & emailAddress & "<br />"
body = body & "Subject: " & subject & "<br />"
body = body & "Note: " & note & "<br />"

function getWebinar(webinarId)
	  set rs = Server.Createobject("ADODB.Recordset")
		rs.ActiveConnection = MM_intellegesSQL_STRING
		rs.Source = "{call sp_getWebinar(" & webinarId & ")}"
		rs.CursorType = 0
		rs.CursorLocation = 2
		rs.LockType = 1
		rs.Open()
		
		if not rs.eof then
			getWebinar = rs("description")
		end if
		
		rs.close()
		set rs = nothing
end function

function getState(stateId)
	  set rs = Server.Createobject("ADODB.Recordset")
		rs.ActiveConnection = MM_intellegesSQL_STRING
		rs.Source = "{call sp_getState(" & stateId & ")}"
		rs.CursorType = 0
		rs.CursorLocation = 2
		rs.LockType = 1
		rs.Open()
		
		if not rs.eof then
			getState = rs("name")
		end if
		
		rs.close()
		set rs = nothing
end function

function getCountry(countryId)
	  set rs = Server.Createobject("ADODB.Recordset")
		rs.ActiveConnection = MM_intellegesSQL_STRING
		rs.Source = "{call sp_getCountry(" & countryId & ")}"
		rs.CursorType = 0
		rs.CursorLocation = 2
		rs.LockType = 1
		rs.Open()
		
		if not rs.eof then
			getCountry = rs("name")
		end if
		
		rs.close()
		set rs = nothing
end function

sub addRegister()

   set Command1 = Server.CreateObject("ADODB.Command")
     Command1.ActiveConnection = MM_intellegesSQL_STRING
     Command1.CommandText = "sp_addRegister"
     Command1.CommandType = 4
     Command1.CommandTimeout = 0
     Command1.Prepared = true
     Command1.Parameters.Append Command1.CreateParameter("@webinar", 3, 1,4,webinarId)
     Command1.Parameters.Append Command1.CreateParameter("@name", 200, 1,250,name)
     Command1.Parameters.Append Command1.CreateParameter("@address", 200, 1,250,address)
     Command1.Parameters.Append Command1.CreateParameter("@address2", 200, 1,250,address2)
     Command1.Parameters.Append Command1.CreateParameter("@city", 200, 1,250,city)
     Command1.Parameters.Append Command1.CreateParameter("@state", 3, 1,4,stateId)
     Command1.Parameters.Append Command1.CreateParameter("@postalCode", 200, 1,50,postalCode)
     Command1.Parameters.Append Command1.CreateParameter("@country", 3, 1,4,countryId)
     Command1.Parameters.Append Command1.CreateParameter("@firstName", 200, 1,50,firstName)
     Command1.Parameters.Append Command1.CreateParameter("@lastName", 200, 1,50,lastName)
     Command1.Parameters.Append Command1.CreateParameter("@title", 200, 1,50,title)
     Command1.Parameters.Append Command1.CreateParameter("@phone", 200, 1,50,phone)
     Command1.Parameters.Append Command1.CreateParameter("@fax", 200, 1,50,fax)
     Command1.Parameters.Append Command1.CreateParameter("@email", 200, 1,50,emailAddress)
     Command1.Parameters.Append Command1.CreateParameter("@subject", 200, 1,500,subject)
     Command1.Parameters.Append Command1.CreateParameter("@note", 200, 1,4000,note)
		 Command1.Execute()
  	 set command1 = nothing

end sub
%>
<html>
<body>
<table width="100%" style="padding-top: 25px">
  <tr>
    <Td align="center">
<%
set mailman = Server.CreateObject("ChilkatMail2.ChilkatMailMan2") 
	mailman.UnlockComponent "SIntellegMAILQ_3rpREpgU6gD7"
	mailman.SmtpHost = "smtp.gmail.com"
	mailman.SmtpSsl = 1
	mailman.SmtpPort = 465
	mailman.SmtpUsername = "john@intelleges.com"
	mailman.SmtpPassword = "012463"

	set email = Server.CreateObject("ChilkatMail2.ChilkatEmail2")
	email.AddTo "John" & " " & "Betancourt", "john@intelleges.com"
	email.FromName = "Intelleges Register Now"
	email.FromAddress = "john@intelleges.com"
	email.Subject = subject
	email.Body = email.AddHtmlAlternativeBody(body)
	if (mailman.SendEmail(email) ) then
		addRegister()
		response.write "Thank you"
	else
		response.write "Thank you"
	end if

	Set email = Nothing
	Set mailman = Nothing

%>
    </td>
  </tr>
</table>
</body>
</html>

