using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.DAL
{
    public partial class tblIndentRequisition : IEquatable<tblIndentRequisition>
    {
      

        public bool Equals(tblIndentRequisition other)
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

    public partial class GateEntryChild : IEquatable<GateEntryChild>
    {


        public bool Equals(GateEntryChild other)
        {
            if (Vendor == other.Vendor)
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            int hashFirstName = Vendor == null ? 0 : Vendor.GetHashCode();


            return hashFirstName;
        }
    }

    public partial class tblVendorMaster : IEquatable<tblVendorMaster>
    {
        public bool Equals(tblVendorMaster other)
        {
            if (MulRegistrationType == other.MulRegistrationType)
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            int hashFirstName = MulRegistrationType == null ? 0 : MulRegistrationType.GetHashCode();


            return hashFirstName;
        }
    }
}