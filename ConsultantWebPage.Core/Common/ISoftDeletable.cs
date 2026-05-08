using System;

namespace ConsultantWebPage.Core.Common
{
    public interface ISoftDeletable
    {
        bool IsActive { get; }
        void Deactivate();
    } 
}


