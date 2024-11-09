using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RateMyPet.Persistence.Models;

namespace RateMyPet.Persistence.Converters;

public class ReactionConverter() : ValueConverter<Reaction, char>(
    reaction => ReactionToChar(reaction),
    c => CharToReaction(c))
{
    private const char LikeChar = 'L';
    private const char FunnyChar = 'F';
    private const char CrazyChar = 'C';
    private const char WowChar = 'W';
    private const char SadChar = 'S';

    private static char ReactionToChar(Reaction reaction) => reaction switch
    {
        Reaction.Like => LikeChar,
        Reaction.Funny => FunnyChar,
        Reaction.Crazy => CrazyChar,
        Reaction.Wow => WowChar,
        Reaction.Sad => SadChar,
        _ => throw new ArgumentOutOfRangeException(nameof(reaction), reaction, "Unknown reaction")
    };

    private static Reaction CharToReaction(char c) => c switch
    {
        LikeChar => Reaction.Like,
        FunnyChar => Reaction.Funny,
        CrazyChar => Reaction.Crazy,
        WowChar => Reaction.Wow,
        SadChar => Reaction.Sad,
        _ => throw new ArgumentOutOfRangeException(nameof(c), c, "Unknown reaction character")
    };
}