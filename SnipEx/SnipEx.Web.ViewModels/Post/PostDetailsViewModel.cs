﻿namespace SnipEx.Web.ViewModels.Post
{
    using System.Globalization;

    using AutoMapper;
    using SnipEx.Data.Models;
    using SnipEx.Services.Mapping.Contracts;
    using SnipEx.Web.ViewModels.Comment;

    using static Common.ApplicationConstants;
    using static Common.EntityValidationConstants.Post;

    public class PostDetailsViewModel : IMapFrom<Post>, IHaveCustomMappings
    {
        public PostDetailsViewModel()
        {
            this.CreatedAt = DateTime.UtcNow
                .ToString(CreatedAtFormat, CultureInfo.InvariantCulture);
        }

        public string Id { get; set; } = null!;

        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;

        public string LanguageName { get; set; } = null!;

        public string? UserName { get; set; }

        public string CreatedAt { get; set; }

        public List<string> Tags { get; set; } = new List<string>();

        public List<CommentViewModel> Comments { get; set; } = new List<CommentViewModel>();

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Post, PostDetailsViewModel>()
                .ForMember(d => d.CreatedAt, opt => opt.Ignore())
                .ForMember(d => d.UserName, opt =>
                    opt.MapFrom(s => s.User != null ? s.User.UserName : NoUserName));
        }
    }
}
