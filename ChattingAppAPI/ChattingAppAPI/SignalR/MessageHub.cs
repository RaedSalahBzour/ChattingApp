using AutoMapper;
using ChattingAppAPI.DTOs;
using ChattingAppAPI.Entities;
using ChattingAppAPI.Extensions;
using ChattingAppAPI.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System.Security.AccessControl;

namespace ChattingAppAPI.SignalR;

public class MessageHub(IMessageRepository messageRepository
    , IUserRepository userRepository, IMapper mapper
    , IHubContext<PresenceHub> presenceHub) : Hub
{
    public override async Task OnConnectedAsync()
    {
        var http = Context.GetHttpContext();
        var otherUser = Context.GetHttpContext()?.Request.Query["user"];
        if (Context.User == null || string.IsNullOrEmpty(otherUser))
            throw new Exception("can not join group");
        var groupName = GetGroupName(Context.User.GetUserName(), otherUser);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        var group = await AddToGroup(groupName);
        await Clients.Group(groupName).SendAsync("UpdatedGroup", group);
        var messages = await messageRepository
            .GetMessageThread(Context.User.GetUserName(), otherUser!);
        await Clients.Caller.SendAsync("ReceiveMessagesThread", messages);
    }
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var group = await RemoveFromGroup();
        await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);
        await base.OnDisconnectedAsync(exception);
    }
    public async Task SendMessage(CreateMessageDto createMessageDto)
    {
        var username = Context.User?.GetUserName()
            ?? throw new Exception("Could not get user");

        if (username == createMessageDto.RecipientUsername.ToLower())
            throw new HubException("can not message your self");

        var sender = await userRepository.GetUserByUsernameAsync(username);
        var recipient = await userRepository
            .GetUserByUsernameAsync(createMessageDto.RecipientUsername);
        if (sender == null || recipient == null)
            throw new HubException("cannot send message at this time");
        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            Content = createMessageDto.Content,
            SenderUsername = username,
            RecipientUsername = createMessageDto.RecipientUsername,
        };
        var groupName = GetGroupName(sender.UserName, recipient.UserName);
        var group = await messageRepository.GetMessageGroup(groupName);
        if (group is not null &&
            group.Connections.Any(x => x.Username == recipient.UserName))
        {
            message.DateRead = DateTime.UtcNow;
        }
        else
        {
            var connections = await PresenceTracker.GetConnectionsForUser(recipient.UserName);
            //checks the nullable of the connections and the count is more than 0
            //it equals to if (connections is not null && connections.Count > 0 )
            if (connections is { Count: > 0 })
            {
                await presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived",
                    new { username = sender.UserName, KnownAs = sender.KnownAs });
            }
        }
        messageRepository.AddMessage(message);
        if (await messageRepository.SaveAllAsync())
        {
            await Clients.Group(groupName)
                .SendAsync("NewMessage", mapper.Map<MessageDto>(message));
        }
    }
    private async Task<Group> AddToGroup(string groupName)
    {
        var username = Context.User?.GetUserName() ?? throw new Exception("Cannot get username");
        var group = await messageRepository.GetMessageGroup(groupName);
        var connection = new Connection
        {
            ConnectionId = Context.ConnectionId,
            Username = username
        };
        if (group is null)
        {
            group = new Group { Name = groupName };
            messageRepository.AddGroup(group);
        }
        group.Connections.Add(connection);
        if (await messageRepository.SaveAllAsync()) return group;
        throw new HubException("faild to join group");
    }
    private async Task<Group> RemoveFromGroup()
    {
        var group = await messageRepository.GetGroupForConnection(Context.ConnectionId);
        var connection = group?.Connections
            .FirstOrDefault(c => c.ConnectionId == Context.ConnectionId);
        if (connection is not null && group is not null)
        {
            messageRepository.RemoveConnection(connection);
            if (await messageRepository.SaveAllAsync()) return group;
        }
        throw new Exception("faild to remove from group");
    }
    private string GetGroupName(string caller, string? other)
    {
        var stringCompare = string.CompareOrdinal(caller, other) < 0;
        return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
    }
}
