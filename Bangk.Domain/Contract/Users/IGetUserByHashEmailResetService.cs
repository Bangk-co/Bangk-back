using Bangk.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Domain.Contract.Users
{
    public interface IGetUserByHashEmailResetService 
    {
        User Execute(string hash);
    }
}
