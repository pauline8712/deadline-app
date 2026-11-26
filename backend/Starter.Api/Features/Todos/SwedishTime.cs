namespace Starter.Api.Features.Todos;

/// <summary>All datumlogik ska gälla svensk tid (IK-3), oavsett vilken tidszon servern står i.</summary>
public static class SwedishTime
{
    private static readonly TimeZoneInfo Stockholm = TimeZoneInfo.FindSystemTimeZoneById("Europe/Stockholm");

    public static DateOnly Today(TimeProvider clock) =>
        DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(clock.GetUtcNow(), Stockholm).DateTime);
}
