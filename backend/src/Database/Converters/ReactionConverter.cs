using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RateMyPet.Core;

namespace RateMyPet.Database.Converters;

public class ReactionConverter() : ValueConverter<Reaction, char>(
    reaction => ReactionToCharMap[reaction],
    c => CharToReactionMap[c])
{
    private static readonly Dictionary<Reaction, char> ReactionToCharMap = new()
    {
        { Reaction.Like, 'L' },
        { Reaction.Funny, 'F' },
        { Reaction.Crazy, 'C' },
        { Reaction.Wow, 'W' },
        { Reaction.Sad, 'S' }
    };

    private static readonly Dictionary<char, Reaction> CharToReactionMap = ReactionToCharMap
        .ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
}
