using AutoMapper;
using AutoMapper.QueryableExtensions;
using ChattingAppAPI.DTOs;
using ChattingAppAPI.Entities;
using ChattingAppAPI.Helpers;
using ChattingAppAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChattingAppAPI.Data.Repositories;

public class MessageRepository(ApplicationDbContext context, IMapper mapper) : IMessageRepository
{
    public void AddGroup(Group group)
    {
        context.Groups.Add(group);
    }

    public void AddMessage(Message message)
    {
        context.Messages.Add(message);
    }

    public void DeleteMessage(Message message)
    {
        context.Messages.Remove(message);
    }

    public async Task<Connection?> GetConnection(string ConnectionId)
    {
        return await context.Connections.FindAsync(ConnectionId);
    }

    public async Task<Group?> GetGroupForConnection(string ConnectionId)
    {
        return await context.Groups.Include(g => g.Connections)
            .Where(g => g.Connections
            .Any(c => c.ConnectionId == ConnectionId)).FirstOrDefaultAsync();
    }

    public async Task<Message?> GetMessage(int id)
    {
        return await context.Messages.FindAsync(id);
    }

    public async Task<Group?> GetMessageGroup(string groupName)
    {
        return await context.Groups.Include(g => g.Connections)
            .FirstOrDefaultAsync(g => g.Name == groupName);
    }

    public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
    {
        var query = context.Messages.OrderByDescending(x => x.MessageSent).AsQueryable();
        query = messageParams.Container switch
        {
            "Inbox" => query.Where(m => m.Recipient.UserName == messageParams.Username
            && m.RecipientDeleted == false),
            "Outbox" => query.Where(m => m.Sender.UserName == messageParams.Username
             && m.SenderDeleted == false),
            (_) => query.Where(m => m.RecipientUsername == messageParams.Username &&
            m.DateRead == null && m.RecipientDeleted == false),
        };
        var messages = query.ProjectTo<MessageDto>(mapper.ConfigurationProvider);
        return await PagedList<MessageDto>.CreateAsync(messages, messageParams.PageNumber,
            messageParams.PageSize);
    }

    public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername)
    {
        var messages = await context.Messages
            .Where(m =>
            (m.SenderUsername == currentUsername &&
            m.RecipientDeleted == false && m.RecipientUsername == recipientUsername) ||
           (m.RecipientUsername == currentUsername &&
           m.SenderDeleted == false && m.SenderUsername == recipientUsername))
            .OrderBy(m => m.MessageSent)
            .ProjectTo<MessageDto>(mapper.ConfigurationProvider)
            .ToListAsync();
        var unreadMessages = messages
            .Where(m => m.DateRead == null &&
            m.RecipientUsername == currentUsername).ToList();
        if (unreadMessages.Count != 0)
        {
            unreadMessages.ForEach(um => um.DateRead = DateTime.UtcNow);
            await context.SaveChangesAsync();
        }
        return messages;
    }

    public void RemoveConnection(Connection connection)
    {
        context.Connections.Remove(connection);
    }

    public async Task<bool> SaveAllAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }
}
