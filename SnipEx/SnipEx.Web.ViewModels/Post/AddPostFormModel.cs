﻿namespace SnipEx.Web.ViewModels.Post
{
    using System.Globalization;
    using System.ComponentModel.DataAnnotations;

    using AutoMapper;
    using SnipEx.Data.Models;
    using SnipEx.Services.Mapping.Contracts;

    using static Common.EntityValidationConstants.Post;

    public class AddPostFormModel : IMapTo<Post>, IHaveCustomMappings
    {
        public AddPostFormModel()
        {
            this.CreatedAt = DateTime.UtcNow
                .ToString(CreatedAtFormat, CultureInfo.InvariantCulture);
        }

        [Required]
        [StringLength(TitleMaxLength, MinimumLength = TitleMinLength)]
        public string Title { get; set; }

        [Required]
        [StringLength(ContentMaxLength, MinimumLength = ContentMinLength)]
        public string Content { get; set; }

        [Required]
        public string CreatedAt { get; set; }

        public List<string> Tags { get; set; } = new List<string>();
        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<AddPostFormModel, Post>()
                .ForMember(dest => dest.Views, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore());
        }
    }
}
