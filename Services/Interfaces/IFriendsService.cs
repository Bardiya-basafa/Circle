namespace Services.Interfaces;

using Domain.DTO;
using Domain.Entities;


public interface IFriendsService {

    Task SendFriendRequest(int senderId, int receiverId);

    Task UpdateFriendshipRequestStatus(int loggedInUserId, int senderId, string newStatus);

    Task RemoveFriendship(int friendshipId);

    Task<List<SuggestedUserResponse>> GetSuggestedFriends(int userId);

    Task SendFriendshipRequest(int senderId, int receiverId);

    Task<List<SentRequestResponse>?> GetSentFriendshipRequests(int userId);

    Task<List<FriendResponse>?> GetFriendships(int userId);

    Task CancelFriendshipRequest(int loggedInUserId, int receiverId);

    Task<List<ReceivedRequestsResponse>?> GetReceivedFriendshipRequests(int userId);

    Task OkButton(int friendshipRequestId);

}
