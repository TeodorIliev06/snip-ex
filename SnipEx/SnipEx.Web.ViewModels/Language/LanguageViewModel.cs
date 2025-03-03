namespace SnipEx.Web.ViewModels.Language
{
    using SnipEx.Data.Models;
    using SnipEx.Services.Mapping.Contracts;

    public class LanguageViewModel : IMapFrom<ProgrammingLanguage>
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
    }
}
