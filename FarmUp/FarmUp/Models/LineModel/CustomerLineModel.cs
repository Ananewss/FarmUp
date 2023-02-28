using System;
using System.Collections.Generic;

namespace FarmUp.Model.LineModel
{
    public class CustomerLineModel
    {
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalPoints { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string SalesType { get; set; }
        public string PrivilegeType { get; set; }
    }

    public class NoticationLineResult
    {
        public string to { get; set; }
        public dynamic messages { get; set; }
    }

    public class LineMessages
    {
        public string type { get; set; }
        public string altText { get; set; }
        public LineContents contents { get; set; }
    }

    public class LineContents
    {
        public string type { get; set; } 
        public string direction { get; set; }
        public LineBodyContents body { get; set; }
        public LineBodyContents footer { get; set; }
    }

    public class LineBody
    {
        public string type { get; set; }
        public string layout { get; set; }
        public string spacing { get; set; }
        public List<LineBodyContents> contents { get; set; }
    }

    public class LineFooter
    {
        public string type { get; set; }
        public string layout { get; set; }
        public List<LineFooterContents> contents { get; set; }
    }

    public class LineBodyContents
    {
        public string type { get; set; }
        public string text { get; set; }
        public string weight { get; set; }
        public string size { get; set; }
        public string color { get; set; }
        public string margin { get; set; }
        public string layout { get; set; }
        public string align { get; set; }
        public bool? wrap { get; set; }
        public int? flex { get; set; }
        public string spacing { get; set; }
        public string style { get; set; }
        public string label { get; set; }
        public string uri { get; set; }
        public string height { get; set; }
        public string url { get; set; }

        public List<LineBodyContents> contents { get; set; }
        public LineBodyContents action { get; set; }
    }
     

    public class Contents
    {
        public string type { get; set; }
        public string text { get; set; }
        public string weight { get; set; }
        public string size { get; set; }
        public string align { get; set; }
        public string margin { get; set; }
        public bool wrap { get; set; }
        public string color { get; set; }
    }

    public class LineFooterContents
    {
        public string type { get; set; }
        public LineAction action { get; set; }
        public string style { get; set; }
        public string height { get; set; }
        public string color { get; set; }
    }

    public class LineAction
    {
        public string type { get; set; }
        public string label { get; set; }
        public string uri { get; set; }
    }
}