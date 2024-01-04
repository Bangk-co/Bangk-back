using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Bangk.Common.Dto;
using Bangk.Domain.Contract;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bangk.Repositories.Repositories;

namespace Bangk.Api.Controllers
{
    public class LogController : BaseController
    {
        private readonly ILogRepository _logDomain;
        private readonly IMapper _mapper;

        public LogController(ILogRepository logDomain, IMapper mapper)
        {
            _logDomain = logDomain;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<List<LogResponseDto>>> Post(LogRequestDto requestDto)
        {
            var logResult = await _logDomain.GetAll(x=> x.TimeStamp.Date >= requestDto.FromDate && x.TimeStamp.Date <= requestDto.ToDate);
            return Ok(_mapper.Map<List<LogResponseDto>>(logResult.Take(requestDto.RegisterCount)));
        }
    }
}
