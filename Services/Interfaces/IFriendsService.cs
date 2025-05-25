namespace Services.Interfaces;

using Domain.DTO;
using Domain.Entities;


public interface IFriendsService {

    Task SendFriendRequest(int senderId, int receiverId);

    Task UpdateFriendRequestStatus(int requestId, string newStatus);

    Task RemoveFriendship(int friendshipId);

    Task<List<SuggestedUserResponse>> GetSuggestedFriends(int userId);

    Task SendFriendshipRequest(int senderId, int receiverId);

    Task<List<SentRequestResponse>?> GetSentRequests(int userId);

    Task<List<FriendResponse>?> GetFriends(int userId);

    Task CancelFriendRequest(int loggedInUserId, int receiverId);

}
