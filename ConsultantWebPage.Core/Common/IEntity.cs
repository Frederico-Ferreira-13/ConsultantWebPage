using System;

namespace ConsultantWebPage.Core.Common
{
    public interface IEntity    
    {
        int GetId();
        void SetId(int id);
        bool GetIsActive();
    }
}

