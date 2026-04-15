using System.Globalization;
using HikeConnect.Core.Dtos;
using HikeConnect.Core.Entities;
using HikeConnect.Core.Interfaces;

namespace HikeConnect.Application.Services
{
    public class MatchingService : IMatchingService
    {
        private const string DefaultPacingKey = "pace_balanced";
        private const string DefaultConflictKey = "conflict_compromise";

        private static readonly string[] PacingPriority =
        {
            "pace_balanced",
            "pace_comfort",
            "pace_sport"
        };

        private static readonly string[] ConflictPriority =
        {
            "conflict_compromise",
            "conflict_direct",
            "conflict_avoid"
        };

        private static readonly Dictionary<string, string> PacingLabels = new(StringComparer.OrdinalIgnoreCase)
        {
            ["pace_comfort"] = "Комфортный темп",
            ["pace_balanced"] = "Сбалансированный темп",
            ["pace_sport"] = "Спортивный темп"
        };

        private static readonly Dictionary<string, string> ConflictLabels = new(StringComparer.OrdinalIgnoreCase)
        {
            ["conflict_compromise"] = "Компромиссный диалог",
            ["conflict_direct"] = "Прямой диалог",
            ["conflict_avoid"] = "Избегание конфликтов"
        };

        private static readonly Dictionary<string, SurveyContribution> ContributionByOption = new(StringComparer.OrdinalIgnoreCase)
        {
            // Темп
            ["pace_comfort"] = new("pace_comfort", null, 0, 5),
            ["pace_balanced"] = new("pace_balanced", null, 5, 10),
            ["pace_sport"] = new("pace_sport", null, 10, 15),

            // Стратегия в конфликте
            ["conflict_compromise"] = new(null, "conflict_compromise", 5, 10),
            ["conflict_direct"] = new(null, "conflict_direct", 10, 15),
            ["conflict_avoid"] = new(null, "conflict_avoid", 0, 5),

            // Риск
            ["risk_low"] = new(null, null, 10, 0),
            ["risk_medium"] = new(null, null, 20, 0),
            ["risk_high"] = new(null, null, 30, 0),

            // Дисциплина
            ["discipline_low"] = new(null, null, 0, 10),
            ["discipline_medium"] = new(null, null, 0, 20),
            ["discipline_high"] = new(null, null, 0, 30)
        };

        public BehavioralProfile BuildBehavioralProfile(BehavioralSurveySubmissionRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            if (request.UserId == Guid.Empty)
            {
                throw new ArgumentException("UserId is required.", nameof(request));
            }

            if (request.Answers is null || request.Answers.Count == 0)
            {
                throw new ArgumentException("At least one answer is required.", nameof(request));
            }

            var pacingScore = CreateCounter(PacingLabels.Keys);
            var conflictScore = CreateCounter(ConflictLabels.Keys);

            var riskTolerance = 0;
            var disciplineLevel = 0;

            foreach (var answer in request.Answers)
            {
                if (answer is null || string.IsNullOrWhiteSpace(answer.OptionCode))
                {
                    continue;
                }

                if (!ContributionByOption.TryGetValue(answer.OptionCode, out var contribution))
                {
                    throw new ArgumentException($"Unknown answer option: '{answer.OptionCode}'.", nameof(request));
                }

                if (!string.IsNullOrWhiteSpace(contribution.PacingKey))
                {
                    pacingScore[contribution.PacingKey] += 1;
                }

                if (!string.IsNullOrWhiteSpace(contribution.ConflictKey))
                {
                    conflictScore[contribution.ConflictKey] += 1;
                }

                riskTolerance += contribution.RiskDelta;
                disciplineLevel += contribution.DisciplineDelta;
            }

            var pacingKey = PickWinner(pacingScore, PacingPriority, DefaultPacingKey);
            var conflictKey = PickWinner(conflictScore, ConflictPriority, DefaultConflictKey);

            return new BehavioralProfile
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                PacingStyle = PacingLabels[pacingKey],
                ConflictStrategy = ConflictLabels[conflictKey],
                RiskTolerance = ClampToPercent(riskTolerance),
                DisciplineLevel = ClampToPercent(disciplineLevel),
                LastUpdatedAt = DateTime.UtcNow
            };
        }

