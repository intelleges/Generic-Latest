using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Generic.Models
{
    public class AMLMMModel
    {
        
        
        [Required]
        [DisplayName("Fl Reviewer")]
        public int Owner { get; set; }

        [DisplayName("Technology")]
        [Required]
        public int group { get; set; }


        [DisplayName("Monitoring")]
        [Required]
        public int protocol { get; set; }


        [DisplayName("Product")]
        [Required]
        public int touchpoint { get; set; }


        [DisplayName("Scenario")]
        [Required]
        public int state { get; set; }

        [DisplayName("Focus")]
        [Required]
        public int partnertype { get; set; }

        [DisplayName("Geography")]
        [Required]
        public int country { get; set; }

        

        [DisplayName("Financial Institution Upload")]
        // [Required]
        public HttpPostedFileBase FileFinancialInstitution { get; set; }

        [DisplayName("Person/Entity Upload")]
        // [Required]
        public HttpPostedFileBase FilePersonEntity { get; set; }

        [DisplayName("Focal Relationship Upload")]
        // [Required]
        public HttpPostedFileBase FileFocalRelationship { get; set; }

        [DisplayName("Transaction History Upload")]
        // [Required]
        public HttpPostedFileBase FileTransactionHistory { get; set; }

       
        [Required]
        [DisplayName("Due date")]
        public DateTime? Duedate { get; set; }

        public string Comments { get; set; }

        

        [Required]
        [DisplayName("Customer-Entity Reviewer")]
        public int CustomerEntityReviewer { get; set; }
        [Required]
        [DisplayName("Due date")]
        public DateTime? CustomerEntityReviewerDuedate { get; set; }


        [Required]
        [DisplayName("Customer-Entity Relationship Reviewer")]
        public int CustomerEntityRelationshipReviewer { get; set; }
        [Required]
        [DisplayName("Due date")]
        public DateTime? CustomerEntityRelationshipReviewerDuedate { get; set; }



        [Required]
        [DisplayName("Transaction Reviewer")]
        public int TransactionReviewer { get; set; }
        [Required]
        [DisplayName("Due date")]
        public DateTime? TransactionReviewerDuedate { get; set; }

    }
}