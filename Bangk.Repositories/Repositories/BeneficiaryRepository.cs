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
    public class BeneficiaryRepository: GenericRepository<Beneficiary, BeneficiaryDto>, IBeneficiaryRepository
    {
        public BeneficiaryRepository(AppDbContext context) : base(context)
        {

        }
    }
}
