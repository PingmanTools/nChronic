using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Chronic.Tags.Repeaters;

namespace Chronic.Tags.Repeaters
{
    public class RepeaterScanner : ITokenScanner
    {
        static readonly List<Func<Token, Options, ITag>> _scanners = new List
            <Func<Token, Options, ITag>>
            {
                //ScanSeasonNames(token, options),
                ScanMonthNames,
                ScanDayNames,
                ScanDayPortions,
                ScanTimes,
                ScanUnits,
            };

        public IList<Token> Scan(IList<Token> tokens, Options options)
        {
            tokens.ForEach(token =>
                {
                    foreach (var scanner in _scanners)
                    {
                        var tag = scanner(token, options);
                        if (tag != null)
                        {
                            token.Tag(tag);
                            break;
                        }
                    }
                });

            return tokens;
        }

        static ITag ScanUnits(Token token, Options options)
        {
            ITag tag = null;
            UnitPatterns.ForEach(item =>
            {
                if (item.Pattern.IsMatch(token.Value))
                {
                    var type = (Type)item.Tag;
                    var hasCtorWithOptions = type.GetConstructors().Any(ctor =>
                    {
                        var parameters = ctor.GetParameters().ToArray();
                        return
                            parameters.Length == 1
                            && parameters.First().ParameterType == typeof(Options);
                    });
                    var ctorParameters = hasCtorWithOptions
                        ? new[] { options }
                        : new object[0];

                    tag = Activator.CreateInstance(
                        type,
                        ctorParameters) as ITag;

                    return;
                }
            });
            return tag;
        }

        static ITag ScanTimes(Token token, Options options)
        {
            var match = _timePattern.Match(token.Value);
            if (match.Success)
            {
                return new RepeaterTime(match.Value);
            }
            return null;
        }

        static ITag ScanDayPortions(Token token, Options options)
        {
            ITag tag = null;
            DayPortionPatterns.ForEach(item =>
                {
                    if (item.Pattern.IsMatch(token.Value))
                    {
                        tag = new EnumRepeaterDayPortion(item.Tag);
                        return;
                    }
                });
            return tag;
        }

        static ITag ScanDayNames(Token token, Options options)
        {
            ITag tag = null;
            DayPatterns.ForEach(item =>
                {
                    if (item.Pattern.IsMatch(token.Value))
                    {
                        tag = new RepeaterDayName(item.Tag);
                        return;
                    }
                });
            return tag;
        }

        static ITag ScanMonthNames(Token token, Options options)
        {
            ITag tag = null;
            MonthPatterns.ForEach(item =>
                {
                    if (item.Pattern.IsMatch(token.Value))
                    {
                        tag = new RepeaterMonthName(item.Tag);
                        return;
                    }
                });
            return tag;
        }

        static ITag ScanSeasonNames(Token token, Options options)
        {
            throw new NotImplementedException();
        }

        static readonly Regex _timePattern =
            @"^\d{1,2}(:?\d{2})?([\.:]?\d{2})?$".Compile();


        static readonly List<RegexTagEntry<DayPortion>> DayPortionPatterns = new List<RegexTagEntry<DayPortion>>
            {
                new RegexTagEntry<DayPortion> { Pattern = "^ams?$".Compile(), Tag = DayPortion.AM },
                new RegexTagEntry<DayPortion> { Pattern = "^pms?$".Compile(), Tag = DayPortion.PM },
                new RegexTagEntry<DayPortion> { Pattern = "^mornings?$".Compile(), Tag = DayPortion.MORNING },
                new RegexTagEntry<DayPortion> { Pattern = "^afternoons?$".Compile(), Tag = DayPortion.AFTERNOON },
                new RegexTagEntry<DayPortion> { Pattern = "^evenings?$".Compile(), Tag = DayPortion.EVENING },
                new RegexTagEntry<DayPortion> { Pattern = "^(night|nite)s?$".Compile(), Tag = DayPortion.NIGHT },
            };


