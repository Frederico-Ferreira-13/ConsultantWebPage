using System;
using ConsultantWebPage.Core.Models;
using ConsultantWebPage.Core.Common;

namespace ConsultantWebPage.Contracts.Interfaces.Services;

public interface ICommentsServices
{
    Task<Result<Comments>> GetCommentDetailAsync(int messageId, int currentUserId);
    Task<Result<IEnumerable<Comments>>> GetUserInboxAsync(int userId);
    Task<Result<IEnumerable<Comments>>> GetChatHistoryAsync(int carId, int buyerId, int sellerId);

    Task<Result<Comments>> SendInquiryAsync(int carId, int senderId, string subject, string messageText);
    Task<Result> MarkAsReadAsync(int messageId, int currentUserId);
    Task<Result> ArchiveCommentAsync(int messageId, int currentUserId);

    Task<Result<int>> GetUnreadCountAsync(int userId);

    Task<Result<IEnumerable<Comments>>> GetChatHistoryAsync(int carId, int currentUserId);

    Task<Result<Comments>> SendCommentAsync(int carId, int senderId, int receiverId, string subject,
        string messageText, int? parentMessageId = null);

    Task<Result> EditCommentAsync(int messageId, int currentUserId, string newText);
}
