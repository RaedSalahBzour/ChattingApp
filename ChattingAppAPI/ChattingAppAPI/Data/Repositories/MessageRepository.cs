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
    public void AddMessage(Message message)
    {
        context.Messages.Add(message);
    }

    public void DeleteMessage(Message message)
    {
        context.Messages.Remove(message);
    }

    public async Task<Message?> GetMessage(int id)
    {
        return await context.Messages.FindAsync(id);
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
            .Include(m => m.Sender).ThenInclude(s => s.Photos)
            .Include(m => m.Recipient).ThenInclude(r => r.Photos)
            .Where(m =>
            (m.SenderUsername == currentUsername &&
            m.RecipientDeleted == false && m.RecipientUsername == recipientUsername) ||
           (m.RecipientUsername == currentUsername &&
           m.SenderDeleted == false && m.SenderUsername == recipientUsername))
            .OrderBy(m => m.MessageSent).ToListAsync();
        var unreadMessages = messages
            .Where(m => m.DateRead == null &&
            m.RecipientUsername == currentUsername).ToList();
        if (unreadMessages.Count != 0)
        {
            unreadMessages.ForEach(um => um.DateRead = DateTime.UtcNow);
            await context.SaveChangesAsync();
        }
        return mapper.Map<IEnumerable<MessageDto>>(messages);
    }

    public async Task<bool> SaveAllAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }
}
