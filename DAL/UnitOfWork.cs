using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.DAL
{
    public class UnitOfWork:IDisposable
    {
        private MMSEntities context = new MMSEntities();
        private GenericRepositiory<tblIndentRequisition> indentRepository;
        private GenericRepositiory<tblProjectMaster> projectRepository;
        private GenericRepositiory<tblItemGroupMaster> itemGroupmasterRepository;
        private GenericRepositiory<BlockMaster> blockMasterRepository;

        // fro indent requisition
        public GenericRepositiory<tblIndentRequisition> tblIndentRepository
        {
            get
            {
                if (this.indentRepository == null)
                {
                    this.indentRepository = new GenericRepositiory<tblIndentRequisition>(context);
                }
                return indentRepository;
            }
        }


        // for PROJECT
        public GenericRepositiory<tblProjectMaster> tblProjectRepository
        {
            get
            {
                if (this.projectRepository == null)
                {
                    this.projectRepository = new GenericRepositiory<tblProjectMaster>(context);
                }
                return projectRepository;
            }
        }
        // for Item group Master

        public GenericRepositiory<tblItemGroupMaster> tblItemgroupmasterRepository
        {
            get
            {
                if (this.itemGroupmasterRepository == null)
                {
                    this.itemGroupmasterRepository = new GenericRepositiory<tblItemGroupMaster>(context);
                }
                return itemGroupmasterRepository;
            }
        }

        // for Block Masters

        public GenericRepositiory<BlockMaster> tblBlockmasterRepository
        {
            get
            {
                if (this.blockMasterRepository == null)
                {
                    this.blockMasterRepository = new GenericRepositiory<BlockMaster>(context);
                }
                return blockMasterRepository;
            }
        }








        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}