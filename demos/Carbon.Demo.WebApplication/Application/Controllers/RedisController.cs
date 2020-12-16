using Carbon.Redis;
using Carbon.WebApplication;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Carbon.Demo.WebApplication.Application.Dtos.Redis;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace Carbon.Demo.WebApplication.Application.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RedisController : CarbonController
    {
        private readonly IDatabase _redis;
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly ILogger<RedisController> _logger;
        public RedisController(IDatabase redis, IConnectionMultiplexer connectionMultiplexer, ILogger<RedisController> logger)
        {
            _redis = redis;
            _connectionMultiplexer = connectionMultiplexer;
            _logger = logger;
        }


        [HttpGet]
        [Route("Cache")]
        public async Task Cache()
        {
            #region Initializations
            var headQuarters = new LocationDto()
            {
                Address = "İstanbul"
            };
            var branch = new LocationDto()
            {
                Address = "Ankara"
            };
            var company = new CompanyDto()
            {
                Age = 75,
                Name = "KoçDigital",
                Location = new List<LocationDto>() { headQuarters, branch }
            };
            var company2 = new CompanyDto()
            {
                Age = 75,
                Name = "KoçSistem",
                Location = new List<LocationDto>() { headQuarters, branch }
            }; 
            #endregion

            #region Set Usage (Added two companies to the cache seperately)
            var (setComplexIsSuccess, setComplexError) = await _redis.Set(string.Format(CacheKey.CompanyById, company.Id), company);
            var (setComplexIsSuccess1, setComplexError1) = await _redis.Set(string.Format(CacheKey.CompanyById, company2.Id), company2);
            #endregion

            #region Set With Logging Usage (Added two companies to the cache with logging feature seperately)
            var cacheKey = string.Format(CacheKey.CompanyByLocation, company.Id, headQuarters.Id);
            var cacheObject = new CacheObject(cacheKey, Guid.Empty, _redis, _logger);
            await cacheObject.SetWithLogging(company);

            cacheKey = string.Format(CacheKey.CompanyByLocation, company2.Id, headQuarters.Id);
            cacheObject = new CacheObject(cacheKey, Guid.Empty, _redis, _logger);
            await cacheObject.SetWithLogging(company2);
            #endregion

            #region Get Usage (Get company1)
            var (companyData, companyErrorMessage) = await _redis.Get<CompanyDto>(string.Format(CacheKey.CompanyById, company.Id));
            #endregion

            #region GetWithLogging Usage (Get company1 with logging feature)
            cacheKey = string.Format(CacheKey.CompanyById, company.Id);
            cacheObject = new CacheObject(cacheKey, Guid.Empty, _redis, _logger);
            (companyData, companyErrorMessage) = await cacheObject.GetWithLogging<CompanyDto>();
            #endregion

            #region GetByPattern Usage (Get companies in given locations)
            var cacheKeyPattern = string.Format(CacheKey.CompanyByLocation, company.Id, "*");
            var (companyDataList, getByPatternErrorMessage) = await _redis.GetByPattern<CompanyDto>(cacheKeyPattern, _connectionMultiplexer);
            #endregion

            #region GetByPatternWithLogging Usage (Get companies in given locations with logging feature)
            cacheKeyPattern = string.Format(CacheKey.CompanyByLocation, company.Id, "*");
            cacheObject = new CacheObject(cacheKeyPattern, Guid.Empty, _redis, _logger);
            (companyDataList, getByPatternErrorMessage) = await cacheObject.GetByPatternWithLogging<CompanyDto>(_connectionMultiplexer);
            #endregion

            #region IsCacheKeyValid Usage (Check the key is valid or not)
            var (isCacheKeyValidResult, isCacheKeyValidErrorMessage) = await _redis.IsCacheKeyValid(string.Format(CacheKey.CompanyById, company2.Id));
            #endregion

            #region RemoveKey Usage (Remove single key from the cache)
            var (isDeleted, errorRemove) = await _redis.RemoveKey(string.Format(CacheKey.CompanyById, company.Id));
            #endregion
            
            #region RemoveKeyWithLogging Usage (Remove single key with logging feature)
            cacheKeyPattern = string.Format(CacheKey.CompanyById, company2.Id, "*");   
            cacheObject = new CacheObject(cacheKeyPattern, Guid.Empty, _redis, _logger);
            (isDeleted, errorRemove) =  await cacheObject.RemoveKeyWithLogging(); 
            #endregion

            #region RemoveKeysByPattern Usage (Remove multiple keys by pattern)
            var (removedList, couldNotBeRemoved, errorMessage) = await _redis.RemoveKeysByPattern(string.Format(CacheKey.CompanyByLocation, company.Id, "*"), _connectionMultiplexer);
            #endregion

            #region RemoveKeysByPatternWithLogging Usage (Remove multiple keys by pattern with logging feature)
            cacheKeyPattern = string.Format(CacheKey.CompanyByLocation, company2.Id, "*");   
            cacheObject = new CacheObject(cacheKeyPattern, Guid.Empty, _redis, _logger);
            (isDeleted, errorRemove) = await cacheObject.RemoveKeysByPatternWithLogging(_connectionMultiplexer); 
            #endregion
        }
       
    }
}