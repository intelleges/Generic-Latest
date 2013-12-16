<%

name = request.form("name")
address = request.form("address")
address2 = request.form("address2")
city = request.form("city")
state = request.form("state")
postalCode = request.form("postalcode")
country = request.form("country")
firstName = request.form("firstName")
lastName = request.form("lastName")
title = request.form("title")
phone = request.form("phone")
fax = request.form("fax")
email = request.form("email")
subject = request.form("subject")
note = request.form("note")

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
body = body & "Email: " & email & "<br />"
body = body & "Subject: " & subject & "<br />"
body = body & "Note: " & note & "<br />"
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
	email.AddTo "Charles" & " " & "Panoff", "charles.panoff@intelleges.com"
	email.FromName = "Intelleges Contact Us"
	email.FromAddress = "john@intelleges.com"
	email.AddCC "John Betancourt","john@intelleges.com"
	email.Subject = subject
	email.Body = email.AddHtmlAlternativeBody(body)
	if (mailman.SendEmail(email) ) then
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

