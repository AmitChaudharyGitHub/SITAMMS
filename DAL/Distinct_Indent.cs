using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.DAL
{
    public partial class ns_tbl_IssueQuantity : IEquatable<ns_tbl_IssueQuantity>
    {
        public bool Equals(ns_tbl_IssueQuantity other)
        {
            if (IndentNo == other.IndentNo)
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            int hashFirstName = IndentNo == null ? 0 : IndentNo.GetHashCode();


            return hashFirstName;
        }
    }
}