using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Chronic.Tags
{
    class TagEntry<TPattern, TTag>
    {
        public TPattern Pattern;
        public TTag Tag;
    }

    class RegexTagEntry<TTag> : TagEntry<Regex, TTag> { }
}
