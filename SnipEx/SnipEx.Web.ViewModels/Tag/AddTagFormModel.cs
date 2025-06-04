namespace SnipEx.Web.ViewModels.Tag
{
    using System.ComponentModel.DataAnnotations;

    using SnipEx.Data.Models;
    using SnipEx.Services.Mapping.Contracts;

    using static Common.EntityValidationConstants.Tag;
    public class AddTagFormModel : IMapTo<Tag>
    {
        [Required]
        [StringLength(NameMaxLength, MinimumLength = NameMinLength)]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }
    }
}
