using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.Models
{
    public class LnResult
    {
       
       // public string ETag { get; set; }
        public  string Title { get; set; }
      
        public string Snippet { get; set; }
       
       /* public string Mime { get; set; }
      
        public string Link { get; set; }
  
        public string Kind { get; set; }
        
        public string HtmlTitle { get; set; }
        
        public string HtmlSnippet { get; set; }*/
        
        public string HtmlFormattedUrl { get; set; }
        
        /*public string FormattedUrl { get; set; }
        
        public string FileFormat { get; set; }
        
        public string DisplayLink { get; set; }
        
        public string CacheId { get; set; }*/
    }
}