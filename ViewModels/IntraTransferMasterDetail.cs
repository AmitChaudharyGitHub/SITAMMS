using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class IntraTransferMasterDetail
    {
        public Nullable<decimal> IntraTransferMasterTransId { get; set; }
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectAddress { get; set; }
        public string ProjectState { get; set; }
        public string ProjectGSTIN { get; set; }
        public string TRansferProjectID { get; set; }
        public DateTime Transferdate { get; set; }
        public string TransferProjectName { get; set; }
        public string TransferProjectAddress { get; set; }
        public string TransferProjectState { get; set; }
        public string TarnsferProjectGSTIN { get; set; }
        public string ModeOfTPT { get; set; }
        public string VehicleType { get; set; }
        public string VehicleNo { get; set; }
        public string EWayBillNo { get; set; }
        public string TaxTapableStatus { get; set; }

        public string TRansferNo { get; set; }


        public Nullable<decimal> Total { get; set; }
        public Nullable<decimal> SubTotal1 { get; set; }
        public Nullable<decimal> SubTotal2 { get; set; }
        public Nullable<decimal> GrandTotal { get; set; }
        public Nullable<decimal> Total_PAndF { get; set; }
        public Nullable<decimal> Total_Cartage { get; set; }
        public Nullable<decimal> Total_Insurance { get; set; }
        public Nullable<decimal> Total_Taxable { get; set; }
        public Nullable<decimal> Total_CGST { get; set; }
        public Nullable<decimal> Total_SGSTAndUTGST { get; set; }
        public Nullable<decimal> Total_IGST { get; set; }
        public string Total_NetAmountInWords { get; set; }

        public string ReachingDateofDestination { get; set; }
        public Nullable<decimal> IntarStateTaxableMasterTransID { get; set; }

        public string GateOutDate { get; set; }
        public string GateOutNo { get; set; }
        public DateTime? E_WayBill_Date { get; set; }


    }
}