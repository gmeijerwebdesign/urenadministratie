//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Premium_Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class tbl_Instellingen_Periodes
    {
        public int VOLGNUMMER { get; set; }
        public Nullable<System.DateTime> DATUM { get; set; }
        public string DAG { get; set; }
        public Nullable<bool> WERKDAG { get; set; }
        public Nullable<bool> FEESTDAG { get; set; }
        public string TYPE { get; set; }
        public Nullable<int> WEEK { get; set; }
        public string OMSCHRIJVING { get; set; }
        public string WEEKNUMMER { get; set; }
        public string WEEK_EVEN_ONEVEN { get; set; }
        public string JAAR { get; set; }
        public string PERIODE_WEEK { get; set; }
        public string PERIODE_MAAND { get; set; }
        public string PERIODE_KWARTAAL { get; set; }
        public string PROVISIE_PERIODE { get; set; }
        public string OPMERKING { get; set; }
        public byte[] SSMA_TimeStamp { get; set; }
    }
}
