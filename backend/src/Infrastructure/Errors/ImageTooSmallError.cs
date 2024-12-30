using FluentResults;

namespace RateMyPet.Infrastructure.Errors;

public class ImageTooSmallError(int width, int height)
    : Error($"Image dimensions are too small, dimensions: {width}x{height}");
