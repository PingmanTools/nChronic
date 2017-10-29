using Chronic.Tags;
using System.Collections.Generic;

namespace Chronic
{
    public class GrabberScanner : ITokenScanner
    {

        static readonly TagEntry<string, Grabber>[] _matches = new TagEntry<string, Grabber>[]
            {
                new TagEntry<string, Grabber> { Pattern = "last", Tag = new Grabber(Grabber.Type.Last) },
                new TagEntry<string, Grabber> { Pattern = "next", Tag = new Grabber(Grabber.Type.Next) },
                new TagEntry<string, Grabber> { Pattern = "this", Tag = new Grabber(Grabber.Type.This) }
            };

        public IList<Token> Scan(IList<Token> tokens, Options options)
        {
            tokens.ForEach(ApplyGrabberTags);
            return tokens;
        }

        static void ApplyGrabberTags(Token token)
        {
            foreach (var match in _matches)
            {
                if (match.Pattern == token.Value)
                {
                    token.Tag(match.Tag);
                }
            }
        }
    }
}