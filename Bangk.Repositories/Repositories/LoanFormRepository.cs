using Bangk.Common.Dto;
using Bangk.Repositories.Context;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Repositories.Repositories
{
    public class LoanFormRepository: GenericRepository<LoanForm, LoanFormDto>, ILoanFormRepository
    {
        public LoanFormRepository(AppDbContext context) : base(context)
        {

        }

        public async Task<List<LoanFormDto>> GetLoanFormById(int loanApplicationId)
        {
            return await Context.LoanForms.Where(l => l.LoanApplicationId == loanApplicationId).Select(l => new LoanFormDto
            {
                Id = l.Id,
                FormName = l.FormName,
                Concept = l.Concept,
                Comment = l.Comment

            }).OrderBy(x => x.Id).ToListAsync();

        }
    }
}
