﻿using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace RateMyPet.Database.Converters;

public class DateTimeUtcConverter() : ValueConverter<DateTime, DateTime>(
    static datetime => datetime.ToUniversalTime(),
    static datetime => DateTime.SpecifyKind(datetime, DateTimeKind.Utc));
