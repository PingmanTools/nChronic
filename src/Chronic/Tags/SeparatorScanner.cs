using Chronic.Tags;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Chronic
{
    public class SeparatorScanner : ITokenScanner
    {

        static readonly RegexTagEntry<Separator>[] Patterns = new RegexTagEntry<Separator>[]
            {
                new RegexTagEntry<Separator> { Pattern = @"^,$".Compile(), Tag = new SeparatorComma() },
                new RegexTagEntry<Separator> { Pattern = @"^and$".Compile(), Tag = new SeparatorComma() },
                new RegexTagEntry<Separator> { Pattern = @"^(at|@)$".Compile(), Tag = new SeparatorAt() },
                new RegexTagEntry<Separator> { Pattern = @"^in$".Compile(), Tag = new SeparatorIn() },
                new RegexTagEntry<Separator> { Pattern = @"^/$".Compile(), Tag = new SeparatorDate(Separator.Type.Slash) },
                new RegexTagEntry<Separator> { Pattern = @"^-$".Compile(), Tag = new SeparatorDate(Separator.Type.Dash) },
                new RegexTagEntry<Separator> { Pattern = @"^on$".Compile(), Tag = new SeparatorOn() },
            };

        public IList<Token> Scan(IList<Token> tokens, Options options)
        {
            tokens.ForEach(ApplyTags);
            return tokens;
        }

        static void ApplyTags(Token token)
        {
            foreach (var pattern in Patterns)
            {
                if (pattern.Pattern.IsMatch(token.Value))
                {
                    token.Tag(pattern.Tag);
                }
            }
        }
    }
}