namespace TodoApp.Shared;

public record UsageDto(
    long DatabaseBytes,
    long DatabaseLimitBytes,
    long StorageBytes,
    long StorageLimitBytes,
    int StorageObjects,
    int Todos,
    int TodosDone,
    DateTime MeasuredAt);
