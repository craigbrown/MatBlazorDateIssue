using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace MatBlazorDateIssue
{
    public sealed class TimeZoneService
    {
        private readonly IJSRuntime _jsRuntime;

        private TimeSpan? _userOffset;

        public TimeZoneService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async ValueTask<DateTimeOffset> GetLocalDateTime(DateTimeOffset dateTime)
        {
            var userOffset = await GetTimezoneOffset();
            return dateTime.ToOffset(userOffset);
        }

        public async ValueTask<DateTime> GetLocalDateTime()
        {
            var userOffset = await GetTimezoneOffset();
            return DateTime.SpecifyKind(DateTime.UtcNow.Add(userOffset), DateTimeKind.Local);
        }

        /// <summary>
        /// Returns the time difference in minutes between this user's timezone and UTC, obtained via Javascript.
        /// </summary>
        private async ValueTask<TimeSpan> GetTimezoneOffset()
        {
            if (_userOffset == null)
            {
                var offsetInMinutes = await _jsRuntime.InvokeAsync<int>("blazorGetTimezoneOffset");
                _userOffset = TimeSpan.FromMinutes(-offsetInMinutes);
            }
            return _userOffset.Value;
        }
    }
}
