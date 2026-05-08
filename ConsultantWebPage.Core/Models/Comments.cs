using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using ConsultantWebPage.Core.Common;


namespace ConsultantWebPage.Core.Models
{
    public class Comments : IEntity
    {
        [Key]
        public int CommentsId { get; protected set; }

        public int RealEstateId { get; protected set; }
        public int UserId { get; protected set; }


    [Required]
        public string CommentText { get; protected set; } = string.Empty;       
        public DateTime CreatedAt { get; protected set; }
        public DateTime LastUpdatedAt { get; protected set; }
        public bool IsEdited { get; protected set; }
        public bool IsDeleted { get; protected set; }

        public string OriginalComment { get; protected set; } = string.Empty;
        public int? ParentCommentId { get; protected set; }

        [ForeignKey("RealEstateId")]
        public virtual RealEstate? RealEstate { get; private set; }
        
        [ForeignKey("UserId")]
        public virtual Users? Users { get; private set; }

        private Comments() { }

        public Comments(int realEstateId, int userId, string commentText, int? parentCommentId = null)
        {
            ValidateComment(realEstateId, userId, commentText);

            RealEstateId = realEstateId;
            UserId = userId;
            CommentText = commentText;
            OriginalComment = commentText;
            CreatedAt = DateTime.UtcNow;
            LastUpdatedAt = DateTime.UtcNow;
            IsEdited = false;
            IsDeleted = false;
            ParentCommentId = parentCommentId;
        }

        public void UpdateComment([NotNull] string newCommentText)
        {
            if(IsDeleted)         
                throw new InvalidOperationException("Não é possível editar um comentário eliminado.");

            if (string.IsNullOrWhiteSpace(newCommentText))
                throw new ArgumentException("Comentário não pode ser vazio.", nameof(newCommentText));

            if(newCommentText.Length > 1000)
                throw new ArgumentException("Comentário não pode exceder 1000 caracteres.", nameof(newCommentText));

            
            if((DateTime.UtcNow - CreatedAt).TotalDays > 30)
                throw new InvalidOperationException("Não é possível editar um comentário com mais de 30 dias.");

            if(CommentText != newCommentText)
            {
                OriginalComment = string.IsNullOrEmpty(OriginalComment) ? CommentText : OriginalComment;                
                CommentText = newCommentText;
                LastUpdatedAt = DateTime.UtcNow;
                IsEdited = true;
            }            
        }

        public void Delete()
        {
            if(!IsDeleted)
            {
                IsDeleted = true;
                CommentText = "[Comentário Eliminado pelo utilizador]";
                LastUpdatedAt = DateTime.UtcNow;
            }
        }

        public int GetId() => CommentsId;

        public void SetId(int id)
        {
            if  (CommentsId != 0)
                throw new ArgumentException("Não é permitido alterar o ID de uma Entidade que já possui um ID.");

            CommentsId = id;
        }

        public bool GetIsActive() => !IsDeleted;

        private void ValidateComment(int realEstateId, int userId, string commentText)
        {
            if (realEstateId <= 0) throw new ArgumentException("RealEstateId inválido.");
            if (userId <= 0) throw new ArgumentException("UserId inválido.");
            if (string.IsNullOrWhiteSpace(commentText)) throw new ArgumentException("O texto do comentário é obrigatório.");
        }
    }
}


