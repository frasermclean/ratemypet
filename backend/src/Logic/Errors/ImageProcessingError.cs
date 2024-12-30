using FluentResults;
using SixLabors.ImageSharp;

namespace RateMyPet.Logic.Errors;

public class ImageProcessingError(Exception exception) : Error(exception switch
{
    NotSupportedException => "Image format is not supported",
    InvalidImageContentException => "Invalid image content",
    UnknownImageFormatException => "Unknown image format",
    _ => "Error reading image"
})
{
    public static ImageProcessingError FromException(Exception exception) => new(exception);
};
