﻿namespace SnipEx.Web.ViewModels.Comment
{
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;

    using AutoMapper;
    using SnipEx.Data.Models;
    using SnipEx.Services.Mapping.Contracts;

    using static Common.EntityValidationConstants.Comment;

    public class AddCommentReplyFormModel : IMapTo<Comment>, IHaveCustomMappings
    {
        public AddCommentReplyFormModel()
        {
            this.CreatedAt = DateTime.UtcNow.ToString(CreatedAtFormat, CultureInfo.InvariantCulture);
        }

        [Required]
        [StringLength(ContentMaxLength, MinimumLength = ContentMinLength)]
        public string Content { get; set; } = null!;

        [Required]
        public string CreatedAt { get; set; }

        [Required]
        public string PostId { get; set; } = null!;

        public string? UserId { get; set; }

        [Required]
        public string ParentCommentId { get; set; } = null!;

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<AddCommentReplyFormModel, Comment>()
                .ForMember(d => d.Id,
                    opt => opt.Ignore())
                .ForMember(d => d.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(d => d.PostId,
                    opt => opt.Ignore())
                .ForMember(d => d.ParentCommentId,
                    opt => opt.Ignore());
        }
    }
}
