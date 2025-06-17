using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RateMyPet.Core;

namespace RateMyPet.Database.Converters;

public class UserActivityCategoryConverter() : ValueConverter<UserActivityCategory, string>(
    category => CategoryToCodeMap[category],
    code => CodeToCategoryMap[code])
{
    private static readonly Dictionary<UserActivityCategory, string> CategoryToCodeMap = new()
    {
        { UserActivityCategory.Register, "REGI" },
        { UserActivityCategory.ConfirmEmail, "CNFE" },
        { UserActivityCategory.ForgotPassword, "FGPW" },
        { UserActivityCategory.ResetPassword, "RSPW" },
        { UserActivityCategory.AddPost, "ADDP" },
        { UserActivityCategory.UpdatePost, "UPDP" },
        { UserActivityCategory.DeletePost, "DELP" },
        { UserActivityCategory.AddComment, "ADDC" },
        { UserActivityCategory.DeleteComment, "DELC" }
    };

    private static readonly Dictionary<string, UserActivityCategory> CodeToCategoryMap = CategoryToCodeMap
        .ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
}
