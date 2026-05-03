namespace HikeConnect.Core.Settings
{
    /// <summary>
    /// Self-hosted Twenty CRM (GraphQL Core API). Bound from configuration section <c>Crm:Twenty</c>.
    /// </summary>
    public sealed class CrmTwentySettings
    {
        public const string SectionName = "Crm:Twenty";

        /// <summary>Server root URL, e.g. <c>http://server:3000</c> inside Docker.</summary>
        public string? BaseUrl { get; set; }

        /// <summary>API key from Twenty → Settings → APIs & Webhooks.</summary>
        public string? ApiKey { get; set; }

        /// <summary>Relative path under <see cref="BaseUrl"/> for Core GraphQL (no leading slash).</summary>
        public string GraphQlPath { get; set; } = "graphql";

        /// <summary>
        /// When false, sync calls succeed immediately without HTTP (no CRM configured).
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Plural GraphQL object API name в Core API (часто lowercase). Из него по умолчанию строится мутация:
        /// <c>createMany</c> + первая буква заглавная + остаток строки, например <c>behavioralleads</c> → <c>createManyBehavioralleads</c>.
        /// Если в вашей версии Twenty имя мутации другое (например <c>createManyBehavioralLeads</c>), задайте <see cref="CreateManyMutationName"/>.
        /// </summary>
        public string ObjectPluralName { get; set; } = "behavioralleads";

        /// <summary>Input element type for create-many, e.g. <c>BehavioralLeadCreateInput</c> — должен совпадать со схемой workspace.</summary>
        public string CreateInputTypeName { get; set; } = "BehavioralLeadCreateInput";

        /// <summary>
        /// Явное имя поля мутации GraphQL (например <c>createManyBehavioralLeads</c>), если автоматика из <see cref="ObjectPluralName"/> не совпадает со схемой.
        /// </summary>
        public string? CreateManyMutationName { get; set; }

        public bool IsConfigured =>
            Enabled
            && !string.IsNullOrWhiteSpace(BaseUrl)
            && !string.IsNullOrWhiteSpace(ApiKey);

        public string GetCreateManyMutationFieldName()
        {
            if (!string.IsNullOrEmpty(CreateManyMutationName))
                return CreateManyMutationName;

            var plural = string.IsNullOrEmpty(ObjectPluralName) ? "behavioralleads" : ObjectPluralName;
            return $"createMany{char.ToUpperInvariant(plural[0])}{plural[1..]}";
        }
    }
}
