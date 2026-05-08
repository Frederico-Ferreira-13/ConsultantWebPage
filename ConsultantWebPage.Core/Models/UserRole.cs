using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ConsultantWebPage.Core.Common;

namespace ConsultantWebPage.Core.Models
{
    [Table("UserRoles")]
    public class UsersRole : IEntity
    {
        [Key]
        public int UsersRoleId { get; private set; }

        [Required]
        [MaxLength(50)]
        public string RoleName { get; private set; } = string.Empty;

        private UsersRole() { }

        public UsersRole(string roleName)
        {
            ValidateRole(roleName);

            RoleName = roleName;
            UsersRoleId = default;
        }

        public UsersRole(int id, string name)
        {
            if (id <= 0)
            {
                throw new ArgumentException("O ID do Nível de Acesso é inválido.", nameof(id));
            }

            UsersRoleId = id;
            RoleName = name;
        }

        public void UpdateName(string newRoleName)
        {
            ValidateRole(newRoleName);

            if (!RoleName.Equals(newRoleName, StringComparison.Ordinal))
            {
                RoleName = newRoleName;
            }
        }

        private static void ValidateRole(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("O nome do Nível de Acesso é obrigatório.", nameof(name));
            }
            if (name.Length > 50)
            {
                throw new ArgumentException("O nome do Nível de Acesso não pode exceder 100 caracteres.", nameof(name));
            }
        }

        public int GetId() => UsersRoleId;

        public void SetId(int id)
        {
            if (UsersRoleId != 0)
            {
                throw new InvalidOperationException("Não é permitido alterar o ID de uma Entidade que já possui um ID.");
            }

            UsersRoleId = id;
        }

        public bool GetIsActive() => true;
    }
}
