using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace RateMyPet.Persistence.Converters;

public class NullableDateTimeUtcConverter() : ValueConverter<DateTime?, DateTime?>(
    static datetime => datetime.HasValue ? datetime.Value.ToUniversalTime() : null,
    static datetime => datetime.HasValue ? DateTime.SpecifyKind(datetime.Value, DateTimeKind.Utc) : null);