        public CompatibilityReport BuildCompatibilityReport(BehavioralProfile authorProfile, BehavioralProfile targetProfile)
        {
            ArgumentNullException.ThrowIfNull(authorProfile);
            ArgumentNullException.ThrowIfNull(targetProfile);

            if (authorProfile.UserId == Guid.Empty)
            {
                throw new ArgumentException("Author profile must contain UserId.", nameof(authorProfile));
            }

            if (targetProfile.UserId == Guid.Empty)
            {
                throw new ArgumentException("Target profile must contain UserId.", nameof(targetProfile));
            }

            var riskPoints = new List<CompatibilityPoint>();
            var matchPoints = new List<CompatibilityPoint>();

            var paceScore = ScoreCategorical(
                authorProfile.PacingStyle,
                targetProfile.PacingStyle,
                maxWeight: 30,
                matchCode: "pace_match",
                matchMessage: "Совпадает стиль темпа в походе.",
                mismatchCode: "pace_mismatch",
                mismatchMessage: "Разный предпочитаемый темп движения.",
                metricLabel: "Темп в походе",
                matchPoints,
                riskPoints);

            var conflictScore = ScoreCategorical(
                authorProfile.ConflictStrategy,
                targetProfile.ConflictStrategy,
                maxWeight: 25,
                matchCode: "conflict_match",
                matchMessage: "Похожие подходы к решению конфликтов.",
                mismatchCode: "conflict_mismatch",
                mismatchMessage: "Разные подходы к разрешению конфликтов.",
                metricLabel: "Конфликты в группе",
                matchPoints,
                riskPoints);

            var riskScore = ScoreNumeric(
                authorValue: authorProfile.RiskTolerance,
                targetValue: targetProfile.RiskTolerance,
                maxWeight: 25,
                metricCode: "risk_tolerance",
                metricLabel: "отношение к риску",
                matchPoints,
                riskPoints);

            var disciplineScore = ScoreNumeric(
                authorValue: authorProfile.DisciplineLevel,
                targetValue: targetProfile.DisciplineLevel,
                maxWeight: 20,
                metricCode: "discipline_level",
                metricLabel: "уровень дисциплины",
                matchPoints,
                riskPoints);

            var compatibilityPercentage = (int)Math.Round(
                paceScore + conflictScore + riskScore + disciplineScore,
                MidpointRounding.AwayFromZero);

            return new CompatibilityReport
            {
                Id = Guid.NewGuid(),
                AuthorId = authorProfile.UserId,
                TargetId = targetProfile.UserId,
                MatchPoints = matchPoints,
                RiskPoints = riskPoints,
                CompatibilityPercentage = ClampToPercent(compatibilityPercentage),
                SummaryText = GenerateSummaryText(authorProfile, targetProfile, compatibilityPercentage, matchPoints, riskPoints)
            };
        }

        private static Dictionary<string, int> CreateCounter(IEnumerable<string> keys)
        {
            return keys.ToDictionary(key => key, _ => 0, StringComparer.OrdinalIgnoreCase);
        }

        private static string PickWinner(IReadOnlyDictionary<string, int> score, IReadOnlyList<string> priority, string fallback)
        {
            if (score.Count == 0)
            {
                return fallback;
            }

            var maxValue = score.Values.Max();
            var winners = score
                .Where(x => x.Value == maxValue)
                .Select(x => x.Key)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (var option in priority)
            {
                if (winners.Contains(option))
                {
                    return option;
                }
            }

            return fallback;
        }

        private static int ClampToPercent(int value)
        {
            return Math.Clamp(value, 0, 100);
        }

        private static int ScoreCategorical(
            string? authorValue,
            string? targetValue,
            int maxWeight,
            string matchCode,
            string matchMessage,
            string mismatchCode,
            string mismatchMessage,
            string metricLabel,
            ICollection<CompatibilityPoint> matchPoints,
            ICollection<CompatibilityPoint> riskPoints)
        {
            if (!string.IsNullOrWhiteSpace(authorValue) &&
                !string.IsNullOrWhiteSpace(targetValue) &&
                string.Equals(authorValue, targetValue, StringComparison.OrdinalIgnoreCase))
            {
                matchPoints.Add(new CompatibilityPoint
                {
                    Code = matchCode,
                    Label = metricLabel,
                    Message = matchMessage,
                    Weight = maxWeight
                });

                return maxWeight;
            }

            riskPoints.Add(new CompatibilityPoint
            {
                Code = mismatchCode,
                Label = metricLabel,
                Message = mismatchMessage,
                Weight = maxWeight
            });

            return 0;
        }

        private static double ScoreNumeric(
            int authorValue,
            int targetValue,
            int maxWeight,
            string metricCode,
            string metricLabel,
            ICollection<CompatibilityPoint> matchPoints,
            ICollection<CompatibilityPoint> riskPoints)
        {
            var difference = Math.Abs(authorValue - targetValue);
            var normalizedDifference = Math.Min(difference, 100) / 100d;
            var score = maxWeight * (1d - normalizedDifference);

            var label = CapitalizeMetricLabel(metricLabel);

            if (difference <= 10)
            {
                matchPoints.Add(new CompatibilityPoint
                {
                    Code = $"{metricCode}_excellent",
                    Label = label,
                    Message = $"Очень близкое совпадение по параметру: {metricLabel}.",
                    Weight = 8
                });
            }
            else if (difference <= 25)
            {
                matchPoints.Add(new CompatibilityPoint
                {
                    Code = $"{metricCode}_good",
                    Label = label,
                    Message = $"Допустимое расхождение по параметру: {metricLabel}.",
                    Weight = 4
                });
            }
            else if (difference <= 40)
            {
                riskPoints.Add(new CompatibilityPoint
                {
                    Code = $"{metricCode}_warning",
                    Label = label,
                    Message = $"Замечено ощутимое расхождение по параметру: {metricLabel}.",
                    Weight = 6
                });
            }
            else
            {
                riskPoints.Add(new CompatibilityPoint
                {
                    Code = $"{metricCode}_critical",
                    Label = label,
                    Message = $"Критическое расхождение по параметру: {metricLabel}.",
                    Weight = 12
                });
            }

            return score;
        }

        private static string CapitalizeMetricLabel(string metricLabel)
        {
            if (string.IsNullOrWhiteSpace(metricLabel))
            {
                return metricLabel;
            }

            var culture = CultureInfo.GetCultureInfo("ru-RU");
            return char.ToUpper(metricLabel[0], culture) + metricLabel.Substring(1);
        }

        private static string GenerateSummaryText(
            BehavioralProfile authorProfile,
            BehavioralProfile targetProfile,
            int compatibilityPercentage,
            IReadOnlyCollection<CompatibilityPoint> matchPoints,
            IReadOnlyCollection<CompatibilityPoint> riskPoints)
        {
            return string.Empty;
        }

        private sealed record SurveyContribution(
            string? PacingKey,
            string? ConflictKey,
            int RiskDelta,
            int DisciplineDelta);
    }
}