        static readonly List<RegexTagEntry<DayOfWeek>> DayPatterns = new List<RegexTagEntry<DayOfWeek>>
            {
                new RegexTagEntry<DayOfWeek> {Pattern ="^m[ou]n(day)?$".Compile(), Tag = DayOfWeek.Monday},
                new RegexTagEntry<DayOfWeek> {Pattern = "^t(ue|eu|oo|u|)s(day)?$".Compile(), Tag = DayOfWeek.Tuesday},
                new RegexTagEntry<DayOfWeek> {Pattern = "^tue$".Compile(), Tag = DayOfWeek.Tuesday},
                new RegexTagEntry<DayOfWeek> {Pattern = "^we(dnes|nds|nns)day$".Compile(), Tag = DayOfWeek.Wednesday},
                new RegexTagEntry<DayOfWeek> {Pattern = "^wed$".Compile(), Tag = DayOfWeek.Wednesday},
                new RegexTagEntry<DayOfWeek> {Pattern = "^th(urs|ers)day$".Compile(), Tag = DayOfWeek.Thursday},
                new RegexTagEntry<DayOfWeek> {Pattern = "^thu$".Compile(), Tag = DayOfWeek.Thursday},
                new RegexTagEntry<DayOfWeek> {Pattern = "^fr[iy](day)?$".Compile(), Tag = DayOfWeek.Friday},
                new RegexTagEntry<DayOfWeek> {Pattern = "^sat(t?[ue]rday)?$".Compile(), Tag = DayOfWeek.Saturday},
                new RegexTagEntry<DayOfWeek> {Pattern = "^su[nm](day)?$".Compile(), Tag = DayOfWeek.Sunday},
            };

        static readonly List<RegexTagEntry<MonthName>> MonthPatterns = new List<RegexTagEntry<MonthName>>
            {
                new RegexTagEntry<MonthName> {Pattern = "^jan\\.?(uary)?$".Compile(), Tag = MonthName.January},
                new RegexTagEntry<MonthName> {Pattern = "^feb\\.?(ruary)?$".Compile(), Tag = MonthName.February},
                new RegexTagEntry<MonthName> {Pattern = "^mar\\.?(ch)?$".Compile(), Tag = MonthName.March},
                new RegexTagEntry<MonthName> {Pattern = "^apr\\.?(il)?$".Compile(), Tag = MonthName.April},
                new RegexTagEntry<MonthName> {Pattern = "^may$".Compile(), Tag = MonthName.May},
                new RegexTagEntry<MonthName> {Pattern = "^jun\\.?e?$".Compile(), Tag = MonthName.June},
                new RegexTagEntry<MonthName> {Pattern = "^jul\\.?y?$".Compile(), Tag = MonthName.July},
                new RegexTagEntry<MonthName> {Pattern = "^aug\\.?(ust)?$".Compile(), Tag = MonthName.August},
                new RegexTagEntry<MonthName>
                    {
                        Pattern = "^sep\\.?(t\\.?|tember)?$".Compile(),
                        Tag = MonthName.September
                    },
                new RegexTagEntry<MonthName> {Pattern = "^oct\\.?(ober)?$".Compile(), Tag = MonthName.October},
                new RegexTagEntry<MonthName> {Pattern = "^nov\\.?(ember)?$".Compile(), Tag = MonthName.November},
                new RegexTagEntry<MonthName> {Pattern = "^dec\\.?(ember)?$".Compile(), Tag = MonthName.December},
            };

        static readonly List<RegexTagEntry<Type>> UnitPatterns = new List<RegexTagEntry<Type>>
            {
                new RegexTagEntry<Type> { Pattern = "^years?$".Compile(), Tag = typeof(RepeaterYear) },
                new RegexTagEntry<Type> { Pattern = "^seasons?$".Compile(), Tag = typeof(RepeaterSeason) },
                new RegexTagEntry<Type> { Pattern = "^months?$".Compile(), Tag = typeof(RepeaterMonth) },
                new RegexTagEntry<Type> { Pattern = "^fortnights?$".Compile(), Tag = typeof(RepeaterFortnight) },
                new RegexTagEntry<Type> { Pattern = "^weeks?$".Compile(), Tag = typeof(RepeaterWeek) },
                new RegexTagEntry<Type> { Pattern = "^weekends?$".Compile(), Tag = typeof(RepeaterWeekend) },
                new RegexTagEntry<Type> { Pattern = "^days?$".Compile(), Tag = typeof(RepeaterDay) },
                new RegexTagEntry<Type> { Pattern = "^hours?$".Compile(), Tag = typeof(RepeaterHour) },
                new RegexTagEntry<Type> { Pattern = "^minutes?$".Compile(), Tag = typeof(RepeaterMinute) },
                new RegexTagEntry<Type> { Pattern = "^seconds?$".Compile(), Tag = typeof(RepeaterSecond) }
            };
    }
}