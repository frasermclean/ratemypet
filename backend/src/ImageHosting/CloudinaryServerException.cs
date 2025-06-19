using CloudinaryDotNet.Actions;

namespace RateMyPet.ImageHosting;

public class CloudinaryServerException(Error error) : Exception($"Cloudinary server error: {error.Message}");
