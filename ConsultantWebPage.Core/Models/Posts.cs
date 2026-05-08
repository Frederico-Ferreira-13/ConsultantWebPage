using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ConsultantWebPage.Core.Common;

namespace ConsultantWebPage.Core.Models
{
    public class Posts : IEntity
    {
        [Key]
        public int PostId { get; protected set; }

        public int AuthorId { get; protected set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; protected set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Slug { get; protected set; } = string.Empty;

        [Required]
        public string Content { get; protected set; } = string.Empty;

        [MaxLength(500)]
        public string Summary { get; protected set; } = string.Empty;

        public string ImageUrl { get; protected set; } = string.Empty;

        public bool IsPublished { get; protected set; }

        public DateTime CreatedAt { get; protected set; }
        public DateTime LastUpdatedAt { get; protected set; }

        [ForeignKey("AuthorId")]
        public virtual Users? Author { get; private set; }

        private Posts() { }

        public Posts(int authorId, string title, string slug, string content, string summary, string imageUrl, bool isPublished)
        {
            ValidatePost(authorId, title, slug, content);

            AuthorId = authorId;
            Title = title;
            Slug = slug.ToLower().Replace(" ", "-"); // Slugs costumam ser minúsculos e sem espaços
            Content = content;
            Summary = summary;
            ImageUrl = imageUrl;
            IsPublished = isPublished;
            CreatedAt = DateTime.UtcNow;
            LastUpdatedAt = DateTime.UtcNow;
        }

        public void UpdatePost(string title, string slug, string content, string summary, string imageUrl, bool isPublished)
        {
            ValidatePost(AuthorId, title, slug, content);

            Title = title;
            Slug = slug.ToLower().Replace(" ", "-");
            Content = content;
            Summary = summary;
            ImageUrl = imageUrl;
            IsPublished = isPublished;
            LastUpdatedAt = DateTime.UtcNow;
        }

        private void ValidatePost(int authorId, string title, string slug, string content)
        {
            if (authorId <= 0)
                throw new ArgumentException("Author ID must be a positive integer.");

            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Post title cannot be empty.");

            if (string.IsNullOrWhiteSpace(slug))
                throw new ArgumentException("Post slug cannot be empty.");

            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Post content cannot be empty.");
        }

        public int GetId() => PostId;

        public void SetId(int id)
        {
            if (PostId != 0)
            {
                throw new InvalidOperationException("Cannot change the ID of an entity that already has an ID.");
            }
            PostId = id;
        }

        public bool GetIsActive() => IsPublished; // Assuming posts are always active for this example
    }
}
