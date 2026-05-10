using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models.ValueObjects;

namespace ConsultantWebPage.Core.Models
{
    [Table("Users")]
    public class Users : IEntity, ISoftDeletable
    {
        [Key]
        public int UserId { get; protected set; }
        public int UsersRoleId { get; protected set; }        

        public bool IsActive { get; protected set; } = true;
        public bool IsApproved { get; protected set; } = false;

        [Required]
        [MaxLength(255)]
        public string Name { get; protected set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string UserName { get; protected set; } = string.Empty;

        public ContactInfo Contact { get; protected set; } = null!;

        public string PasswordHash { get; protected set; } = string.Empty;
        public string Salt { get; protected set; } = string.Empty;

        public string? ProfilePicture { get; protected set; }
        public string? FacebookUrl { get; protected set; }
        public string? InstagramUrl { get; protected set; }
        public string? TwitterUrl { get; protected set; }

        public DateTime CreatedAt { get; protected set; }
        public DateTime? LastUpdatedAt { get; protected set; }

        [ForeignKey("UsersRoleId")]
        public virtual UsersRole? Role { get; private set; }      

        public virtual ICollection<Leads> MyLeads { get; private set; } = new List<Leads>();

        private Users() { }

        private Users(string name, string userName, ContactInfo contact, int usersRoleId, bool isApproved)
        {
            Name = name;
            UserName = userName;
            Contact = contact;
            UsersRoleId = usersRoleId;
            IsApproved = isApproved;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        public static Result<Users> Create(string name, string userName, ContactInfo contact, int usersRoleId, bool isApproved)
        {
            if (string.IsNullOrWhiteSpace(name)) return Error.Validation("Nome é obrigatório.");
            if (string.IsNullOrWhiteSpace(userName)) return Error.Validation("Username é obrigatório.");
            if (contact == null) return Error.Validation("Contacto é obrigatório.");
            if (usersRoleId <= 0) return Error.Validation("Role inválida.");

            return new Users(name, userName, contact, usersRoleId, isApproved);
        }

        public Result UpdateContact(ContactInfo newContact)
        {
            if (!IsActive) return Error.Conflict("Utilizador inativo.");
            Contact = newContact;
            SetLastUpdatedAt();
            return Result.Success();
        }       

        public void UpdateProfilePicture(string? fileName)
        {
            CheckIfActive();
            if (ProfilePicture != fileName)
            {
                ProfilePicture = fileName;
                SetLastUpdatedAt();
            }
        }

        public void UpdateSocialLinks(string? fb, string? insta, string? twitter)
        {
            CheckIfActive();
            // Só atualiza e marca data se algo mudou efetivamente
            if (FacebookUrl != fb || InstagramUrl != insta || TwitterUrl != twitter)
            {
                FacebookUrl = fb;
                InstagramUrl = insta;
                TwitterUrl = twitter;
                SetLastUpdatedAt();
            }
        }

        public void SetPassword(string newPasswordHash, string newSalt)
        {
            CheckIfActive();
            if (string.IsNullOrWhiteSpace(newPasswordHash) || string.IsNullOrWhiteSpace(newSalt))
            {
                throw new ArgumentException("O hash da password e o salt são obrigatórios.");
            }

            PasswordHash = newPasswordHash;
            Salt = newSalt;
            SetLastUpdatedAt();
        }

        public void ChangeRole(int newUsersRoleId)
        {
            CheckIfActive();
            if (newUsersRoleId <= 0)
            {
                throw new ArgumentException("O novo UsersRoleId deve ser positivo.", nameof(newUsersRoleId));
            }

            if (UsersRoleId != newUsersRoleId)
            {
                UsersRoleId = newUsersRoleId;
                SetLastUpdatedAt();
            }
        }        

        public void Deactivate()
        {
            if (this.IsActive)
            {
                this.IsActive = false;
                SetLastUpdatedAt();
            }
        }

        public void Activate()
        {
            if (!IsActive)
            {
                IsActive = true;
                SetLastUpdatedAt();
            }
        }

        public void Approve()
        {
            CheckIfActive();
            if (!IsApproved)
            {
                IsApproved = true;
                SetLastUpdatedAt();
            }
        }

        private void CheckIfActive()
        {
            if (!IsActive)
            {
                throw new InvalidOperationException("Operação inválida para utilizador inativo.");
            }
        }
        
        private void SetLastUpdatedAt()
        {
            LastUpdatedAt = DateTime.UtcNow;
        }

        public int GetId() => UserId;

        public void SetId(int id)
        {
            if (UserId != 0)
            {
                throw new InvalidOperationException("Não é permitido alterar o ID de uma Entidade que já possui um ID.");
            }
            UserId = id;
        }

        public bool GetIsActive() => IsActive;
    }
}
