namespace HRShared.Helpers
{
    public static class DateHelper
    {


        public static DateTime ConvertToLocalTime(string datetimestring)
        {
            DateTime timeUtc = DateTime.Parse(datetimestring);
            var dt = DateTime.SpecifyKind(timeUtc, DateTimeKind.Utc);
            TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("W. Central Africa Standard Time");
            DateTime cstTime = TimeZoneInfo.ConvertTimeFromUtc(dt, cstZone);
            return cstTime;
        }


    }
}