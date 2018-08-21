using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CryptoTracker.MarketData.DataAccess.Repositories;
using CryptoTracker.MarketData.DomainModel;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CryptoTracker.MarketData.Controllers
{
    [Route("api/v1/currencies")]
    public class CurrenciesController : Controller
    {
        private readonly CurrencyRepository _currencyRepository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CurrenciesController(CurrencyRepository currencyRepository, IMapper mapper, ILogger<CurrenciesController> logger)
        {
            _currencyRepository = currencyRepository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken = default)
        {
            Uri uri = Request.GetUri();
            var host = HttpContext.Connection.RemoteIpAddress;
            _logger.LogInformation("URI is {uri} and remote ip is {host}", uri, host);
            

            IReadOnlyCollection<Currency> allCurrencies = await _currencyRepository.GetAll(cancellationToken);
            IEnumerable<ViewModel.v1.Currency> viewModels = _mapper.Map<IEnumerable<ViewModel.v1.Currency>>(allCurrencies);
            return Ok(viewModels);
        }
    }
}