namespace SnipEx.Web.ViewModels.Tag
{
    using SnipEx.Data.Models;
    using SnipEx.Services.Mapping.Contracts;

    public class TagViewModel : IMapFrom<Tag>
    {
        public string Name { get; set; } = null!;

        public bool IsSelected { get; set; }
    }
}
