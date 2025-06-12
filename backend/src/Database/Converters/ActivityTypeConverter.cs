using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RateMyPet.Core;

namespace RateMyPet.Database.Converters;

public class ActivityTypeConverter() : ValueConverter<ActivityType, string>(
    activityType => ActivityTypeToCode(activityType),
    code => CodeToActivityType(code))
{
    private const string RegisterCode = "REGI";
    private const string ConfirmEmailCode = "CNFE";
    private const string ForgotPasswordCode = "FGPW";
    private const string ResetPasswordCode = "RSPW";
    private const string AddPostCode = "ADDP";
    private const string UpdatePostCode = "UPDP";

    private static string ActivityTypeToCode(ActivityType activityType)
    {
        return activityType switch
        {
            ActivityType.Register => RegisterCode,
            ActivityType.ConfirmEmail => ConfirmEmailCode,
            ActivityType.ForgotPassword => ForgotPasswordCode,
            ActivityType.ResetPassword => ResetPasswordCode,
            ActivityType.AddPost => AddPostCode,
            ActivityType.UpdatePost => UpdatePostCode,
            _ => throw new ArgumentOutOfRangeException(nameof(activityType), activityType, "Unknown activity type")
        };
    }

    private static ActivityType CodeToActivityType(string code)
    {
        return code switch
        {
            RegisterCode => ActivityType.Register,
            ConfirmEmailCode => ActivityType.ConfirmEmail,
            ForgotPasswordCode => ActivityType.ForgotPassword,
            ResetPasswordCode => ActivityType.ResetPassword,
            AddPostCode => ActivityType.AddPost,
            UpdatePostCode => ActivityType.UpdatePost,
            _ => throw new ArgumentOutOfRangeException(nameof(code), code, "Unknown activity type code")
        };
    }
}
