using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using HumidityMonitorModels;
using NodaTime;
using AutoMapper;

namespace HumidityMonitorApi.Controllers
{
    [Route("api/[controller]")]
    public class HumidityController : Controller
    {
        private readonly CloudTableClient _client;
        private readonly CloudTable _table;
        private readonly string _tableName;

        public HumidityController()
        {
            var storageAccount = CloudStorageAccount.Parse("TODO - Cloud table connection string");
            _client = storageAccount.CreateCloudTableClient();
            _tableName = "HumidityLog";
            _table = _client.GetTableReference(_tableName);
        }

        [HttpGet]
        public async Task<IActionResult> Get(string location, string ianaTimeZone)
        {
            var operation = new TableQuery<HumidityEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, location))
                .Take(1);
            var humidity = (await _table.ExecuteQuerySegmentedAsync(operation, null)).FirstOrDefault();
            if (humidity != null)
            {
                var dateTimeZone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(ianaTimeZone);

                if (dateTimeZone == null)
                {
                    return BadRequest($"Unknown IANA time zone: {ianaTimeZone}");
                }

                return Ok(Mapper.Map<HumidityViewModel>(humidity, opts =>
                {
                    opts.Items["DateTimeZone"] = dateTimeZone;
                }));
            }
            else
            {
                return NotFound();
            }
        }


        [HttpGet]
        [Route("history")]
        public async Task<IActionResult> History(string location, string ianaTimeZone, DateTime date)
        {
            var dateTimeZone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(ianaTimeZone);

            if (dateTimeZone == null)
            {
                return BadRequest($"Unknown IANA time zone: {ianaTimeZone}");
            }

            var zonedDateTime = LocalDateTime.FromDateTime(date).InZoneLeniently(dateTimeZone);
            var result = await GetHumidityEntitiesForDate(location, zonedDateTime);

            return Ok(Mapper.Map<List<HumidityViewModel>>(result, opts =>
            {
                opts.Items["DateTimeZone"] = dateTimeZone;
            }));
        }

        [HttpGet]
        [Route("summary")]
        public async Task<IActionResult> Summary(string location, DateTime startDate, DateTime endDate, string ianaTimeZone)
        {
            var dateTimeZone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(ianaTimeZone);

            if (dateTimeZone == null)
            {
                return BadRequest($"Unknown IANA time zone: {ianaTimeZone}");
            }

            var localStartDate = new LocalDateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0);
            var localEndDate = new LocalDateTime(endDate.Year, endDate.Month, endDate.Day, 0, 0);

            if (localStartDate > localEndDate)
            {
                return BadRequest("Given startDate must be less than or equal to given endDate");
            }

            var result = new List<HumiditySummaryEntity>();

            for (var requestStartDate = localStartDate; requestStartDate <= localEndDate; requestStartDate = requestStartDate.PlusDays(1))
            {
                var humidityRecords = await GetHumidityEntitiesForDate(location, requestStartDate.InZoneLeniently(dateTimeZone));

                var humiditySummaryEntity = new HumiditySummaryEntity(location, requestStartDate, humidityRecords);
                result.Add(humiditySummaryEntity);
            }

            return base.Ok(Mapper.Map<List<HumiditySummaryViewModel>>(result, opts =>
            {
                opts.Items["DateTimeZone"] = dateTimeZone;
            }));
        }

        public async Task<List<HumidityEntity>> GetHumidityEntitiesForDate(string location, ZonedDateTime zonedDateTime)
        {
            var dateTimeZone = zonedDateTime.Zone;
            var requestStartDateTime = zonedDateTime.ToDateTimeUtc();
            var requestEndDateTime = zonedDateTime.PlusHours(24).ToDateTimeUtc();

            var requestStartReverseTick = requestStartDateTime.ToReverseTicks();
            var requestEndReverseTick = requestEndDateTime.ToReverseTicks();

            //The nature of ReverseTicks is that the start time is greater than the end time.
            var operation = new TableQuery<HumidityEntity>()
               .Where(TableQuery.CombineFilters(
                   TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, location),
                   TableOperators.And,
                   TableQuery.CombineFilters(
                       TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.LessThanOrEqual, requestStartReverseTick),
                       TableOperators.And,
                       TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThan, requestEndReverseTick))));

            return (await _table.ExecuteQuerySegmentedAsync(operation, null)).ToList();
        }

    }
}
