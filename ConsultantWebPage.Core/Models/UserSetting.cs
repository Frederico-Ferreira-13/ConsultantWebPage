using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using ConsultantWebPage.Core.Common;

namespace ConsultantWebPage.Core.Models
{
    [Table("UserSettings")]
    public class UserSettings : IEntity
    {
        [Key]
        public int UserSettingsId { get; protected set; }

        public int UserId { get; protected set; }

        [MaxLength(50)]
        public string Theme { get; protected set; } = "Light";

        [MaxLength(10)]
        public string Language { get; protected set; } = "pt-PT";

        public bool ReceiveNotifications { get; protected set; }

        [ForeignKey("UserId")]
        public virtual Users? User { get; private set; }

        private UserSettings()
        {
            Theme = string.Empty;
            Language = string.Empty;
        }

        public UserSettings(int userId, string theme, string language, bool receiveNotifications)
        {
            ValidateSettings(userId, theme, language);

            UserId = userId;
            Theme = theme;
            Language = language;
            ReceiveNotifications = receiveNotifications;
        }

        public void UpdateSettings(string newTheme, string newLanguage, bool newReceiveNotifications)
        {
            ValidateSettings(UserId, newTheme, newLanguage);

            Theme = newTheme;
            Language = newLanguage;
            ReceiveNotifications = newReceiveNotifications;
        }

        private static void ValidateSettings(int userId, [NotNull] string? theme, [NotNull] string? language)
        {
            if (userId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(userId), "O ID do Utilizador deve ser positivo.");
            }
            if (string.IsNullOrWhiteSpace(theme))
            {
                throw new ArgumentException("O tema não pode ser vazio.", nameof(theme));
            }
            if (string.IsNullOrWhiteSpace(language))
            {
                throw new ArgumentException("O idioma não pode ser vazio.", nameof(language));
            }

            if (language.Length > 10)
            {
                throw new ArgumentException("O idioma não pode exceder 10 caracteres (ex: 'pt-PT')", nameof(language));
            }
        }

        public void UpdateTheme(string newTheme)
        {
            if (Theme != newTheme)
            {
                UpdateSettings(newTheme, Language, ReceiveNotifications);
            }
        }

        public void UpdateLanguage(string newLanguage)
        {
            if (Language != newLanguage)
            {
                UpdateSettings(Theme, newLanguage, ReceiveNotifications);
            }
        }

        public void UpdateReceiveNotifications(bool newNotificationsEnabled)
        {
            if (ReceiveNotifications != newNotificationsEnabled)
            {
                UpdateSettings(Theme, Language, newNotificationsEnabled);
            }
        }

        public int GetId() => UserSettingsId;

        public void SetId(int id)
        {
            if (UserSettingsId != 0)
            {
                throw new InvalidOperationException("Não é permitido alterar o ID de uma Entidade que já possui um ID.");
            }
            UserSettingsId = id;
        }

        public bool GetIsActive() => true;
    }
}
