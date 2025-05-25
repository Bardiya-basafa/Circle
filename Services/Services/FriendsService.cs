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

    public async Task UpdateFriendRequestStatus(int requesId, string newStatus)
    {
        var request = await _context.FriendRequests.FirstOrDefaultAsync(x => x.Id == requesId);

        if (request != null){
            request.Status = newStatus;
            _context.Update(request);
            await _context.SaveChangesAsync();

            if (newStatus == FriendShipStatus.Accepted){
                var newFriendShip = new FriendShip()
                {
                    SenderId = request.SenderId,
                    ReceiverId = request.ReceiverId,
                    DateCreated = DateTime.Now,
                    DateAccepted = DateTime.Now
                };

                _context.FriendShips.Add(newFriendShip);
                await _context.SaveChangesAsync();
            }
        }
    }

    public async Task RemoveFriendship(int friendshipId)
    {
        var friendship = await _context.FriendShips.FirstOrDefaultAsync(x => x.Id == friendshipId);

        if (friendship != null){
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
            .Where(n => (n.SenderId == userId || n.ReceiverId == userId) && n.Status == FriendShipStatus.Pending)
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
                DateCreated = DateTime.Now,
                Status = FriendShipStatus.Pending,
            };

            _context.FriendRequests.Add(newRequest);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<SentRequestResponse>?> GetSentRequests(int userId)
    {
        var pendingRequests = await _context.FriendRequests
            .Where(friendRequest => friendRequest.SenderId == userId && friendRequest.Status == FriendShipStatus.Pending)
            .Select(friendRequest => new SentRequestResponse()
            {
                FullName = friendRequest.Receiver.FullName,
                ProfilePictureUrl = friendRequest.Receiver.ProfilePictureUrl,
                CreatedDate = friendRequest.DateCreated,
                SentRequestUserId = friendRequest.ReceiverId,
            })
            .OrderByDescending(f => f.CreatedDate)
            .ToListAsync();

        return pendingRequests;
    }

    public async Task<List<FriendResponse>?> GetFriends(int userId)
    {
        var friends = await _context.FriendShips
            .Where(friendship => friendship.SenderId == userId || friendship.ReceiverId == userId)
            .Select(friendship => new FriendResponse()
            {
                FriendId = friendship.SenderId == userId ? friendship.ReceiverId : friendship.SenderId,
                ProfilePictureUrl = friendship.SenderId == userId ? friendship.Receiver.ProfilePictureUrl : friendship.Sender.ProfilePictureUrl,
                FullName = friendship.SenderId == userId ? friendship.Receiver.FullName : friendship.Sender.FullName,
                CreatedDate = friendship.DateCreated,
            })
            .OrderByDescending(f => f.CreatedDate)
            .ToListAsync();

        return friends;
    }

    public async Task CancelFriendRequest(int loggedInUserId, int receiverId)
    {
        var friendshipRequest = await _context.FriendRequests
            .Where(friendRequest => friendRequest.SenderId == loggedInUserId && friendRequest.ReceiverId == receiverId)
            .FirstOrDefaultAsync();

        if (friendshipRequest != null){
            _context.FriendRequests.Remove(friendshipRequest);
            _context.SaveChanges();
        }
    }

}
