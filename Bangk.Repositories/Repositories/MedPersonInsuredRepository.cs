using Bangk.Common.Dto;
using Bangk.Repositories.Context;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Repositories.Repositories
{
    public class MedPersonInsuredRepository : GenericRepository<MedPersonInsureds, MedPersonInsuredsDto>, IMedPersonInsuredRepository
    {
        public MedPersonInsuredRepository(AppDbContext context) : base(context)
        {
        }
    }
}