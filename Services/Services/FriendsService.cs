namespace Services.Services;

using Domain.DTO;
using Domain.Entities;
using Infrastructure.Persistence.DbContexts;
using Infrastructure.Persistence.Helpers.Constansts;
using Interfaces;
using Microsoft.EntityFrameworkCore;


public class FriendsService : IFriendsService {

    private readonly AppDbContext _context;

    public FriendsService(AppDbContext context)
    {
        _context = context;
    }

    public async Task SendFriendRequest(int senderId, int receiverId)
    {
        var request = new FriendRequest()
        {
            SenderId = senderId,
            ReceiverId = receiverId,
            DateCreated = DateTime.Now,
            Status = FriendShipStatus.Pending,
        };

        _context.FriendRequests.Add(request);
        await _context.SaveChangesAsync();
    }


    public async Task UpdateFriendshipRequestStatus(int loggedInUserId, int senderId, string newStatus)
    {
        var request = await _context.FriendRequests
            .Where(friendRequest => friendRequest.ReceiverId == loggedInUserId && friendRequest.SenderId == senderId)
            .FirstOrDefaultAsync();

        if (request != null){
            request.Status = newStatus;
            _context.Update(request);

            if (newStatus == FriendShipStatus.Accepted){
                if (newStatus == FriendShipStatus.Accepted){
                    var newFriendShip = new FriendShip()
                    {
                        SenderId = request.SenderId,
                        ReceiverId = request.ReceiverId,
                        DateCreated = DateTime.Now,
                        DateAccepted = DateTime.Now
                    };

                    _context.FriendShips.Add(newFriendShip);
                    _context.FriendRequests.Remove(request);
                    await _context.SaveChangesAsync();
                }
            }

            await _context.SaveChangesAsync();
        }
    }

    public async Task RemoveFriendship(int friendshipId)
    {
        var friendship = await _context.FriendShips.FirstOrDefaultAsync(friendship => friendship.Id == friendshipId);

        if (friendship != null){
            var senderId = friendship.SenderId;
            var receiverId = friendship.ReceiverId;

            
            _context.FriendShips.Remove(friendship);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<SuggestedUserResponse>> GetSuggestedFriends(int userId)
    {
        var existingFriendsIds = await _context.FriendShips
            .Where(n => n.SenderId == userId || n.ReceiverId == userId)
            .Select(n => n.SenderId == userId ? n.ReceiverId : n.SenderId)
            .ToListAsync();

        var pendingRequestIds = await _context.FriendRequests
            .Where(n => (n.SenderId == userId || n.ReceiverId == userId))
            .Select(n => n.SenderId == userId ? n.ReceiverId : n.SenderId)
            .ToListAsync();

        var suggestedFriends = await _context.Users
            .Where(n => n.Id != userId && !existingFriendsIds.Contains(n.Id) && !pendingRequestIds.Contains(n.Id))
            .Take(5)
            .Select(u => new SuggestedUserResponse()
            {
                User = u,
                FollowerCount = _context.FriendShips.Count(n => n.SenderId == u.Id || n.ReceiverId == u.Id),
            })
            .ToListAsync();

        return suggestedFriends;
    }

    public async Task SendFriendshipRequest(int senderId, int receiverId)
    {
        var request = _context.FriendRequests.FirstOrDefault(n => n.SenderId == senderId && n.ReceiverId == receiverId);

        if (request == null){
            var newRequest = new FriendRequest()
            {
                ReceiverId = receiverId,
                SenderId = senderId,
                DateCreated = DateTime.UtcNow,
                Status = FriendShipStatus.Pending,
            };

            _context.FriendRequests.Add(newRequest);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<SentRequestResponse>?> GetSentFriendshipRequests(int userId)
    {
        var requests = await _context.FriendRequests
            .Where(friendRequest => friendRequest.SenderId == userId)
            .Select(friendRequest => new SentRequestResponse()
            {
                RequestId = friendRequest.Id,
                FullName = friendRequest.Receiver.FullName,
                ProfilePictureUrl = friendRequest.Receiver.ProfilePictureUrl,
                CreatedDate = friendRequest.DateCreated,
                SentRequestUserId = friendRequest.ReceiverId,
                Status = friendRequest.Status,
            })
            .OrderByDescending(f => f.CreatedDate)
            .ToListAsync();

        var accepted = requests.Where(r => r.Status == FriendShipStatus.Accepted).ToList();
        var rejected = requests.Where(r => r.Status == FriendShipStatus.Rejected).ToList();
        var cancelled = requests.Where(r => r.Status == FriendShipStatus.Cancelled).ToList();
        var pending = requests.Where(r => r.Status == FriendShipStatus.Pending).ToList();
        var sortedRequests = accepted.Concat(rejected).Concat(cancelled).Concat(pending).ToList();


        return sortedRequests;
    }

    public async Task<List<FriendResponse>?> GetFriendships(int userId)
    {
        var friends = await _context.FriendShips
            .Where(friendship => friendship.SenderId == userId || friendship.ReceiverId == userId)
            .Select(friendship => new FriendResponse()
            {
                FriendshipId = friendship.Id,
                FriendId = friendship.SenderId == userId ? friendship.ReceiverId : friendship.SenderId,
                ProfilePictureUrl = friendship.SenderId == userId ? friendship.Receiver.ProfilePictureUrl : friendship.Sender.ProfilePictureUrl,
                FullName = friendship.SenderId == userId ? friendship.Receiver.FullName : friendship.Sender.FullName,
                CreatedDate = friendship.DateCreated,
            })
            .OrderByDescending(f => f.CreatedDate)
            .ToListAsync();

        return friends;
    }

    public async Task CancelFriendshipRequest(int loggedInUserId, int receiverId)
    {
        var friendshipRequest = await _context.FriendRequests
            .Where(friendRequest => friendRequest.SenderId == loggedInUserId && friendRequest.ReceiverId == receiverId)
            .FirstOrDefaultAsync();

        if (friendshipRequest != null){
            _context.FriendRequests.Remove(friendshipRequest);
            _context.SaveChanges();
        }
    }

    public async Task<List<ReceivedRequestsResponse>?> GetReceivedFriendshipRequests(int userId)
    {
        var receivedRequests = await _context.FriendRequests
            .Where(friendRequest => friendRequest.ReceiverId == userId && friendRequest.Status == FriendShipStatus.Pending)
            .Select(request => new ReceivedRequestsResponse()
            {
                FullName = request.Sender.FullName,
                ProfilePictureUrl = request.Sender.ProfilePictureUrl,
                UserId = request.SenderId,
                DateSent = request.DateCreated,
            }).OrderByDescending(f => f.DateSent)
            .ToListAsync();

        return receivedRequests;
    }

    public async Task OkButton(int friendshipRequestId)
    {
        var request = await _context.FriendRequests.FirstOrDefaultAsync(f => f.Id == friendshipRequestId);

        if (request != null){
            _context.FriendRequests.Remove(request);
            await _context.SaveChangesAsync();
        }
    }

}
