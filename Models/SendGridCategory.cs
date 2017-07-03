using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.Models
{
	public enum SendGridCategory
	{
		SendEmailAlert = 1,
		QuestionnaireResponse = 2,
		SendInventation = 3,
		Finish = 4,
		ValdatezCodeFn = 5,
		CreaeEstimaionQuestionnaie = 6,
		InvitePartnes = 7,
		Invite = 9,
		RemovePartner = 11,
		FindRemind = 12,
		SendEmailByAccessCode = 13,
		ChangeRailsStatus = 14,
		SendIteratePartnerEmailTest = 15,
		SendIteratePartnerEmail = 16,
		Create = 17,
		CreatePerson = 18,
		EmailSend = 19,
		QuestionnaireQuestionnaireTestAutomailAll = 20,
		WrongContract = 21,
		SendPassword = 22,
		SendPasswordChangedNotification = 23,
		ReminderList = 24,
		CompleteConfirmation = 25,
		Incomplete = 26,
		SendFirstReminderByPptq = 27
	}

	public enum Reminders
	{
		Automaed = 1,
		InviteRemind = 2,
		PartnerFind = 3,
		IerateEmail = 4
	}
}