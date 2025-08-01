﻿namespace SnipEx.Web.ViewModels.Post
{
    using System.Globalization;
    using System.ComponentModel.DataAnnotations;

    using AutoMapper;

    using SnipEx.Data.Models;
    using SnipEx.Web.ViewModels.Tag;
    using SnipEx.Web.ViewModels.Language;
    using SnipEx.Services.Mapping.Contracts;

    using static Common.EntityValidationConstants.Post;

    public class AddPostFormModel : IMapTo<Post>, IHaveCustomMappings
    {
        public AddPostFormModel()
        {
            this.CreatedAt = DateTime.UtcNow
                .ToString(CultureInfo.InvariantCulture);
        }

        [Required]
        [StringLength(TitleMaxLength, MinimumLength = TitleMinLength)]
        public string Title { get; set; }

        [Required]
        [StringLength(ContentMaxLength, MinimumLength = ContentMinLength)]
        public string Content { get; set; }

        [Required]
        public string CreatedAt { get; set; }

        public string? UserId { get; set; }

        public IEnumerable<AddTagFormModel> Tags { get; set; } = new 
            HashSet<AddTagFormModel>();

        [Required]
        public string LanguageId { get; set; }

        public IEnumerable<LanguageViewModel> AvailableLanguages { get; set; }
            = new HashSet<LanguageViewModel>();

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<AddPostFormModel, Post>()
                .ForMember(dest => dest.Views, opt => opt.MapFrom(src => 0))
                .ForMember(d => d.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        }
    }
}
