namespace RateMyPet.Core;

public interface ISoftDeletable
{
    DateTime? DeletedAtUtc { get; set; }
    bool IsDeleted => DeletedAtUtc is not null;
}
