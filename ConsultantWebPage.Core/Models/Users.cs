using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using ConsultantWebPage.Core.Common;

namespace ConsultantWebPage.Core.Models
{
    [Table("Users")]
    public class Users : IEntity, ISoftDeletable
    {
        [Key]
        public int UserId { get; protected set; }

        public int UsersRoleId { get; protected set; }
        public int? LeadsId { get; protected set; }

        public bool IsActive { get; protected set; } = true;
        public bool IsApproved { get; protected set; } = false;

        [Required]
        [MaxLength(255)]
        public string Name { get; protected set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string UserName { get; protected set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Email { get; protected set; } = string.Empty;

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

        [ForeignKey("LeadsId")]
        public virtual Leads? Contact { get; private set; }

        public virtual ICollection<Leads> MyLeads { get; private set; } = new List<Leads>();

        private Users() { }

        public Users(string name, string userName, string email, int usersRoleId, bool isApproved)
        {
            ValidateName(name, nameof(name));
            ValidateUserName(userName, nameof(userName));
            ValidateEmail(email);

            if (usersRoleId <= 0)
                throw new ArgumentException("O UsersRoleId deve ser positivo.", nameof(usersRoleId));

            Name = name;
            UserName = userName;
            Email = email;
            UsersRoleId = usersRoleId;
            IsApproved = isApproved;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdateName(string newName)
        {
            CheckIfActive();
            ValidateName(newName, nameof(newName));

            if (Name != newName)
            {
                Name = newName;
                SetLastUpdatedAt();
            }
        }

        public void UpdateUserName(string newUserName)
        {
            CheckIfActive();
            ValidateUserName(newUserName, nameof(newUserName));

            if (!string.Equals(UserName, newUserName, StringComparison.OrdinalIgnoreCase))
            {
                UserName = newUserName;
                SetLastUpdatedAt();
            }
        }

        public void UpdateEmail(string newEmail)
        {
            CheckIfActive();
            ValidateEmail(newEmail);
            if (!string.Equals(Email, newEmail, StringComparison.OrdinalIgnoreCase))
            {
                Email = newEmail;
                SetLastUpdatedAt();
            }
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

        public void ChangeContact(int newLeadsId)
        {
            CheckIfActive();
            if (newLeadsId <= 0)
            {
                throw new ArgumentException("O novo ContactId deve ser positivo.", nameof(newLeadsId));
            }

            if (LeadsId != newLeadsId)
            {
                LeadsId = newLeadsId;
                Contact = null;
                SetLastUpdatedAt();
            }
        }

        public void SetContact(Leads leads)
        {
            if (LeadsId != leads.InquiryId)
                throw new InvalidOperationException("Erro de mapeamento: IDs não coincidem.");

            Contact = leads;
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

        private void ValidateName(string name, string paramName)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException($"O nome de utilizador não pode ser vazio ou nulo ({paramName}).");
            }
            if (name.Length > 255)
            {
                throw new ArgumentException("O nome de utilizador não pode exceder 100 caracteres.", paramName);
            }
        }

        private void ValidateUserName(string userName, string paramName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException($"O nome de utilizador não pode ser vazio ou nulo ({paramName}).");

            if (userName.Length > 255)
                throw new ArgumentException("O nome de utilizador não pode exceder 100 caracteres.", paramName);
        }

        private void ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("O email é obrigatório.", nameof(email));
            }
            if (email.Length > 255)
            {
                throw new ArgumentException("O email não pode exceder 255 caracteres.", nameof(email));
            }
            if (!IsValidEmailFormat(email))
            {
                throw new ArgumentException("Formato de email inválido.", nameof(email));
            }
        }

        private bool IsValidEmailFormat(string email)
        {
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
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
