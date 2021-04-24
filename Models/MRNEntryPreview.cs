using MMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.Models
{
    public class MRNEntryPreview
    {
        public GetDetailOnNewDMR MRNDetail { get; set; }
        public List<GetGridDMR> RecItems { get; set; }
        public string EwayBillNo { get; set; }
        public string DMRDate { get; set; }
        public decimal GrandTotal { get; set; }

        public decimal GetTotalDiscount { get {
                decimal totalDisc = 0;
                if (RecItems != null) {
                    foreach (GetGridDMR item in RecItems)
                    {
                        totalDisc += item.Discount.Value;
                    }
                }
                return totalDisc;
            }
        }

        public decimal GetTotalPF
        {
            get
            {
                decimal totalPF = 0;
                if (RecItems != null)
                {
                    foreach (GetGridDMR item in RecItems)
                    {
                        totalPF += item.PandF == null ? 0 : item.PandF.Value;
                    }
                }
                return totalPF;
            }
        }

        public decimal GetTotalInsurance
        {
            get
            {
                decimal totalIns = 0;
                if (RecItems != null)
                {
                    foreach (GetGridDMR item in RecItems)
                    {
                        totalIns += item.InsuranceAmt == null ? 0 : item.InsuranceAmt.Value;
                    }
                }
                return totalIns;
            }
        }

        public decimal GetTotalGrand
        {
            get
            {
                decimal totalGT = 0;
                if (RecItems != null)
                {
                    foreach (GetGridDMR item in RecItems)
                    {
                        totalGT += item.GrandTotal.Value;
                    }
                }
                return totalGT;
            }
        }

        public decimal GetTotalCartage { get {
                decimal totalCart = 0;
                if (RecItems != null)
                {
                    foreach (GetGridDMR item in RecItems)
                    {
                        totalCart += item.CartageAmt == null ? 0 : item.CartageAmt.Value;
                    }
                }
                return totalCart;

            }
        }

        public decimal GetTotalCgst
        {
            get
            {
                decimal totalCgst = 0;
                if (RecItems != null)
                {
                    foreach (GetGridDMR item in RecItems)
                    {
                        totalCgst += item.CGSTAmt.Value == null ? 0 : item.CGSTAmt.Value;
                    }
                }
                return totalCgst;

            }
        }

        public decimal GetTotalSgst
        {
            get
            {
                decimal totalSgst = 0;
                if (RecItems != null)
                {
                    foreach (GetGridDMR item in RecItems)
                    {
                        totalSgst += item.SGSTAndUTGSTAmt.Value == null ? 0 : item.SGSTAndUTGSTAmt.Value;
                    }
                }
                return totalSgst;

            }
        }

        public decimal GetTotalIgst
        {
            get
            {
                decimal totalIgst = 0;
                if (RecItems != null)
                {
                    foreach (GetGridDMR item in RecItems)
                    {
                        totalIgst += item.IGSTAmt.Value == null ? 0 : item.IGSTAmt.Value;
                    }
                }
                return totalIgst;

            }
        }

        public decimal GetTotalTCS
        {
            get
            {
                decimal totalTCS = 0;
                if (RecItems != null)
                {
                    foreach (GetGridDMR item in RecItems)
                    {
                        totalTCS += item.TCSAmt.Value == null ? 0 : item.TCSAmt.Value;
                    }
                }
                return totalTCS;
            }
        }

    }
}