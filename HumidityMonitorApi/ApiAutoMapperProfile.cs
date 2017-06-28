using AutoMapper;
using HumidityMonitorModels;
using NodaTime;
using System;

namespace HumidityMonitorApi
{
    public class ApiAutoMapperProfile: Profile
    {
        public ApiAutoMapperProfile()
        {
            CreateMap<HumidityEntity, HumidityViewModel>()
                .ForMember(vm => vm.Location, opt => opt.MapFrom(m => m.PartitionKey))
                .ForMember(vm => vm.Humidity, opt => opt.MapFrom(m => m.RelativeHumidity))
                .ForMember(vm => vm.Date, opt => opt.ResolveUsing(ConvertReverseTicksToLocalDateTimeString));

            CreateMap<HumiditySummaryEntity, HumiditySummaryViewModel>()
                .ForMember(vm => vm.Location, opt => opt.MapFrom(m => m.PartitionKey))
                .ForMember(vm => vm.Date, opt => opt.ResolveUsing(GetLocalDateFromContextOrRowKey));
        }

        private object GetLocalDateFromContextOrRowKey(HumiditySummaryEntity source, HumiditySummaryViewModel destination, string destMember, ResolutionContext context)
        {
            context.Items.TryGetValue("LocalDate", out var localDate);

            if (localDate is LocalDate) {
                return ((LocalDate)localDate).ToString("yyyy-MM-dd", null);
            }

            context.Items.TryGetValue("DateTimeZone", out var dateTimeZone);

            if (dateTimeZone is DateTimeZone && !string.IsNullOrWhiteSpace(source.RowKey))
            {
                return ReverseTicks.ToZonedDateTime(source.RowKey, (DateTimeZone)dateTimeZone).ToString("yyyy-MM-dd", null);
            }

            return null;
        }

        private object ConvertReverseTicksToLocalDateTimeString(HumidityEntity humidity, HumidityViewModel destination, string destMember, ResolutionContext context)
        {
            if (string.IsNullOrWhiteSpace(humidity.RowKey))
            {
                return null;
            }

            context.Items.TryGetValue("DateTimeZone", out var ianaTimeZone);

            var dateTimeZone = ianaTimeZone as DateTimeZone;
            
            if (dateTimeZone == null)
            {
                return null;
            }

            return ReverseTicks.ToZonedDateTime(humidity.RowKey, dateTimeZone).ToOffsetDateTime().ToString();
        }

        
    }
}
